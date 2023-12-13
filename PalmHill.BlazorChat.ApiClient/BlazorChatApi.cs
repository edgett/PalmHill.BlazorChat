using PalmHill.BlazorChat.ApiClient.WebApiInterface;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.ApiClient
{
    public class BlazorChatApi
    {
        public BlazorChatApi(HttpClient httpClient) 
        { 
            HttpClient = httpClient;
            Attachment =  RestService.For<IAttachment>(httpClient);
            Chat = RestService.For<IChat>(httpClient);
        }

        public HttpClient HttpClient { get; }
        public IAttachment Attachment { get; }
        public IChat Chat { get; }
    }
}
