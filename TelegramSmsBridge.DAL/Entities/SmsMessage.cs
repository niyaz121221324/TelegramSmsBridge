using Telegram.Bot.Types;

namespace TelegramSmsBridge.DAL.Entities;

public class SmsMessage
{
    /// <summary>
    /// Идентификатор чата Telegram
    /// </summary>
    public long ChatId { get; set; }

    /// <summary>
    /// Номер телефона, на который отправляется сообщение.
    /// </summary>
    public string PhoneNumber { get; set; }  

    /// <summary>
    /// Текст самого сообщения.
    /// </summary>
    public string MessageContent { get; set; }

    public SmsMessage(long chatId, string phoneNumber, string messageContent)
    {
        ChatId = chatId;
        PhoneNumber = phoneNumber;
        MessageContent = messageContent;
    }

    /// <summary>
    /// Преобразует объект сообщения в объект SmsMessage.
    /// </summary>
    /// <param name="message">Объект сообщения, на основе которого создается SmsMessage.</param>
    /// <returns>Экземпляр SmsMessage, созданный на основе переданного сообщения.</returns>
    public SmsMessage(Message? message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        } 
        
        ChatId = message.Chat.Id;
        PhoneNumber = ExtractPhoneNumber(message);
        MessageContent = message.Text ?? string.Empty;
    }

    /// <summary>
    /// Извлекает номер телефона из сообщения.
    /// </summary>
    /// <param name="message">Объект сообщения.</param>
    /// <returns>Номер телефона или пустая строка, если номер не указан.</returns>
    private string ExtractPhoneNumber(Message message)
    {
        if (message.ReplyToMessage != null && !string.IsNullOrEmpty(message.ReplyToMessage.Text))
        {
            string replyText = message.ReplyToMessage.Text;

            string[] parts = replyText.Split(':');
            return parts.Length > 0 ? parts[0].Trim() : string.Empty;
        }

        return string.Empty;
    }

    public override string ToString()
    {
        return $"{PhoneNumber}:{MessageContent}";
    }
}