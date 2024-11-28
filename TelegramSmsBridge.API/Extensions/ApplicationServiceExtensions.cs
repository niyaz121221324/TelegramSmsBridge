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

        services.AddSignalR(); // Добавляем SignalR в DI контейнер

        return services;
    }

    private static void ConfigureTelegramBotClient(IServiceCollection services, IConfiguration configuration)
    {
        var botConfigSection = configuration.GetSection(nameof(TelegramSettings));
        var botToken = botConfigSection.Get<TelegramSettings>()!.BotToken;

        services.Configure<TelegramSettings>(configuration.GetSection(nameof(TelegramSettings)));

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