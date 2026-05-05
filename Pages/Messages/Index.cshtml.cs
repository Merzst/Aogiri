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

        // Загружаем все сообщения пользователя
        var msgs = await _db.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.Advertisement)
            .Where(m => m.SenderID == uid || m.ReceiverID == uid)
            .OrderByDescending(m => m.SentDate)
            .ToListAsync();

        // Строим список диалогов корректно:
        // партнёр — это тот, кто НЕ является текущим пользователем
        var partnerIds = msgs
            .Select(m => m.SenderID == uid ? m.ReceiverID : m.SenderID)
            .Distinct()
            .ToList();

        Conversations = new List<ConversationSummary>();
        foreach (var pid in partnerIds)
        {
            // Последнее сообщение именно этого диалога (с конкретным партнёром)
            var last = msgs.First(m =>
                (m.SenderID == uid && m.ReceiverID == pid) ||
                (m.SenderID == pid && m.ReceiverID == uid));

            var partner = last.SenderID == uid ? last.Receiver : last.Sender;

            var unread = msgs.Count(m =>
                m.SenderID == pid &&
                m.ReceiverID == uid &&
                !m.IsRead);

            Conversations.Add(new ConversationSummary
            {
                Partner = partner,
                LastMessage = last,
                UnreadCount = unread
            });
        }

        // Сортируем диалоги по дате последнего сообщения (новые вверху)
        Conversations = Conversations
            .OrderByDescending(c => c.LastMessage.SentDate)
            .ToList();

        // Открытый диалог
        if (partnerId.HasValue)
        {
            CurrentPartner = await _db.Users.FindAsync(partnerId);
            if (CurrentPartner == null) return RedirectToPage();

            CurrentMessages = msgs
                .Where(m =>
                    (m.SenderID == uid && m.ReceiverID == partnerId) ||
                    (m.SenderID == partnerId && m.ReceiverID == uid))
                .OrderBy(m => m.SentDate)
                .ToList();

            // Помечаем входящие как прочитанные
            var unreadMsgs = CurrentMessages
                .Where(m => m.ReceiverID == uid && !m.IsRead)
                .ToList();

            if (unreadMsgs.Any())
            {
                foreach (var m in unreadMsgs) m.IsRead = true;
                await _db.SaveChangesAsync();
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostReplyAsync(int partnerId)
    {
        var uid = HttpContext.Session.GetInt32("UserId");
        if (uid == null) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(ReplyText))
        {
            TempData["Error"] = "Введите текст сообщения";
            return RedirectToPage(new { partnerId });
        }

        // Проверяем что партнёр существует
        var partner = await _db.Users.FindAsync(partnerId);
        if (partner == null) return NotFound();

        _db.Messages.Add(new Message
        {
            SenderID = uid.Value,
            ReceiverID = partnerId,
            Text = ReplyText.Trim(),
            SentDate = DateTime.UtcNow
        });
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