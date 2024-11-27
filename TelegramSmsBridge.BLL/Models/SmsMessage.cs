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

    public override string ToString()
    {
        return $"PhoneNumber: {PhoneNumber}, MessageContent: {MessageContent}";
    }
}
