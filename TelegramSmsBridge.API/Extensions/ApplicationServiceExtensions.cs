using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramSmsBridge.BLL.Models;

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
        services.Configure<TelegramSettings>(configuration.GetSection(nameof(TelegramSettings)));

        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var botToken = provider.GetRequiredService<IOptions<TelegramSettings>>().Value.BotToken;

            if (string.IsNullOrEmpty(botToken))
            {
                throw new InvalidOperationException("Bot token is not configured.");
            }

            return new TelegramBotClient(botToken);
        });
    }
}