using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PalmHill.Llama;
using PalmHill.Llama.Models;

namespace PalmHill.BlazorChat.Server.WebApi
{
    [Route("api/model")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly ModelProvider _modelProvider;
        private readonly IConfiguration _configuration;

        public ModelController(
            ModelProvider modelProvider,
            IConfiguration configuration
            )
        {
            _modelProvider = modelProvider;
            _configuration = configuration;
        }

        [HttpGet("current", Name =(nameof(GetCurrentModel)))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InjectedModel))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
        public ActionResult<InjectedModel?> GetCurrentModel()
        {
            var model = _modelProvider.GetModel();

            if (_modelProvider.ModelSwapLock.CurrentCount == 0)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Model operation in progress.");
            }

            if (model is null)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return model;
        }

        [HttpPost("load", Name = nameof(LoadModel))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<ActionResult<InjectedModel>> LoadModel([FromBody] ModelConfig? modelConfig)
        {
            if (_modelProvider.ModelSwapLock.CurrentCount == 0)
            { 
                return StatusCode(StatusCodes.Status409Conflict, "Model operation in progress.");
            }

            //Attempt to load model from config file if no model config is provided.
            if (modelConfig is null)
            {
                modelConfig = _configuration.GetModelConfigFromConfigSection("InferenceModelConfig");
            }

            if (modelConfig is null)
            { 
                return StatusCode(StatusCodes.Status400BadRequest, "Model config is null.");
            }

            var currentModel = await _modelProvider.LoadModel(modelConfig);
            return Ok(currentModel);
        }

        [HttpPost("unload", Name = nameof(UnloadModel))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(string))]
        public async Task<ActionResult> UnloadModel()
        {
            if (_modelProvider.ModelSwapLock.CurrentCount == 0)
            {
                return StatusCode(StatusCodes.Status409Conflict, "Model operation in progress.");
            }

            await _modelProvider.UnloadModel();
            return Ok();
        }

    }
}
