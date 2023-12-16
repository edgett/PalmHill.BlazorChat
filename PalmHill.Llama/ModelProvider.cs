using Microsoft.Extensions.DependencyInjection;
using PalmHill.Llama.Models;

namespace PalmHill.Llama
{
    public class ModelProvider
    {
        public SemaphoreSlim ModelSwapLock { get; private set; } = new SemaphoreSlim(1, 1);
        private IServiceCollection _modelServiceCollection = new ServiceCollection();
        private ServiceProvider? _modelServiceProvider;
        private bool _isServiceProviderInitialized = false;

        private void InitializeServiceProvider()
        {
            if (!_isServiceProviderInitialized)
            {
                _modelServiceProvider = _modelServiceCollection.BuildServiceProvider();
                _isServiceProviderInitialized = true;
            }

        }

        public async Task<InjectedModel?> LoadModel(ModelConfig modelConfig)
        {
            await UnloadModel();
            await Task.Run(async () =>
            {
                try
                {
                    await ModelSwapLock.WaitAsync();
                    _modelServiceCollection.AddLlamaModel(modelConfig);
                    InitializeServiceProvider();
                    ModelSwapLock.Release();
                }
                catch (Exception e)
                {
                    ModelSwapLock.Release();
                    throw new InvalidOperationException($"Failed to load {modelConfig.ModelPath}. See inner exception.", e);
                }
            });

            return _modelServiceProvider?.GetService<InjectedModel>();
        }

        public InjectedModel? GetModel()
        {
            InitializeServiceProvider();
            ModelSwapLock.Wait();
            var model = _modelServiceProvider?.GetService<InjectedModel>();
            ModelSwapLock.Release();
            return model;
        }

        public async Task UnloadModel()
        {
            await ModelSwapLock.WaitAsync();
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

            ModelSwapLock.Release();
        }
    }
}
