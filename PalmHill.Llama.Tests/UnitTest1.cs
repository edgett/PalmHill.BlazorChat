using PalmHill.Llama.Models;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PalmHill.Llama.Tests
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
            var model1ConfigJson = config["Model1"].ToJsonString();
            var model2ConfigJson = config["Model2"].ToJsonString();

            var testTask = TestLoadUnload(model1ConfigJson, model2ConfigJson);

            testTask.GetAwaiter().GetResult();  
            Assert.IsTrue(testTask.IsCompletedSuccessfully);
        }

        private static async Task TestLoadUnload(string model1ConfigJson, string model2ConfigJson)
        {
            
            // Assuming 'config' is of type IConfiguration
            var modelConfig1 = JsonSerializer.Deserialize<ModelConfig>(model1ConfigJson);
            var modelConfig2 = JsonSerializer.Deserialize<ModelConfig>(model2ConfigJson);


            Assert.IsNotNull(modelConfig1);
            Assert.IsNotNull(modelConfig2);

            var loadTimer = new Stopwatch();

            loadTimer.Start();
            await ModelProvider.LoadModel(modelConfig1);
            loadTimer.Stop();
            Console.WriteLine($"Load time {System.IO.Path.GetFileName(modelConfig1.ModelPath)}: {loadTimer.ElapsedMilliseconds}ms");
            var currentModel = ModelProvider.GetModel();
            Assert.IsNotNull(currentModel);

            await Task.Delay(10000);
            loadTimer.Reset();
            loadTimer.Start();
            await ModelProvider.UnloadModel();
            loadTimer.Stop();
            Console.WriteLine($"Unload time {System.IO.Path.GetFileName(modelConfig1.ModelPath)}: {loadTimer.ElapsedMilliseconds}ms");
            var unloadedModel = ModelProvider.GetModel();
            Assert.IsNull(unloadedModel);

            await Task.Delay(10000);
            loadTimer.Reset();
            loadTimer.Start();
            await ModelProvider.LoadModel(modelConfig2);
            loadTimer.Stop();
            Console.WriteLine($"Load time {System.IO.Path.GetFileName(modelConfig2.ModelPath)}: {loadTimer.ElapsedMilliseconds}ms");
            await Task.Delay(10000);
            var currentModel2 = ModelProvider.GetModel();
            Assert.IsNotNull(currentModel2);
            var unloadedModel2 = ModelProvider.GetModel();
            Assert.IsNull(unloadedModel2);

            Assert.Pass();
        }
    }
}