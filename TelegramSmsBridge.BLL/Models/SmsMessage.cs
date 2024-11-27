namespace TelegramSmsBridge.BLL.Models;

public class SmsMessage
{
    /// <summary>
    /// Номер телефона, на который отправляется сообщение.
    /// </summary>
    public string PhoneNumber { get; set; } = ""; 

    /// <summary>
    /// Текст самого сообщения.
    /// </summary>
    public string MessageContent { get; set; } = "";

    public override string ToString()
    {
        return $"PhoneNumber: {PhoneNumber}, MessageContent: {MessageContent}";
    }
}
