using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> _sessionMap = new ConcurrentDictionary<string, string>();
        public async Task SendMessage(string user, string message)
        {
            string connectionId = FindConnectionIdByUserName(user);
            if (string.IsNullOrEmpty(connectionId))
            {
                // 消息暂存到队列
            }
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", base.Context.User.Identity.Name, message);
        }

        public override async Task OnConnectedAsync()
        {
            string connectionId = base.Context.ConnectionId;

            string name = base.Context.User.Identity.Name;
            var v = _sessionMap.AddOrUpdate(name, connectionId, (key, value) =>
            {
                return connectionId;
            });

            await Task.CompletedTask;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            await base.OnDisconnectedAsync(exception);
        }

        private string FindConnectionIdByUserName(string userName)
        {
            _sessionMap.TryGetValue(userName, out string connectionId);

            return connectionId;
        }
    }
}
