using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramSmsBridge.BLL.Models;
using TelegramSmsBridge.BLL.Services;
using TelegramSmsBridge.BLL.Services.Authentification;
using TelegramSmsBridge.DAL.Contexts;
using TelegramSmsBridge.DAL.Entities;
using TelegramSmsBridge.DAL.Repository;

namespace TelegramSmsBridge.API.Extensions;

public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Регистрация сервисов, необходимых для работы приложения.
    /// </summary>
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Регистрируем TelegramBotClient с использованием конфигурации
        ConfigureTelegramBotClient(services, configuration);
        services.AddSingleton<UpdateHandler>();
        services.ConfigureTelegramBotMvc();
        services.Configure<TelegramSettings>(configuration.GetSection(nameof(TelegramSettings)));

        // Настройка Cors для доступа к api
        ConfigureCorsPolicy(services);

        services.AddSignalR(); // Добавляем SignalR в DI контейнер
        services.AddSingleton<IUserIdProvider, NameIdentifierUserIdProvider>();
        services.AddSingleton<TelegramHub>();

        // Настраиваем аутентификацию по JWT токенам
        services.Configure<JWTSettings>(configuration.GetSection(nameof(JWTSettings)));
        services.AddSingleton<IJWTProvider, JWTProvider>();
        ConfigureJwtAuthentication(services, configuration);

        // Настраиваем подключение к бд пользователей
        ConfigureUsersDb(services, configuration);

        // Настраиваем подключение к NoSql бд MongoDb
        ConfigureMongoDb(services, configuration);

        // Добавляем сервис для кещирования in-memory cache
        services.AddMemoryCache();
        services.AddTransient(typeof(ICacheService<>), typeof(CacheService<>));

        return services;
    }

    private static void ConfigureUsersDb(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => 
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgesqlDbConnection"));
        });

        services.AddTransient<IUserRepository, UserRepository>();
    }

    private static void ConfigureMongoDb(IServiceCollection services, IConfiguration configuration)
    {
        var mongoClient = new MongoClient(configuration.GetConnectionString("MongoDbConnection"));
        services.AddSingleton<IMongoClient>(mongoClient);

        services.AddSingleton(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var databaseName = configuration["MongoDb:DatabaseName"];
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("MongoDb:DatabaseName is not configured");
            }
            return client.GetDatabase(databaseName);
        });

        services.AddTransient<IMongoDbRepository<Update>, UpdateRepository>();
        services.AddTransient<IMongoDbRepository<SmsMessage>, SmsMessageRepository>();
    }

    private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JWTSettings)).Get<JWTSettings>();

        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
        {
            throw new InvalidOperationException("JWT settings are not configured properly.");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, 
                    ValidateAudience = false, 
                    ValidateLifetime = true, 
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey 
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = HandleMessageReceived
                };
            });
    }

    // Обработчик события получения сообщения для настройки токена из строки запроса.
    private static Task HandleMessageReceived(MessageReceivedContext context)
    {
        var accessToken = context.Request.Query["access_token"];

        if (!string.IsNullOrEmpty(accessToken) && 
            context.HttpContext.Request.Path.StartsWithSegments("/notificationHub"))
        {
            context.Token = accessToken;
        }

        return Task.CompletedTask;
    }

    private static void ConfigureCorsPolicy(IServiceCollection services)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyOrigin() 
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }

    private static void ConfigureTelegramBotClient(IServiceCollection services, IConfiguration configuration)
    {
        var botToken = configuration.GetSection(nameof(TelegramSettings)).Get<TelegramSettings>();

        services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(httpClient =>
        {
            if (string.IsNullOrEmpty(botToken!.BotToken))
            {
                throw new InvalidOperationException("Bot token is not configured.");
            }

            return new TelegramBotClient(botToken!.BotToken, httpClient);
        });
    }
}