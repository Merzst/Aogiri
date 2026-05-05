using System.Collections.Concurrent;
using Aogiri.Data;
using Aogiri.Models;
using Microsoft.AspNetCore.SignalR;

namespace Aogiri.Hubs;

public class ChatHub : Hub
{
    // Потокобезопасный словарь: userId -> connectionId
    private static readonly ConcurrentDictionary<int, string> _connections = new();

    private readonly ApplicationDbContext _db;
    public ChatHub(ApplicationDbContext db) { _db = db; }

    /// <summary>
    /// Регистрация пользователя при подключении.
    /// Уведомляем всех остальных, что пользователь появился онлайн.
    /// </summary>
    public async Task Register(int userId)
    {
        _connections[userId] = Context.ConnectionId;
        await Clients.All.SendAsync("UserOnline", userId);
    }

    /// <summary>
    /// Клиент спрашивает: партнёр сейчас онлайн?
    /// </summary>
    public Task<bool> IsOnline(int userId)
    {
        return Task.FromResult(_connections.ContainsKey(userId));
    }

    /// <summary>
    /// Отправить сообщение и сохранить в БД.
    /// </summary>
    public async Task SendMessage(int senderId, int receiverId, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        var msg = new Message
        {
            SenderID = senderId,
            ReceiverID = receiverId,
            Text = text.Trim(),
            SentDate = DateTime.UtcNow,
            IsRead = false
        };
        _db.Messages.Add(msg);
        await _db.SaveChangesAsync();

        var payload = new
        {
            messageId = msg.MessageID,
            senderId,
            receiverId,
            text = msg.Text,
            sentDate = msg.SentDate.ToString("HH:mm · dd.MM")
        };

        // Пуш получателю если онлайн
        if (_connections.TryGetValue(receiverId, out var receiverConn))
            await Clients.Client(receiverConn).SendAsync("ReceiveMessage", payload);

        // Эхо отправителю
        await Clients.Caller.SendAsync("ReceiveMessage", payload);
    }

    /// <summary>
    /// При отключении удаляем из словаря и уведомляем всех.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        int disconnectedUserId = 0;

        // Ищем по connectionId
        foreach (var kv in _connections)
        {
            if (kv.Value == Context.ConnectionId)
            {
                disconnectedUserId = kv.Key;
                _connections.TryRemove(kv.Key, out _);
                break;
            }
        }

        if (disconnectedUserId != 0)
            await Clients.All.SendAsync("UserOffline", disconnectedUserId);

        await base.OnDisconnectedAsync(exception);
    }
}