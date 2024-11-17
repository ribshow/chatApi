﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace chatApi.Hubs
{
    [Authorize]
    public class ChatSci : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
