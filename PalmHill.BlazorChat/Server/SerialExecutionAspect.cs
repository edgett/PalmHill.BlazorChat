namespace PalmHill.BlazorChat.Server
{
    using AspectInjector.Broker;
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    [Aspect(Scope.Global)]
    public class SerialExecutionAspect
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

        [Advice(Kind.Around, Targets = Target.Method)]
        public object? Handle(
            [Argument(Source.Name)] string methodName,
            [Argument(Source.Target)] Func<object[], object> method,
            [Argument(Source.Arguments)] object[] args,
            [Argument(Source.Metadata)] MethodBase methodBase)
        {
            var attribute = methodBase.GetCustomAttribute<SerialExecutionAttribute>();
            var key = attribute?.Key ?? string.Empty;
            var semaphore = _semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            semaphore.Wait();
            try
            {
                var result = method(args);
                if (result is Task task)
                {
                    task.GetAwaiter().GetResult(); // Ensure the task completes
                    return task;
                }
                return result;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

}