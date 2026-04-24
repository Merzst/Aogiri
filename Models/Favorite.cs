namespace Aogiri.Models;

public class Favorite
{
    public int FavoriteID { get; set; }
    public DateTime AddTime { get; set; } = DateTime.UtcNow;

    public int UserID { get; set; }
    public User User { get; set; } = null!;

    public int AdID { get; set; }
    public Advertisement Advertisement { get; set; } = null!;
}

public class ActivityLog
{
    public int LogID { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Result { get; set; } = "Success";

    public int? UserID { get; set; }
    public User? User { get; set; }
}

public class ModerationRule
{
    public int RuleID { get; set; }
    public string Phrase { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
