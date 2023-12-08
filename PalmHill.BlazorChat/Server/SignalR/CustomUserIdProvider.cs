using Microsoft.AspNetCore.SignalR;

namespace PalmHill.BlazorChat.Server.SignalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();

            if (httpContext == null)
            {
                return "";
            }

            if (httpContext.Request.Query.ContainsKey("customUserId"))
            { 
                return httpContext.Request.Query["customUserId"].SingleOrDefault() ?? "";
            }

            return "";
        }
    }
}
