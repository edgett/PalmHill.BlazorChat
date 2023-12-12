using System.Collections.Concurrent;

namespace PalmHill.BlazorChat.Server
{
    public static class ChatCancelation
    {
        public static ConcurrentDictionary<Guid, CancellationTokenSource> CancelationTokens = new ConcurrentDictionary<Guid, CancellationTokenSource>();
    }
}
