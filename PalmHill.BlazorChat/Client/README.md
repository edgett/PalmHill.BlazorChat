# PlamHill.BlazorChat Client Application

## Introduction
The Client folder contains the Blazor WebAssembly client application for the PlamHill.BlazorChat project. This application is responsible for the user interface and client-side logic of the chat application.

## Table of Contents
1. [Project Structure](#project-structure)
2. [Key Components](#key-components)
3. [Key Pages](#key-pages)
4. [Key Services](#key-services)

## Project Structure
Here's an overview of the main files and folders in the Client folder:

- `Program.cs`: The entry point of the Blazor application.
- `wwwroot`: Contains static files for the Blazor application.
- `Pages`: Contains the Blazor pages of the application.
- `Components`: Contains the Blazor components used in the application.
- `Models`: Contains the View Models used by the Components and Pages.
- `Services`: Contains the Services used to operate the UI.

## Key Components
Here are some of the key components in the Components folder:

- `ChatInput.razor`:  This component handles the input area for the chat. It includes a text area for the user to type their message and a send button to send the message.
- `AttachmentManager.razor`: This component is responsible for uploading attachments. It includes the file input, and list of uploaded files.
- `ModelMarkdown.razor`: This component is used to display the response from the inference in markdown format. It takes a ModelResponse object as a parameter and listens for changes in the response. When the response changes, it updates the markdown content to be rendered.
- `Settings.razor`: This component handles user configurable settings (Theme, Inference, System Message).

## Key Pages
Here's a key page in the Pages folder:

- `Index.razor`: This is the main chat component of the application. It uses the ChatService.cs to implement its functionality.
- `MainLayout.razor`: This is the main layout of the application. It includes a header with the application title and a body where the current page is rendered. The layout is defined using the FluentContainer and FluentRow components from the Fluent UI library, providing a consistent look and feel across the application.

## Key Services
- `ChatService.cs`: The main service provided by Dependency Injection for operating the UI.
- `WebSocketChatService.cs`: The service used by the ChatService to interact with the server via WebSockets.
