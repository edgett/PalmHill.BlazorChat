using Microsoft.Extensions.DependencyInjection;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.Llama.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PalmHill.LlmMemory.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {

            var configJson = File.ReadAllText("testsettings.json");
            var config = JsonObject.Parse(configJson);
            Assert.IsNotNull(config);
            var model1ConfigJson = config["Model1"]?.ToJsonString();
            var model2ConfigJson = config["Model2"]?.ToJsonString();
            var modelConfig1 = JsonSerializer.Deserialize<ModelConfig>(model1ConfigJson ?? "");
            var modelConfig2 = JsonSerializer.Deserialize<ModelConfig>(model2ConfigJson ?? "");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLlmMemory(modelConfig1, modelConfig2);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var llmMemory = serviceProvider.GetService<ServerlessLlmMemory>();

            Assert.IsNotNull(llmMemory);

            var attachmentInfoToAttach = new AttachmentInfo()
            {
                Name = "constitution.pdf",
                ContentType = "application/pdf",
                FileBytes = File.ReadAllBytes("constitution-rag_test.pdf"),
                Status = AttachmentStatus.Pending,
                ConversationId = Guid.NewGuid()
            };

            attachmentInfoToAttach.Size = attachmentInfoToAttach.FileBytes.Length;


            var attachedDoc = llmMemory.ImportDocumentAsync(attachmentInfoToAttach).GetAwaiter().GetResult();

            Assert.IsNotNull(attachedDoc);

            Assert.That(attachedDoc.Status == AttachmentStatus.Uploaded);
            Assert.That(attachedDoc.ConversationId == attachmentInfoToAttach.ConversationId);

            var askResult = llmMemory.Ask(attachmentInfoToAttach.ConversationId.Value, "Free speech").GetAwaiter().GetResult();

            Assert.IsNotNull(askResult);

            Assert.Pass();
        }
    }
}