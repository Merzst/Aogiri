using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Ads;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    public EditModel(ApplicationDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    public List<Category> Categories { get; set; } = new();
    public List<Location> Locations { get; set; } = new();
    public Advertisement? Ad { get; set; }

    [BindProperty] public string Title { get; set; } = string.Empty;
    [BindProperty] public string? Description { get; set; }
    [BindProperty] public decimal Price { get; set; }
    [BindProperty] public int CategoryID { get; set; }
    [BindProperty] public int LocationID { get; set; }
    [BindProperty] public IFormFile? Image { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        Ad = await _db.Advertisements.FindAsync(id);
        if (Ad == null || Ad.UserID != uid) return Forbid();
        Title = Ad.Title; Description = Ad.Description; Price = Ad.Price;
        CategoryID = Ad.CategoryID; LocationID = Ad.LocationID;
        Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        Locations = await _db.Locations.OrderBy(l => l.City).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        Ad = await _db.Advertisements.FindAsync(id);
        if (Ad == null || Ad.UserID != uid) return Forbid();

        if (string.IsNullOrWhiteSpace(Title)) { ModelState.AddModelError("", "Заголовок обязателен"); Categories = await _db.Categories.ToListAsync(); Locations = await _db.Locations.ToListAsync(); return Page(); }

        if (Image != null && Image.Length > 0)
        {
            var ext = Path.GetExtension(Image.FileName).ToLower();
            if (new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
            {
                var fileName = $"{Guid.NewGuid()}{ext}";
                var path = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using var fs = System.IO.File.Create(path);
                await Image.CopyToAsync(fs);
                Ad.ImageUrl = $"/uploads/{fileName}";
            }
        }

        Ad.Title = Title; Ad.Description = Description; Ad.Price = Price;
        Ad.CategoryID = CategoryID; Ad.LocationID = LocationID;
        Ad.Status = "Pending"; Ad.RejectionReason = null;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Объявление обновлено и отправлено на модерацию";
        return RedirectToPage("/Account/Cabinet");
    }
}
