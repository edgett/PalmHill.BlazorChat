using Microsoft.Extensions.DependencyInjection;
using PalmHill.Llama.Models;

namespace PalmHill.Llama
{
    public static class ModelProvider
    {
        private static SemaphoreSlim _modelSwapLock = new SemaphoreSlim(1, 1);
        private static IServiceCollection _modelServiceCollection = new ServiceCollection();
        private static ServiceProvider? _modelServiceProvider;
        private static bool _isServiceProviderInitialized = false;

        private static void InitializeServiceProvider()
        {
            if (!_isServiceProviderInitialized)
            {
                _modelServiceProvider = _modelServiceCollection.BuildServiceProvider();
                _isServiceProviderInitialized = true;
            }
        }

        public static async Task LoadModel(ModelConfig modelConfig)
        {
            await UnloadModel();
            await Task.Run(async () => {
                try
                {
                    await _modelSwapLock.WaitAsync();
                    _modelServiceCollection.AddLlamaModel(modelConfig);
                    InitializeServiceProvider();
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
            InitializeServiceProvider();
            return _modelServiceProvider?.GetService<InjectedModel>();
        }

        public static async Task UnloadModel()
        {
            await _modelSwapLock.WaitAsync();
            await Task.Run(() =>
            {
                var servicesToDispose = _modelServiceProvider?.GetServices<InjectedModel>();

                if (servicesToDispose is null)
                {
                    return;
                }

                foreach (var service in servicesToDispose)
                {
                    if (service is IDisposable disposableService)
                    {
                        disposableService.Dispose();
                    }
                }
                _modelServiceCollection.Clear();
            });
            _modelServiceProvider?.Dispose();
            _modelServiceProvider = null;
            _isServiceProviderInitialized = false;

            _modelSwapLock.Release();
        }
    }
}
