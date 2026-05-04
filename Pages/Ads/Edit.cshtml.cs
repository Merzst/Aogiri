using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;
using Aogiri.Helpers;

namespace Aogiri.Pages.Ads;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment  _env;
    public EditModel(ApplicationDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    public List<Category>    Categories     { get; set; } = new();
    public List<Subcategory> Subcategories  { get; set; } = new();
    public List<Location>    Locations      { get; set; } = new();
    public List<AdImage>     ExistingImages { get; set; } = new();
    public Advertisement?    Ad             { get; set; }

    /// <summary>JSON-описание динамических полей для JS</summary>
    public string CategoryFieldsJson { get; set; } = "{}";

    [BindProperty] public string  Title         { get; set; } = string.Empty;
    [BindProperty] public string? Description   { get; set; }
    [BindProperty] public decimal Price         { get; set; }
    [BindProperty] public int     CategoryID    { get; set; }
    [BindProperty] public int?    SubcategoryID { get; set; }
    [BindProperty] public int     LocationID    { get; set; }
    [BindProperty] public string? Condition     { get; set; }
    [BindProperty] public string? DealType      { get; set; }

    [BindProperty] public List<IFormFile>? NewImages    { get; set; }
    [BindProperty] public List<int>?       DeleteImages { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        Ad = await _db.Advertisements
            .Include(a => a.Images)
            .Include(a => a.Attributes)
            .FirstOrDefaultAsync(a => a.AdID == id);
        if (Ad == null || Ad.UserID != uid) return Forbid();

        Title         = Ad.Title;
        Description   = Ad.Description;
        Price         = Ad.Price;
        CategoryID    = Ad.CategoryID;
        SubcategoryID = Ad.SubcategoryID;
        LocationID    = Ad.LocationID;
        Condition     = Ad.Condition;
        DealType      = Ad.DealType;

        ExistingImages = Ad.Images.OrderBy(i => i.SortOrder).ToList();
        await LoadLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        Ad = await _db.Advertisements
            .Include(a => a.Images)
            .Include(a => a.Attributes)
            .FirstOrDefaultAsync(a => a.AdID == id);
        if (Ad == null || Ad.UserID != uid) return Forbid();

        if (string.IsNullOrWhiteSpace(Title))
        {
            ModelState.AddModelError("", "Заголовок обязателен");
            ExistingImages = Ad.Images.OrderBy(i => i.SortOrder).ToList();
            await LoadLists();
            return Page();
        }

        // Удаляем помеченные фото
        var toDelete = DeleteImages ?? new List<int>();
        foreach (var imgId in toDelete)
        {
            var img = Ad.Images.FirstOrDefault(i => i.AdImageID == imgId);
            if (img == null) continue;
            DeleteFile(img.ImageUrl);
            _db.AdImages.Remove(img);
        }

        // Удаляем старые атрибуты и пересохраняем
        _db.AdAttributes.RemoveRange(Ad.Attributes);

        // Добавляем новые фото
        if (NewImages != null && NewImages.Count > 0)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
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
        Ad.SubcategoryID   = SubcategoryID;
        Ad.LocationID      = LocationID;
        Ad.Condition       = Condition;
        Ad.DealType        = DealType;
        Ad.Status          = "Pending";
        Ad.RejectionReason = null;

        await _db.SaveChangesAsync();

        // Сохраняем динамические атрибуты
        var fields = CategoryFields.GetFields(CategoryID);
        foreach (var field in fields)
        {
            var val = Request.Form[$"attr_{field.Key}"].ToString().Trim();
            if (!string.IsNullOrEmpty(val))
            {
                _db.AdAttributes.Add(new AdAttribute
                {
                    AdID  = id,
                    Key   = field.Key,
                    Value = val
                });
            }
        }

        // Обновляем обложку — первое оставшееся фото
        var coverImg = await _db.AdImages
            .Where(i => i.AdID == id)
            .OrderBy(i => i.SortOrder)
            .FirstOrDefaultAsync();

        Ad.ImageUrl = coverImg?.ImageUrl;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Объявление обновлено и отправлено на модерацию";
        return RedirectToPage("/Account/Cabinet");
    }

    private async Task LoadLists()
    {
        Categories    = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        Subcategories = await _db.Subcategories.Include(s => s.Category).OrderBy(s => s.Name).ToListAsync();
        Locations     = await _db.Locations.OrderBy(l => l.City).ToListAsync();
        CategoryFieldsJson = CategoryFields.ToJson();
    }

    private void DeleteFile(string? url)
    {
        if (string.IsNullOrEmpty(url) || !url.StartsWith("/uploads/")) return;
        var path = Path.Combine(_env.WebRootPath,
            url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
    }
}
