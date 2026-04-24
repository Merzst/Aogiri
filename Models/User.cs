namespace Aogiri.Models;

public class User
{
    public int UserID { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateTime RegDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active"; // Active, Blocked
    public string Role { get; set; } = "User"; // User, Moderator, Admin
    public string? AvatarUrl { get; set; }

    public ICollection<Advertisement> Advertisements { get; set; } = new List<Advertisement>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}
