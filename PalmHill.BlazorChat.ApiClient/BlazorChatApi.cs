﻿using PalmHill.BlazorChat.ApiClient.WebApiInterface;
using Refit;

namespace PalmHill.BlazorChat.ApiClient
{
    public class BlazorChatApi
    {
        public BlazorChatApi(HttpClient httpClient)
        {
            HttpClient = httpClient;
            Attachment = RestService.For<IAttachment>(httpClient);
            Chat = RestService.For<IChat>(httpClient);
        }

        public HttpClient HttpClient { get; }
        public IAttachment Attachment { get; }
        public IChat Chat { get; }
    }
}
