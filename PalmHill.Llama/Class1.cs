using LLama.Common;
using LLama;

namespace PalmHill.Llama
{
    public class Class1
    {
        public static async Task DemoChat()
        {
         
            string modelPath = @"C:\Users\localadmin\Downloads\orca-2-13b.Q6_K.gguf"; // change it to your own model path
            var prompt = "Transcript of a dialog, where the User interacts with an Assistant named Bob. Bob is helpful, kind, honest, good at writing, and never fails to answer the User's requests immediately and with precision.\r\n\r\nUser: Hello, Bob.\r\nBob: Hello. How may I help you today?\r\nUser: Please tell me the largest city in Europe.\r\nBob: Sure. The largest city in Europe is Moscow, the capital of Russia.\r\nUser:"; // use the "chat-with-bob" prompt here.

            // Load a model
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                Seed = 1337,
                GpuLayerCount = 5
            };
            using var model = LLamaWeights.LoadFromFile(parameters);

            // Initialize a chat session
            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);
            ChatSession session = new ChatSession(ex);

            // show the prompt
            Console.WriteLine();
            Console.Write(prompt);

            // run the inference in a loop to chat with LLM
            while (prompt != "stop")
            {
                await foreach (var text in session.ChatAsync(prompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
                {
                    Console.Write(text);
                }
                prompt = Console.ReadLine();
            }

            // save the session
            session.SaveSession("SavedSessionPath");
        }
    }
}
