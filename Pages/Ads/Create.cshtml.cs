using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Ads;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment  _env;
    public CreateModel(ApplicationDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    public List<Category> Categories { get; set; } = new();
    public List<Location> Locations  { get; set; } = new();

    [BindProperty] public string       Title       { get; set; } = string.Empty;
    [BindProperty] public string?      Description { get; set; }
    [BindProperty] public decimal      Price       { get; set; }
    [BindProperty] public int          CategoryID  { get; set; }
    [BindProperty] public int          LocationID  { get; set; }

    // ── ИЗМЕНЕНО: несколько файлов ───────────────────────────
    [BindProperty] public List<IFormFile>? Images { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Account/Login");
        await LoadLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(Title))
        { ModelState.AddModelError("", "Заголовок обязателен"); await LoadLists(); return Page(); }
        if (Price < 0)
        { ModelState.AddModelError("", "Цена не может быть отрицательной"); await LoadLists(); return Page(); }

        // Проверка правил модерации
        var rules  = await _db.ModerationRules.ToListAsync();
        var status = "Pending";
        string? reason = null;
        foreach (var r in rules)
        {
            if ((Title + " " + Description).Contains(r.Phrase, StringComparison.OrdinalIgnoreCase))
            { status = "Rejected"; reason = $"Содержит запрещённую фразу: «{r.Phrase}»"; break; }
        }

        var ad = new Advertisement
        {
            Title           = Title,
            Description     = Description,
            Price           = Price,
            CategoryID      = CategoryID,
            LocationID      = LocationID,
            UserID          = uid.Value,
            Status          = status,
            RejectionReason = reason,
            PublishedDate   = DateTime.UtcNow,
            ExpiryDate      = DateTime.UtcNow.AddDays(30)
        };
        _db.Advertisements.Add(ad);
        await _db.SaveChangesAsync();   // нужен AdID

        // ── Сохраняем фото ───────────────────────────────────
        if (Images != null && Images.Count > 0)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            int order   = 0;
            foreach (var img in Images.Take(10))            // не более 10
            {
                if (img.Length == 0) continue;
                var ext = Path.GetExtension(img.FileName).ToLower();
                if (!allowed.Contains(ext)) continue;

                var fileName = $"{Guid.NewGuid()}{ext}";
                var path     = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using var fs = System.IO.File.Create(path);
                await img.CopyToAsync(fs);
                var url = $"/uploads/{fileName}";

                _db.AdImages.Add(new AdImage { AdID = ad.AdID, ImageUrl = url, SortOrder = order });

                if (order == 0) ad.ImageUrl = url;  // обложка для листинга
                order++;
            }
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = status == "Pending"
            ? "Объявление отправлено на модерацию!"
            : $"Объявление отклонено: {reason}";
        return RedirectToPage("/Account/Cabinet");
    }

    private async Task LoadLists()
    {
        Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        Locations  = await _db.Locations.OrderBy(l => l.City).ToListAsync();
    }
}
