using System.Collections.Concurrent;

namespace PalmHill.BlazorChat.Server
{
    public static class ChatCancellation
    {
        public static ConcurrentDictionary<Guid, CancellationTokenSource> CancellationTokens { get; private set; } = new ConcurrentDictionary<Guid, CancellationTokenSource>();
    }
}
