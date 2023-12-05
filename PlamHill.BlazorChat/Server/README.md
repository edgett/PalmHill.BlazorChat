# BlazorChat Server

This is the server-side part of the BlazorChat application. It is responsible for handling chat API requests and real-time chat communication via WebSockets.

## Directory Structure

- **Properties**: Contains the launch settings for the application.
- **Program.cs**: The main entry point for the server application. It configures and runs the server application, sets up services like Swagger, SignalR, and response compression, and maps the WebSocket URI.
- **ApiChat.cs**: Handles chat API requests. It uses a LLamaWeights model and ModelParams to process chat conversations and perform inference.
- **WebSocketChat.cs**: Handles real-time chat communication via WebSockets. It uses a LLamaWeights model and ModelParams to process chat conversations and perform inference.

## Key Files

- **launchSettings.json**: Contains the settings for launching the application in different environments. It includes profiles for running the application with HTTP, HTTPS, and IIS Express. You can configure the application for different environments using this file.
- **Program.cs**: Configures and runs the server application. It sets up services like Swagger, SignalR, and response compression, and maps the WebSocket URI. To run the server application, use the `dotnet run` command in the Server directory.
- **ApiChat.cs**: Defines the `ApiChat` class which handles chat API requests. It uses a LLamaWeights model and ModelParams to process chat conversations and perform inference. The server exposes a chat API endpoint at `api/chat` which accepts POST requests with a `ChatConversation` object in the request body and returns a string response from the chat model inference.
- **WebSocketChat.cs**: Defines the `WebSocketChat` class which handles real-time chat communication via WebSockets. It uses a LLamaWeights model and ModelParams to process chat conversations and perform inference. The server exposes a WebSocket endpoint at `/chathub` for real-time chat communication.

## Running the Server

To run the server, navigate to the Server directory and use the `dotnet run` command. The server will start and listen for incoming connections.

## Dependencies

The server-side application depends on several packages and projects. These dependencies are defined in the `PlamHill.BlazorChat.Server.csproj` file. They include:

- Microsoft.AspNetCore.Components.WebAssembly.Server
- Swashbuckle.AspNetCore
- PalmHill.Llama
- PlamHill.BlazorChat.Client
- PlamHill.BlazorChat.Shared
