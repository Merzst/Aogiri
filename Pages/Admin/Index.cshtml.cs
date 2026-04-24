using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Admin;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IndexModel(ApplicationDbContext db) { _db = db; }

    public List<Category> Categories { get; set; } = new();
    public List<ModerationRule> Rules { get; set; } = new();
    public List<ActivityLog> Logs { get; set; } = new();
    public string Tab { get; set; } = "categories";
    public int TotalUsers { get; set; }
    public int TotalAds { get; set; }
    public int ActiveAds { get; set; }
    public int PendingAds { get; set; }

    [BindProperty] public string CategoryName { get; set; } = string.Empty;
    [BindProperty] public string CategoryIcon { get; set; } = "bi-tag";
    [BindProperty] public string RulePhrase { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string tab = "categories")
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        Tab = tab;
        Categories = await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        Rules = await _db.ModerationRules.OrderBy(r => r.Phrase).ToListAsync();
        Logs = await _db.ActivityLogs.Include(l => l.User).OrderByDescending(l => l.Timestamp).Take(100).ToListAsync();
        TotalUsers = await _db.Users.CountAsync();
        TotalAds = await _db.Advertisements.CountAsync();
        ActiveAds = await _db.Advertisements.CountAsync(a => a.Status == "Active");
        PendingAds = await _db.Advertisements.CountAsync(a => a.Status == "Pending");
        return Page();
    }

    public async Task<IActionResult> OnPostAddCategoryAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        if (string.IsNullOrWhiteSpace(CategoryName)) { TempData["Error"] = "Введите название"; return RedirectToPage(new { tab = "categories" }); }
        if (await _db.Categories.AnyAsync(c => c.Name == CategoryName)) { TempData["Error"] = "Категория с таким названием уже существует"; return RedirectToPage(new { tab = "categories" }); }
        _db.Categories.Add(new Category { Name = CategoryName, IconClass = CategoryIcon });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Категория добавлена";
        return RedirectToPage(new { tab = "categories" });
    }

    public async Task<IActionResult> OnPostDeleteCategoryAsync(int catId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        var cat = await _db.Categories.FindAsync(catId);
        if (cat == null) return NotFound();
        var hasAds = await _db.Advertisements.AnyAsync(a => a.CategoryID == catId && a.Status == "Active");
        if (hasAds) { TempData["Error"] = "Нельзя удалить категорию с активными объявлениями"; return RedirectToPage(new { tab = "categories" }); }
        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Категория удалена";
        return RedirectToPage(new { tab = "categories" });
    }

    public async Task<IActionResult> OnPostAddRuleAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        if (string.IsNullOrWhiteSpace(RulePhrase)) { TempData["Error"] = "Введите фразу"; return RedirectToPage(new { tab = "rules" }); }
        _db.ModerationRules.Add(new ModerationRule { Phrase = RulePhrase, CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Правило добавлено";
        return RedirectToPage(new { tab = "rules" });
    }

    public async Task<IActionResult> OnPostDeleteRuleAsync(int ruleId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        var rule = await _db.ModerationRules.FindAsync(ruleId);
        if (rule != null) { _db.ModerationRules.Remove(rule); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Правило удалено";
        return RedirectToPage(new { tab = "rules" });
    }

    public async Task<IActionResult> OnPostDeleteLogAsync(int logId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        var log = await _db.ActivityLogs.FindAsync(logId);
        if (log != null) { _db.ActivityLogs.Remove(log); await _db.SaveChangesAsync(); }
        return RedirectToPage(new { tab = "logs" });
    }

    public async Task<IActionResult> OnPostClearLogsAsync()
    {
        if (HttpContext.Session.GetString("UserRole") != "Admin") return Forbid();
        _db.ActivityLogs.RemoveRange(_db.ActivityLogs);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Логи очищены";
        return RedirectToPage(new { tab = "logs" });
    }
}
