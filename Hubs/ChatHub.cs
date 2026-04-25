using Aogiri.Data;
using Aogiri.Models;
using Microsoft.AspNetCore.SignalR;

namespace Aogiri.Hubs;

public class ChatHub : Hub
{
    // userId -> connectionId (в памяти, достаточно для одного сервера)
    private static readonly Dictionary<int, string> _connections = new();
    private static readonly object _lock = new();

    private readonly ApplicationDbContext _db;
    public ChatHub(ApplicationDbContext db) { _db = db; }

    // Вызывается клиентом сразу после соединения для привязки userId
    public Task Register(int userId)
    {
        lock (_lock) { _connections[userId] = Context.ConnectionId; }
        return Task.CompletedTask;
    }

    // Отправить сообщение: сохраняем в БД + пушим получателю если онлайн
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

        // Пушим получателю если он онлайн
        string? receiverConn;
        lock (_lock) { _connections.TryGetValue(receiverId, out receiverConn); }
        if (receiverConn != null)
            await Clients.Client(receiverConn).SendAsync("ReceiveMessage", payload);

        // Подтверждение отправителю (эхо в его же окно)
        await Clients.Caller.SendAsync("ReceiveMessage", payload);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        lock (_lock)
        {
            var key = _connections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (key != 0) _connections.Remove(key);
        }
        return base.OnDisconnectedAsync(exception);
    }
}
