using Microsoft.AspNetCore.SignalR;

namespace PalmHill.BlazorChat.Server.SignalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Your custom logic to determine the UserId
            // For example, using a query string parameter
            return connection.GetHttpContext().Request.Query["customUserId"];
        }
    }
}
