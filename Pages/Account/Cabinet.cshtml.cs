using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Account;

public class CabinetModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment  _env;

    public CabinetModel(ApplicationDbContext db, IWebHostEnvironment env)
    { _db = db; _env = env; }

    public User?               CurrentUser  { get; set; }
    public List<Advertisement> MyAds        { get; set; } = new();
    public int ActiveCount  { get; set; }
    public int PendingCount { get; set; }
    public int TotalViews   { get; set; }

    [BindProperty] public string     Name            { get; set; } = string.Empty;
    [BindProperty] public string     Phone           { get; set; } = string.Empty;
    [BindProperty] public string?    Email           { get; set; }
    [BindProperty] public IFormFile? Avatar          { get; set; }
    [BindProperty] public string     OldPassword     { get; set; } = string.Empty;
    [BindProperty] public string     NewPassword     { get; set; } = string.Empty;
    [BindProperty] public string     ConfirmPassword { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        CurrentUser = await _db.Users.FindAsync(uid);
        if (CurrentUser == null) return RedirectToPage("/Account/Login");

        Name  = CurrentUser.Name;
        Phone = CurrentUser.Phone;
        Email = CurrentUser.Email;

        MyAds = await _db.Advertisements
            .Include(a => a.Category).Include(a => a.Location)
            .Where(a => a.UserID == uid && a.Status != "Deleted")
            .OrderByDescending(a => a.PublishedDate).ToListAsync();

        ActiveCount  = MyAds.Count(a => a.Status == "Active");
        PendingCount = MyAds.Count(a => a.Status == "Pending");
        TotalViews   = MyAds.Sum(a => a.ViewCount);
        return Page();
    }

    // ── Обновление профиля ───────────────────────────────────────
    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        var user = await _db.Users.FindAsync(uid);
        if (user == null) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Phone))
        { TempData["Error"] = "Имя и телефон обязательны"; return RedirectToPage(); }

        if (Avatar != null && Avatar.Length > 0)
        {
            var ext = Path.GetExtension(Avatar.FileName).ToLower();
            if (new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" }.Contains(ext))
            {
                if (!string.IsNullOrEmpty(user.AvatarUrl) && user.AvatarUrl.StartsWith("/uploads/"))
                {
                    var old = Path.Combine(_env.WebRootPath,
                        user.AvatarUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
                }
                var fileName = $"avatar_{uid}_{Guid.NewGuid():N}{ext}";
                var savePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using var fs = System.IO.File.Create(savePath);
                await Avatar.CopyToAsync(fs);
                user.AvatarUrl = $"/uploads/{fileName}";
            }
            else
            { TempData["Error"] = "Формат аватарки должен быть JPG, PNG, WEBP или GIF"; return RedirectToPage(); }
        }

        user.Name = Name; user.Phone = Phone; user.Email = Email;
        await _db.SaveChangesAsync();
        HttpContext.Session.SetString("UserName", Name);
        TempData["Success"] = "Профиль обновлён";
        return RedirectToPage();
    }

    // ── Смена пароля ─────────────────────────────────────────────
    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        var user = await _db.Users.FindAsync(uid);
        if (user == null) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(OldPassword) ||
            string.IsNullOrWhiteSpace(NewPassword)  ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        { TempData["Error"] = "Заполните все поля смены пароля"; return RedirectToPage(); }

        if (!BCrypt.Net.BCrypt.Verify(OldPassword, user.PasswordHash))
        { TempData["Error"] = "Текущий пароль введён неверно"; return RedirectToPage(); }

        if (NewPassword.Length < 4)
        { TempData["Error"] = "Новый пароль должен содержать минимум 4 символа"; return RedirectToPage(); }

        if (NewPassword != ConfirmPassword)
        { TempData["Error"] = "Новый пароль и подтверждение не совпадают"; return RedirectToPage(); }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Пароль успешно изменён";
        return RedirectToPage();
    }

    // ── Продление объявления ─────────────────────────────────────
    public async Task<IActionResult> OnPostRenewAdAsync(int adId)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        var ad  = await _db.Advertisements.FindAsync(adId);
        if (ad == null || ad.UserID != uid) return Forbid();

        if (ad.Status != "Active" && ad.Status != "Inactive")
        { TempData["Error"] = "Продлить можно только активное или неактивное объявление"; return RedirectToPage(); }

        ad.ExpiryDate = (ad.ExpiryDate.HasValue && ad.ExpiryDate > DateTime.UtcNow
            ? ad.ExpiryDate.Value : DateTime.UtcNow).AddDays(30);

        if (ad.Status == "Inactive")
        {
            ad.Status = "Pending";
            TempData["Success"] = "Объявление отправлено на повторную модерацию (+30 дней)";
        }
        else
        {
            TempData["Success"] = "Срок публикации продлён на 30 дней";
        }

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    // ── Удаление аватарки ────────────────────────────────────────
    public async Task<IActionResult> OnPostRemoveAvatarAsync()
    {
        var uid  = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        var user = await _db.Users.FindAsync(uid);
        if (user == null) return RedirectToPage("/Account/Login");

        if (!string.IsNullOrEmpty(user.AvatarUrl) && user.AvatarUrl.StartsWith("/uploads/"))
        {
            var old = Path.Combine(_env.WebRootPath,
                user.AvatarUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
        }
        user.AvatarUrl = null;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Аватарка удалена";
        return RedirectToPage();
    }

    // ── Удаление объявления ──────────────────────────────────────
    public async Task<IActionResult> OnPostDeleteAdAsync(int adId)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        var ad  = await _db.Advertisements.FindAsync(adId);
        if (ad == null || ad.UserID != uid) return Forbid();
        ad.Status = "Deleted";
        await _db.SaveChangesAsync();
        TempData["Success"] = "Объявление удалено";
        return RedirectToPage();
    }

    // ── ИСПРАВЛЕНИЕ БАГ 2: деактивация/активация ────────────────
    // Разрешаем переключение ТОЛЬКО между Active и Inactive.
    // Pending, Rejected и другие статусы — не трогаем.
    public async Task<IActionResult> OnPostDeactivateAdAsync(int adId)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        var ad  = await _db.Advertisements.FindAsync(adId);
        if (ad == null || ad.UserID != uid) return Forbid();

        if (ad.Status == "Active")
        {
            ad.Status = "Inactive";
            await _db.SaveChangesAsync();
            TempData["Success"] = "Объявление деактивировано";
        }
        else if (ad.Status == "Inactive")
        {
            // Возвращаем в Active — отправляем на повторную модерацию,
            // чтобы объявление не обходило проверку
            ad.Status = "Pending";
            await _db.SaveChangesAsync();
            TempData["Success"] = "Объявление отправлено на повторную модерацию";
        }
        else
        {
            TempData["Error"] = "Это объявление нельзя активировать напрямую";
        }

        return RedirectToPage();
    }
}
