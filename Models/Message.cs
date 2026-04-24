namespace Aogiri.Models;

public class Message
{
    public int MessageID { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentDate { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    public int SenderID { get; set; }
    public User Sender { get; set; } = null!;

    public int ReceiverID { get; set; }
    public User Receiver { get; set; } = null!;

    public int? AdID { get; set; }
    public Advertisement? Advertisement { get; set; }
}
