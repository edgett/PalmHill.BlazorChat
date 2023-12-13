using Microsoft.AspNetCore.SignalR;

namespace PalmHill.BlazorChat.Server.SignalR
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// Used to create a fake for now, but will be used to get the user id from the request.
        /// </summary>
        /// <param name="connection">The hub connection.</param>
        /// <returns></returns>
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
