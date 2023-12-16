# PalmHill.BlazorChat.ApiClient

This project is a .NET 8.0 library that provides an API client for the PalmHill Blazor Chat application. It uses the Refit library for HTTP communication.

## Dependencies
- .NET 8.0
- Refit 7.0.0

## Project References
- PalmHill.BlazorChat.Shared

## API Interface
The API client provides an interface to interact with the chat API. The interface is defined in the `IChat` interface.

### Methods
- `Chat(InferenceRequest conversation)`: This method sends a chat message to the server and returns the server's response as a string. The chat message is encapsulated in an `InferenceRequest` object.
- `Ask(InferenceRequest chatConversation)`: This method sends a chat message to the server and returns the server's response as a `ChatMessage` object. The chat message is encapsulated in an `InferenceRequest` object.
- `CancelChat(Guid conversationId)`: This method sends a request to the server to cancel a chat conversation. The ID of the conversation to be cancelled is passed as a parameter.

### BlazorChatApi.cs
The `BlazorChatApi.cs` file contains the `BlazorChatApi` class, which is the main entry point for using the API client. It provides a convenient way to access the interface and its methods.

The `BlazorChatApi` class has a constructor that takes a `HttpClient` object. This `HttpClient` object is used by the Refit library to make HTTP requests.

The `BlazorChatApi` implments the interfaces by using Refit.

## Usage
To use this library, add a reference to it in your project and create an instance of the `IChat` interface using Refit's `RestService` class. Then, you can call the methods defined in the `IChat` interface.

```csharp
var httpClient = new HttpClient { BaseAddress = new Uri("https://api.example.com") };
var blazorChatApi = new BlazorChatApi(httpClient);
var response = await blazorChatApi.Chat.Chat(new InferenceRequest { /* ... */ });
```

Note: This library is not yet available on NuGet.


## To Do
- [ ] Publish to NuGet
- [ ] Add SignalR Chat suppport
- [x] Add support for model swapping APIs

