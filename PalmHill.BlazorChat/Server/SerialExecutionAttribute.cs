namespace PalmHill.BlazorChat.Server
{
    using AspectInjector.Broker;

    [AttributeUsage(AttributeTargets.Method)]
    [Injection(typeof(SerialExecutionAspect))]
    public class SerialExecutionAttribute : Attribute
    {
        public string Key { get; }

        public SerialExecutionAttribute(string key)
        {
            Key = key;
        }
    }


}
