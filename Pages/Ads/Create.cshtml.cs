using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Ads;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    public CreateModel(ApplicationDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    public List<Category> Categories { get; set; } = new();
    public List<Location> Locations { get; set; } = new();

    [BindProperty] public string Title { get; set; } = string.Empty;
    [BindProperty] public string? Description { get; set; }
    [BindProperty] public decimal Price { get; set; }
    [BindProperty] public int CategoryID { get; set; }
    [BindProperty] public int LocationID { get; set; }
    [BindProperty] public IFormFile? Image { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null) return RedirectToPage("/Account/Login");
        await LoadLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(Title)) { ModelState.AddModelError("", "Заголовок обязателен"); await LoadLists(); return Page(); }
        if (Price < 0) { ModelState.AddModelError("", "Цена не может быть отрицательной"); await LoadLists(); return Page(); }

        // Check moderation rules
        var rules = await _db.ModerationRules.ToListAsync();
        var status = "Pending";
        string? reason = null;
        foreach (var r in rules)
        {
            if ((Title + " " + Description).Contains(r.Phrase, StringComparison.OrdinalIgnoreCase))
            { status = "Rejected"; reason = $"Содержит запрещённую фразу: «{r.Phrase}»"; break; }
        }

        string? imageUrl = null;
        if (Image != null && Image.Length > 0)
        {
            var ext = Path.GetExtension(Image.FileName).ToLower();
            if (new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
            {
                var fileName = $"{Guid.NewGuid()}{ext}";
                var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using var fs = System.IO.File.Create(path);
                await Image.CopyToAsync(fs);
                imageUrl = $"/uploads/{fileName}";
            }
        }

        var ad = new Advertisement
        {
            Title = Title, Description = Description, Price = Price,
            CategoryID = CategoryID, LocationID = LocationID, UserID = uid.Value,
            Status = status, RejectionReason = reason, ImageUrl = imageUrl,
            PublishedDate = DateTime.UtcNow, ExpiryDate = DateTime.UtcNow.AddDays(30)
        };
        _db.Advertisements.Add(ad);
        await _db.SaveChangesAsync();

        TempData["Success"] = status == "Pending" ? "Объявление отправлено на модерацию!" : $"Объявление отклонено: {reason}";
        return RedirectToPage("/Account/Cabinet");
    }

    private async Task LoadLists()
    {
        Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        Locations = await _db.Locations.OrderBy(l => l.City).ToListAsync();
    }
}
