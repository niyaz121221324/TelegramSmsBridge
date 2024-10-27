using Telegram.Bot;

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

        return services;
    }

    private static void ConfigureTelegramBotClient(IServiceCollection services, IConfiguration configuration)
    {
        string? botToken = configuration["Telegram:BotToken"];

        if (!string.IsNullOrEmpty(botToken))
        {
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

            //services.AddHttpClient("TelegramSmsBridgeWebhook")
            //    .RemoveAllLoggers()
            //    .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(botToken, httpClient));
        }
    }
}