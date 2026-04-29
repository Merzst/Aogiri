using Aogiri.Data;
using Aogiri.Models;
using Microsoft.AspNetCore.SignalR;

namespace Aogiri.Hubs;

public class ChatHub : Hub
{
    // userId -> connectionId
    private static readonly Dictionary<int, string> _connections = new();
    private static readonly object _lock = new();

    private readonly ApplicationDbContext _db;
    public ChatHub(ApplicationDbContext db) { _db = db; }

    // Регистрация пользователя при подключении
    public async Task Register(int userId)
    {
        lock (_lock) { _connections[userId] = Context.ConnectionId; }

        // ── ИСПРАВЛЕНИЕ БАГ 1 ──────────────────────────────────
        // Уведомляем всех, кто сейчас открыл чат с этим пользователем,
        // что он появился онлайн
        await Clients.All.SendAsync("UserOnline", userId);
    }

    // Клиент спрашивает: партнёр сейчас онлайн?
    public Task<bool> IsOnline(int userId)
    {
        bool online;
        lock (_lock) { online = _connections.ContainsKey(userId); }
        return Task.FromResult(online);
    }

    // Отправить сообщение
    public async Task SendMessage(int senderId, int receiverId, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var msg = new Message
        {
            SenderID   = senderId,
            ReceiverID = receiverId,
            Text       = text.Trim(),
            SentDate   = DateTime.UtcNow,
            IsRead     = false
        };
        _db.Messages.Add(msg);
        await _db.SaveChangesAsync();

        var payload = new
        {
            messageId  = msg.MessageID,
            senderId,
            receiverId,
            text       = msg.Text,
            sentDate   = msg.SentDate.ToString("HH:mm · dd.MM")
        };

        // Пушим получателю если онлайн
        string? receiverConn;
        lock (_lock) { _connections.TryGetValue(receiverId, out receiverConn); }
        if (receiverConn != null)
            await Clients.Client(receiverConn).SendAsync("ReceiveMessage", payload);

        // Эхо отправителю
        await Clients.Caller.SendAsync("ReceiveMessage", payload);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        int disconnectedUserId = 0;
        lock (_lock)
        {
            var kv = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (kv.Key != 0)
            {
                disconnectedUserId = kv.Key;
                _connections.Remove(kv.Key);
            }
        }

        // ── ИСПРАВЛЕНИЕ БАГ 1 ──────────────────────────────────
        // Уведомляем всех, что пользователь ушёл офлайн
        if (disconnectedUserId != 0)
            await Clients.All.SendAsync("UserOffline", disconnectedUserId);

        await base.OnDisconnectedAsync(exception);
    }
}
