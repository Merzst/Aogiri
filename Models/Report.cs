namespace Aogiri.Models;

public class Report
{
    public int ReportID { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsReviewed { get; set; } = false;

    public int AdID { get; set; }
    public Advertisement Advertisement { get; set; } = null!;

    public int UserID { get; set; }
    public User User { get; set; } = null!;
}
