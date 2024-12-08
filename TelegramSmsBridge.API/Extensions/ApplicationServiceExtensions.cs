using Microsoft.AspNetCore.SignalR;
using Telegram.Bot;
using TelegramSmsBridge.BLL.Models;
using TelegramSmsBridge.BLL.Services;

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

        // Настройка Cors для доступа к api
        ConfigureCorsPolicy(services);

        services.AddSignalR(); // Добавляем SignalR в DI контейнер
        services.AddSingleton<TelegramHub>();
        services.AddSingleton<IUserIdProvider, ConnectionIdUserProvider>();

        return services;
    }

    private static void ConfigureCorsPolicy(this IServiceCollection services)
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
        var botConfigSection = configuration.GetSection(nameof(TelegramSettings));
        var botToken = botConfigSection.Get<TelegramSettings>()!.BotToken;

        services.Configure<TelegramSettings>(botConfigSection);

        services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(httpClient =>
        {
            if (string.IsNullOrEmpty(botToken))
            {
                throw new InvalidOperationException("Bot token is not configured.");
            }

            return new TelegramBotClient(botToken, httpClient);
        });
    }
}