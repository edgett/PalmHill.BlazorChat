using PalmHill.Llama.Models;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PalmHill.Llama.Tests
{
    public class Tests
    {
        public static ModelProvider TestModelProvider { get; } = new ModelProvider();

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

            var testTask = testLoadUnload(model1ConfigJson, model2ConfigJson);

            testTask.GetAwaiter().GetResult();  
            Assert.IsTrue(testTask.IsCompletedSuccessfully);
        }

        private static async Task testLoadUnload(string model1ConfigJson, string model2ConfigJson)
        {
            //Get model config from json
            var modelConfig1 = JsonSerializer.Deserialize<ModelConfig>(model1ConfigJson);
            var modelConfig2 = JsonSerializer.Deserialize<ModelConfig>(model2ConfigJson);
            
            //Assert model config is not null
            Assert.IsNotNull(modelConfig1);
            Assert.IsNotNull(modelConfig2);


            var loadTimer = new Stopwatch();
            //Load model 1, then wait 10 seconds
            Console.WriteLine($"Loading {modelConfig1.ModelName}");
            await loadModel(modelConfig1);
            countDown(10, $"{modelConfig1.ModelName} loaded. Unloading model");

            //Unload model 1, then wait 3 seconds
            await unloadModel();
            countDown(3, $"{modelConfig1.ModelName} unloaded. Loading model {modelConfig2.ModelPath}");


            //Load model 2, then wait 3 seconds
            await loadModel(modelConfig2);
            countDown(3, $"{modelConfig2.ModelName} loaded. Loading model {modelConfig2.ModelPath}");

            //Load model 1, without unloading first, then wait 3 seconds
            await loadModel(modelConfig1);
            countDown(3, $"{modelConfig1.ModelName} loaded. Unloading");

            //Unload
            await unloadModel();
        }


        private static async Task loadModel(ModelConfig modelConfig)
        {
            var loadTimer = new Stopwatch();

            loadTimer.Start();
            await TestModelProvider.LoadModel(modelConfig);
            loadTimer.Stop();
            Console.WriteLine($"Load time {modelConfig.ModelName}: {loadTimer.ElapsedMilliseconds}ms");
            var currentModel = TestModelProvider.GetModel();
            var modelJson = currentModel?.ToJson();
            Assert.IsNotEmpty(modelJson);
            Assert.IsNotNull(currentModel);
        }

        private static async Task unloadModel()
        {
            var loadTimer = new Stopwatch();

            loadTimer.Start();
            await TestModelProvider.UnloadModel();
            loadTimer.Stop();
            Console.WriteLine($"Unload time: {loadTimer.ElapsedMilliseconds}ms");
            var unloadedModel = TestModelProvider.GetModel();
            Assert.IsNull(unloadedModel);
        }

        private static void countDown(int seconds, string message)
        {             
            for (int i = seconds; i > 0; i--)
            {
                Console.WriteLine($"{message} in {i} seconds");
                Thread.Sleep(1000);
            }
        }
    }
}