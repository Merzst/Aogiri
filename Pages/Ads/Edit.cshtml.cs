using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Ads;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment  _env;
    public EditModel(ApplicationDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    public List<Category> Categories     { get; set; } = new();
    public List<Location> Locations      { get; set; } = new();
    public List<AdImage>  ExistingImages { get; set; } = new();
    public Advertisement? Ad             { get; set; }

    [BindProperty] public string  Title       { get; set; } = string.Empty;
    [BindProperty] public string? Description { get; set; }
    [BindProperty] public decimal Price       { get; set; }
    [BindProperty] public int     CategoryID  { get; set; }
    [BindProperty] public int     LocationID  { get; set; }

    [BindProperty] public List<IFormFile>? NewImages    { get; set; }
    [BindProperty] public List<int>?       DeleteImages { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        Ad = await _db.Advertisements
            .Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.AdID == id);
        if (Ad == null || Ad.UserID != uid) return Forbid();

        Title = Ad.Title; Description = Ad.Description; Price = Ad.Price;
        CategoryID = Ad.CategoryID; LocationID = Ad.LocationID;
        ExistingImages = Ad.Images.OrderBy(i => i.SortOrder).ToList();

        Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        Locations  = await _db.Locations.OrderBy(l => l.City).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        Ad = await _db.Advertisements
            .Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.AdID == id);
        if (Ad == null || Ad.UserID != uid) return Forbid();

        if (string.IsNullOrWhiteSpace(Title))
        {
            ModelState.AddModelError("", "Заголовок обязателен");
            Categories     = await _db.Categories.ToListAsync();
            Locations      = await _db.Locations.ToListAsync();
            ExistingImages = Ad.Images.OrderBy(i => i.SortOrder).ToList();
            return Page();
        }

        // ── ИСПРАВЛЕНИЕ БАГ 3 ──────────────────────────────────
        // Собираем ID фото, помеченных на удаление (могут быть null)
        var toDelete = DeleteImages ?? new List<int>();

        // Удаляем только помеченные явно
        foreach (var imgId in toDelete)
        {
            var img = Ad.Images.FirstOrDefault(i => i.AdImageID == imgId);
            if (img == null) continue;
            DeleteFile(img.ImageUrl);
            _db.AdImages.Remove(img);
        }

        // Добавляем новые фото — вычисляем maxOrder только по тем,
        // которые НЕ помечены на удаление (читаем из БД напрямую)
        if (NewImages != null && NewImages.Count > 0)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

            // Берём максимальный порядок среди оставшихся фото
            int maxOrder = await _db.AdImages
                .Where(i => i.AdID == id && !toDelete.Contains(i.AdImageID))
                .Select(i => (int?)i.SortOrder)
                .MaxAsync() ?? -1;

            foreach (var file in NewImages.Take(10))
            {
                if (file.Length == 0) continue;
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (!allowed.Contains(ext)) continue;

                var fileName = $"{Guid.NewGuid()}{ext}";
                var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using var fs = System.IO.File.Create(path);
                await file.CopyToAsync(fs);

                _db.AdImages.Add(new AdImage
                {
                    AdID      = id,
                    ImageUrl  = $"/uploads/{fileName}",
                    SortOrder = ++maxOrder
                });
            }
        }

        // Обновляем основные поля
        Ad.Title           = Title;
        Ad.Description     = Description;
        Ad.Price           = Price;
        Ad.CategoryID      = CategoryID;
        Ad.LocationID      = LocationID;
        Ad.Status          = "Pending";
        Ad.RejectionReason = null;

        await _db.SaveChangesAsync();

        // Обновляем обложку для листинга — первое оставшееся фото
        var coverImg = await _db.AdImages
            .Where(i => i.AdID == id)
            .OrderBy(i => i.SortOrder)
            .FirstOrDefaultAsync();

        Ad.ImageUrl = coverImg?.ImageUrl;   // null если фото не осталось
        await _db.SaveChangesAsync();

        TempData["Success"] = "Объявление обновлено и отправлено на модерацию";
        return RedirectToPage("/Account/Cabinet");
    }

    private void DeleteFile(string? url)
    {
        if (string.IsNullOrEmpty(url) || !url.StartsWith("/uploads/")) return;
        var path = Path.Combine(_env.WebRootPath,
            url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
    }
}
