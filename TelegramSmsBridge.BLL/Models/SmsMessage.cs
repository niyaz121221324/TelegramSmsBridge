using Telegram.Bot.Types;

namespace TelegramSmsBridge.BLL.Models;

public class SmsMessage
{
    /// <summary>
    /// Номер телефона, на который отправляется сообщение.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty; 

    /// <summary>
    /// Текст самого сообщения.
    /// </summary>
    public string MessageContent { get; set; } = string.Empty;

    /// <summary>
    /// Создает объект SmsMessage на основе сообщения.
    /// </summary>
    /// <param name="message">Объект сообщения.</param>
    /// <returns>Новый объект SmsMessage.</returns>
    public static SmsMessage FromMessage(Message? message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        } 

        return new SmsMessage
        {
            PhoneNumber = ExtractPhoneNumber(message),
            MessageContent = message.Text ?? string.Empty
        };
    }

    /// <summary>
    /// Извлекает номер телефона из сообщения.
    /// </summary>
    /// <param name="message">Объект сообщения.</param>
    /// <returns>Номер телефона или пустая строка, если номер не указан.</returns>
    private static string ExtractPhoneNumber(Message message)
    {
        return message.Contact?.PhoneNumber ?? string.Empty;
    }

    public override string ToString()
    {
        return $"{PhoneNumber}:{MessageContent}";
    }
}