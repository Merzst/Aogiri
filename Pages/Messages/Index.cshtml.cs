using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Aogiri.Data;
using Aogiri.Models;

namespace Aogiri.Pages.Messages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public IndexModel(ApplicationDbContext db) { _db = db; }

    public List<ConversationSummary> Conversations { get; set; } = new();
    public List<Message> CurrentMessages { get; set; } = new();
    public User? CurrentPartner { get; set; }
    public int? PartnerID { get; set; }
    [BindProperty] public string ReplyText { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int? partnerId)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        PartnerID = partnerId;

        var msgs = await _db.Messages
            .Include(m => m.Sender).Include(m => m.Receiver).Include(m => m.Advertisement)
            .Where(m => m.SenderID == uid || m.ReceiverID == uid)
            .OrderByDescending(m => m.SentDate).ToListAsync();

        var partnerIds = msgs.Select(m => m.SenderID == uid ? m.ReceiverID : m.SenderID).Distinct();
        Conversations = new List<ConversationSummary>();
        foreach (var pid in partnerIds)
        {
            var last = msgs.First(m => m.SenderID == pid || m.ReceiverID == pid);
            var partner = last.SenderID == uid ? last.Receiver : last.Sender;
            var unread = msgs.Count(m => m.SenderID == pid && m.ReceiverID == uid && !m.IsRead);
            Conversations.Add(new ConversationSummary { Partner = partner, LastMessage = last, UnreadCount = unread });
        }

        if (partnerId.HasValue)
        {
            CurrentPartner = await _db.Users.FindAsync(partnerId);
            CurrentMessages = msgs.Where(m => (m.SenderID == uid && m.ReceiverID == partnerId) || (m.SenderID == partnerId && m.ReceiverID == uid))
                .OrderBy(m => m.SentDate).ToList();
            // Mark as read
            foreach (var m in CurrentMessages.Where(m => m.ReceiverID == uid && !m.IsRead)) m.IsRead = true;
            await _db.SaveChangesAsync();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostReplyAsync(int partnerId)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");
        if (string.IsNullOrWhiteSpace(ReplyText)) { TempData["Error"] = "Введите текст"; return RedirectToPage(new { partnerId }); }

        _db.Messages.Add(new Message { SenderID = uid.Value, ReceiverID = partnerId, Text = ReplyText, SentDate = DateTime.UtcNow });
        await _db.SaveChangesAsync();
        return RedirectToPage(new { partnerId });
    }
}

public class ConversationSummary
{
    public User Partner { get; set; } = null!;
    public Message LastMessage { get; set; } = null!;
    public int UnreadCount { get; set; }
}
