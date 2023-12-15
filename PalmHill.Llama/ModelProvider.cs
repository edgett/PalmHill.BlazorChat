using Microsoft.Extensions.DependencyInjection;
using PalmHill.Llama.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.Llama
{
    public static class ModelProvider
    {
        private static SemaphoreSlim _modelSwapLock { get; set; } = new SemaphoreSlim(1, 1);
        private static IServiceCollection _modelServiceCollection { get; } = new ServiceCollection();
        private static ServiceProvider _modelServiceProvider { get => _modelServiceCollection.BuildServiceProvider(); }

        public static async Task LoadModel(ModelConfig modelConfig)
        { 
            await _modelSwapLock.WaitAsync();
            //await UnloadModel();
            await Task.Run(() => { 
                try
                {
                    _modelServiceCollection.AddLlamaModel(modelConfig);
                    _modelSwapLock.Release();
                }
                catch (Exception e)
                {
                    _modelSwapLock.Release();
                    throw new InvalidOperationException($"Failed to load {modelConfig.ModelPath}. See inner exception.", e);
                }
            });
        }

        public static InjectedModel? GetModel()
        {
            var injectedModel = _modelServiceProvider.GetService<InjectedModel>();
            return injectedModel;
        }

        public static async Task UnloadModel()
        {
            await _modelSwapLock.WaitAsync();
            await Task.Run(() => _modelServiceCollection.Clear());
            _modelSwapLock.Release();
        }



      
    }
}
