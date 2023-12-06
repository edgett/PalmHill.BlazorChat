# PlamHill.BlazorChat Client Application

## Introduction
The Client folder contains the Blazor WebAssembly client application for the PlamHill.BlazorChat project. This application is responsible for the user interface and client-side logic of the chat application.

# Table of Contents
1. [Introduction](#introduction)
2. [Project Structure](#project-structure)
    - [Program.cs](#programcs)
    - [_Imports.razor](#importsrazor)
    - [wwwroot](#wwwroot)
    - [Components](#components)
    - [Pages](#pages)
3. [Key Components](#key-components)
    - [Chat.razor](#chatrazor)
    - [ChatInput.razor](#chatinputrazor)
    - [MarkdownSection.razor](#markdownsectionrazorcs)

## Project Structure
Here's an overview of the main files and folders in the Client folder:

- `Program.cs`: The entry point of the Blazor application.
- `_Imports.razor`: Contains global using directives for the Blazor application.
- `wwwroot`: Contains static files for the Blazor application.
- `Components`: Contains the Blazor components used in the application.
- `Pages`: Contains the Blazor pages of the application.

## Key Components
Here are some of the key components in the Components folder:

- `Chat.razor`: This is the main chat component of the application. It establishes a connection with a SignalR hub, listens for incoming messages from the server, and sends messages to the server when a user sends a chat message. The chat messages are displayed in a list, with each message being represented by a ModelResponse object.
- `ChatInput.razor`:  This component handles the input area for the chat. It includes a text area for the user to type their message and a send button to send the message. It also has a button to show the chat settings. The ChatInput component is attached to the Chat component after the first render, allowing it to send messages to the chat.
- `MarkdownSection.razor`: This component is responsible for converting Markdown content to HTML for rendering in the Blazor application. It is used in the Chat component to display the chat messages in a user-friendly format.
- `ModelMarkdown.razor`This component is used to display the response from the model in markdown format. It takes a ModelResponse object as a parameter and listens for changes in the response. When the response changes, it updates the markdown content to be rendered.
## Key Pages
Here's a key page in the Pages folder:


- `Index.razor`: This is the main page of the application. It includes the Chat and ChatInput components, providing the main interface for the chat application. The Chat component is responsible for displaying the chat messages, and the ChatInput component is used for entering and sending new messages.
- `MainLayout.razor`: This is the main layout of the application. It includes a header with the application title and a body where the current page is rendered. The layout is defined using the FluentContainer and FluentRow components from the Fluent UI library, providing a consistent look and feel across the application.
