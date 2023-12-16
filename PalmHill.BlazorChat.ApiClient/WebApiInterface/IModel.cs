using PalmHill.BlazorChat.ApiClient.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.ApiClient.WebApiInterface
{
    public interface IModel
    {
        // Corresponds to GetCurrentModel method in ModelController
        [Get("/api/model/current")]
        Task<ApiResponse<InjectedModel?>> GetCurrentModel();

        // Corresponds to LoadModel method in ModelController
        [Post("/api/model/load")]
        Task<ApiResponse<InjectedModel>> LoadModel([Body] ModelConfig? modelConfig);

        // Corresponds to UnloadModel method in ModelController
        [Post("/api/model/unload")]
        Task<IApiResponse> UnloadModel();
    }
}
