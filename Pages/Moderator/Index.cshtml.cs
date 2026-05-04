using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Moderator;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IndexModel(ApplicationDbContext db) { _db = db; }

    public List<Advertisement> PendingAds { get; set; } = new();
    public List<User>          Users      { get; set; } = new();
    public List<Report>        Reports    { get; set; } = new();

    public string Tab { get; set; } = "ads";
    [BindProperty] public string RejectionReason { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string tab = "ads")
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Moderator" && role != "Admin") return Forbid();
        Tab = tab;

        PendingAds = await _db.Advertisements
            .Include(a => a.User)
            .Include(a => a.Category)
            .Include(a => a.Subcategory)
            .Include(a => a.Location)
            .Where(a => a.Status == "Pending")
            .OrderBy(a => a.PublishedDate).ToListAsync();

        Users = await _db.Users.OrderBy(u => u.Name).ToListAsync();

        Reports = await _db.Reports
            .Include(r => r.User)
            .Include(r => r.Advertisement).ThenInclude(a => a.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int adId)
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Moderator" && role != "Admin") return Forbid();
        var ad = await _db.Advertisements.FindAsync(adId);
        if (ad == null) return NotFound();
        if (ad.Status == "Active") { TempData["Error"] = "Уже активно"; return RedirectToPage(); }
        ad.Status = "Active"; ad.RejectionReason = null;
        var modId = HttpContext.Session.GetInt32("UserId");
        _db.ActivityLogs.Add(new ActivityLog { UserID = modId, Action = "Одобрение объявления", Details = $"AdID={adId}: {ad.Title}", Timestamp = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Объявление одобрено";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int adId)
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Moderator" && role != "Admin") return Forbid();
        var ad = await _db.Advertisements.FindAsync(adId);
        if (ad == null) return NotFound();
        ad.Status = "Rejected";
        ad.RejectionReason = string.IsNullOrWhiteSpace(RejectionReason)
            ? "Не соответствует правилам"
            : RejectionReason;
        var modId = HttpContext.Session.GetInt32("UserId");
        _db.ActivityLogs.Add(new ActivityLog { UserID = modId, Action = "Отклонение объявления", Details = $"AdID={adId}: {ad.RejectionReason}", Timestamp = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Объявление отклонено";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleBlockAsync(int userId)
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Moderator" && role != "Admin") return Forbid();
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return NotFound();
        user.Status = user.Status == "Active" ? "Blocked" : "Active";
        var modId = HttpContext.Session.GetInt32("UserId");
        _db.ActivityLogs.Add(new ActivityLog
        {
            UserID = modId,
            Action = $"{(user.Status == "Blocked" ? "Блокировка" : "Разблокировка")} пользователя",
            Details = $"User: {user.Login}",
            Timestamp = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = user.Status == "Blocked"
            ? "Пользователь заблокирован"
            : "Пользователь разблокирован";
        return RedirectToPage(new { tab = "users" });
    }

    public async Task<IActionResult> OnPostReviewReportAsync(int reportId)
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Moderator" && role != "Admin") return Forbid();
        var report = await _db.Reports.FindAsync(reportId);
        if (report != null) { report.IsReviewed = true; await _db.SaveChangesAsync(); }
        TempData["Success"] = "Жалоба помечена как рассмотренная";
        return RedirectToPage(new { tab = "reports" });
    }

    public async Task<IActionResult> OnPostDeleteReportAsync(int reportId)
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Moderator" && role != "Admin") return Forbid();
        var report = await _db.Reports.FindAsync(reportId);
        if (report != null) { _db.Reports.Remove(report); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Жалоба удалена";
        return RedirectToPage(new { tab = "reports" });
    }
}
