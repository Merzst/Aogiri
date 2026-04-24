using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Ads;

public class ReportModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public ReportModel(ApplicationDbContext db) { _db = db; }

    public Advertisement? Ad { get; set; }
    [BindProperty] public string Reason { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        Ad = await _db.Advertisements
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.AdID == id);

        if (Ad == null) return NotFound();
        if (Ad.UserID == uid)
        { TempData["Error"] = "Нельзя пожаловаться на собственное объявление"; return RedirectToPage("/Ads/Detail", new { id }); }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(Reason))
        { TempData["Error"] = "Укажите причину жалобы"; return RedirectToPage(new { id }); }

        Ad = await _db.Advertisements.FindAsync(id);
        if (Ad == null) return NotFound();
        if (Ad.UserID == uid)
        { TempData["Error"] = "Нельзя пожаловаться на собственное объявление"; return RedirectToPage("/Ads/Detail", new { id }); }

        // Проверяем: пользователь уже жаловался на это объявление?
        var alreadyReported = await _db.Reports
            .AnyAsync(r => r.AdID == id && r.UserID == uid);
        if (alreadyReported)
        { TempData["Error"] = "Вы уже подавали жалобу на это объявление"; return RedirectToPage("/Ads/Detail", new { id }); }

        _db.Reports.Add(new Report
        {
            AdID      = id,
            UserID    = uid.Value,
            Reason    = Reason,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Жалоба отправлена модератору. Спасибо!";
        return RedirectToPage("/Ads/Detail", new { id });
    }
}
