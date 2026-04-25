namespace Aogiri.Models;

public class Advertisement
{
    public int      AdID            { get; set; }
    public string   Title           { get; set; } = string.Empty;
    public string?  Description     { get; set; }
    public decimal  Price           { get; set; }
    public DateTime PublishedDate   { get; set; } = DateTime.UtcNow;
    public string   Status          { get; set; } = "Draft";
    public string?  RejectionReason { get; set; }
    public int      ViewCount       { get; set; } = 0;
    public string?  ImageUrl        { get; set; }   // главное фото (для листинга)
    public DateTime? ExpiryDate     { get; set; }

    public int      UserID     { get; set; }
    public User     User       { get; set; } = null!;

    public int      LocationID { get; set; }
    public Location Location   { get; set; } = null!;

    public int      CategoryID { get; set; }
    public Category Category   { get; set; } = null!;

    public ICollection<Message>   Messages  { get; set; } = new List<Message>();
    public ICollection<Favorite>  Favorites { get; set; } = new List<Favorite>();

    // ── НОВОЕ: галерея фотографий ──────────────────────────────
    public ICollection<AdImage>   Images    { get; set; } = new List<AdImage>();
}
