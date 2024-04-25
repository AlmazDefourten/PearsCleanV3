using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PearsCleanV3.Web.SignalRHubs;

[Authorize]
public class MessagesHub : Hub
{
    public async Task Send(string userId, string message)
    {
        await this.Clients.User(userId).SendAsync("Receive", message);
    }
}
