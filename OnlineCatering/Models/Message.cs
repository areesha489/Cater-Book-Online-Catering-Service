namespace OnlineCatering.Models;

public class Message
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public string SenderType { get; set; } = string.Empty;
    public int ReceiverId { get; set; }
    public string ReceiverType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime SentDate { get; set; } = DateTime.Now;
    public int? ReplyToMessageId { get; set; }
}
