using Microsoft.AspNetCore.SignalR;

namespace chatApi.Hubs
{
    public class ChatDSM : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
