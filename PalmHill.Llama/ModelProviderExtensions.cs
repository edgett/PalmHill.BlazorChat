using LLama.Common;
using LLama;
using Microsoft.Extensions.Hosting;
using PalmHill.Llama.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace PalmHill.Llama
{
    public static class ModelProviderExtensions
    {
        const string DEFAULT_MODEL_CONFIG_SECTION = "InferenceModelConfig";
       
        /// <summary>
        /// Add Llama to the service collection.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="modelConfig"></param>
        /// <exception cref="FileNotFoundException">Occurs when the model file is missing.</exception>
        /// <exception cref="ArgumentNullException">Occurs when the <see cref="ModelConfig"/> is null and can not be retreived from appsettings.json.</exception>"
        public static InjectedModel AddLlamaModel(this IServiceCollection serviceCollection, ModelConfig? modelConfig = null)
        {
            if (modelConfig == null)
            {
                throw new ArgumentNullException(nameof(modelConfig), $"The argument {modelConfig} must be supplied if there is no {DEFAULT_MODEL_CONFIG_SECTION} section in app configuartion.");
            }

            //check if model is present
            var modelExsists = System.IO.File.Exists(modelConfig.ModelPath);
            if (!modelExsists)
            {
                throw new FileNotFoundException($"Model file does not exsist.", modelConfig.ModelPath);
            }

            //Initlize Llama
            ModelParams parameters = new ModelParams(modelConfig.ModelPath ?? "")
            {
                ContextSize = modelConfig.ContextSize,
                GpuLayerCount = modelConfig.GpuLayerCount,
                MainGpu = modelConfig.Gpu
            };

            LLamaWeights model = LLamaWeights.LoadFromFile(parameters);
            //End Initlize Llama

            var injectedModel = new InjectedModel(model, parameters, modelConfig.AntiPrompts);

            //Add to services
            serviceCollection?.AddSingleton(injectedModel);

            return injectedModel;
        }

        public static InjectedModel AddLlamaModel(this IServiceCollection serviceCollection, IConfigurationManager configuration)
        {
            //Attemt to get model config from config
            var modelConfig = configuration.GetModelConfigFromConfigSection(DEFAULT_MODEL_CONFIG_SECTION);

            var injectedModel = serviceCollection.AddLlamaModel(modelConfig);

            return injectedModel;
        }



        public static ModelConfig? GetModelConfigFromConfigSection(this IConfigurationManager configuration, string configSection)
        {
            var appConfig = configuration?.GetSection(configSection);
            var appSettingsConfig = appConfig?.Get<ModelConfig>();
            appSettingsConfig!.AntiPrompts = appConfig?.GetSection("AntiPrompts").Get<List<string>>() ?? [];

            return appSettingsConfig;
        }
    }
}
