using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Ads;

public class DetailModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public DetailModel(ApplicationDbContext db) { _db = db; }

    public Advertisement? Ad          { get; set; }
    public bool           IsFavorited { get; set; }
    public bool           IsOwner     { get; set; }
    public List<AdImage>  Images      { get; set; } = new();

    [BindProperty] public string MessageText { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Ad = await _db.Advertisements
            .Include(a => a.User)
            .Include(a => a.Category)
            .Include(a => a.Subcategory)
            .Include(a => a.Location)
            .Include(a => a.Images)
            .Include(a => a.Attributes)
            .FirstOrDefaultAsync(a => a.AdID == id);

        if (Ad == null) return NotFound();

        Images = Ad.Images.OrderBy(i => i.SortOrder).ToList();

        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid.HasValue)
        {
            IsFavorited = await _db.Favorites.AnyAsync(f => f.UserID == uid && f.AdID == id);
            IsOwner     = Ad.UserID == uid;
        }

        if (!IsOwner) { Ad.ViewCount++; await _db.SaveChangesAsync(); }
        return Page();
    }

    public async Task<IActionResult> OnPostSendMessageAsync(int id)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        if (string.IsNullOrWhiteSpace(MessageText))
        { TempData["Error"] = "Введите текст сообщения"; return RedirectToPage(new { id }); }

        Ad = await _db.Advertisements.FindAsync(id);
        if (Ad == null) return NotFound();
        if (Ad.UserID == uid)
        { TempData["Error"] = "Нельзя отправить сообщение самому себе"; return RedirectToPage(new { id }); }

        _db.Messages.Add(new Message
        {
            SenderID   = uid.Value,
            ReceiverID = Ad.UserID,
            AdID       = id,
            Text       = MessageText,
            SentDate   = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Сообщение отправлено!";
        return RedirectToPage("/Messages/Index");
    }
}
