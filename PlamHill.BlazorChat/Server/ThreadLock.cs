namespace PlamHill.BlazorChat.Server
{
    public static class ThreadLock
    {
        public static SemaphoreSlim InferenceLock = new SemaphoreSlim(1, 1);
    }
}
