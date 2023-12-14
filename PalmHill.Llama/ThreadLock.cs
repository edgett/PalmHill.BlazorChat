namespace PalmHill.Llama
{
    public static class ThreadLock
    {
        public static SemaphoreSlim InferenceLock { get; private set; } = new SemaphoreSlim(1, 1);

    }
}
