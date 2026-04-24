using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Favorites;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IndexModel(ApplicationDbContext db) { _db = db; }
    public List<Favorite> Favorites { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        Favorites = await _db.Favorites.Include(f => f.Advertisement).ThenInclude(a => a.Category)
            .Include(f => f.Advertisement).ThenInclude(a => a.Location)
            .Where(f => f.UserID == uid).OrderByDescending(f => f.AddTime).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(int favId)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        var fav = await _db.Favorites.FindAsync(favId);
        if (fav != null && fav.UserID == uid) { _db.Favorites.Remove(fav); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Удалено из избранного";
        return RedirectToPage();
    }
}
