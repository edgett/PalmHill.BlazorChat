# PlamHill.BlazorChat

## Introduction
PlamHill.BlazorChat is a Blazor WebAssembly chat application. It provides a user-friendly interface for real-time chat communication, with a focus on simplicity and functionality.

## Table of Contents
1. [Introduction](#introduction)
2. [Project Structure](#project-structure)
3. [Features](#features)
4. [Key Components](#key-components)
5. [Key Pages](#key-pages)
6. [Getting Started](#getting-started)
7. [Contributing](#contributing)

## Project Structure
The project is structured into three main directories:

- [`Client`](PlamHill.BlazorChat/Client/README.md): Contains the Blazor WebAssembly client application. This includes the entry point of the application, global using directives, static files, and the Blazor components and pages used in the application.

- [`Components`](PlamHill.BlazorChat/Client/Components/README.md): Contains the Blazor components used in the application. These components handle various functionalities such as chat, chat input, markdown rendering, and chat settings.

- [`Pages`](PlamHill.BlazorChat/Client/Pages/README.md): Contains the Blazor pages of the application. These pages include the main page, error page, and not found page.


## Features

PlamHill.BlazorChat offers a range of features to provide a seamless and interactive chat experience:

- **Real-Time Chat**: Engage in real-time conversations with the help of SignalR, which ensures instant message delivery.

- **Markdown Support**: The [`ModelMarkdown.razor`](PlamHill.BlazorChat/Client/Components/ModelMarkdown.razor#L1-L53) component allows for markdown formatting in chat messages, enhancing readability and user experience.

- **Chat Settings**: Customize your chat experience with adjustable settings such as temperature, max length, top P, frequency penalty, and presence penalty, all managed by the [`ChatSettings.razor`](PlamHill.BlazorChat/Client/Components/ChatSettings.razor#L1-L37) component.

- **Error Handling**: The application gracefully handles errors and displays a user-friendly error page [`Error.razor`](PlamHill.BlazorChat/Client/Pages/Error.razor#L1-L15) when an unhandled error occurs.

- **Responsive Layout**: The application layout is responsive and provides a consistent look and feel across different screen sizes, thanks to the `FluentContainer` and `FluentRow` components from the Fluent UI library.

## Key Components
Here are some of the key components in the application:

- [`Chat.razor`](PlamHill.BlazorChat/Client/Components/Chat.razor#L1-L133): Manages the chat functionality, including establishing a connection with a SignalR hub, listening for messages from the server, and sending messages to the server.

- [`ChatInput.razor`](PlamHill.BlazorChat/Client/Components/ChatInput.razor#L1-L91): Handles the input area for the chat, including a text area for the user to type their message and a send button to send the message.

- [`ModelMarkdown.razor`](PlamHill.BlazorChat/Client/Components/ModelMarkdown.razor#L1-L53): Displays the response from the model in markdown format.

## Key Pages
Here are some of the key pages in the application:

- [`Index.razor`](PlamHill.BlazorChat/Client/Pages/Index.razor#L1-L33): The main page of the application, which includes the `Chat` and `ChatInput` components.

- [`MainLayout.razor`](PlamHill.BlazorChat/Client/MainLayout.razor#L1-L35): The main layout of the application, which includes a header with the application title and a body where the current page is rendered.

- [`Error.razor`](PlamHill.BlazorChat/Client/Pages/Error.razor#L1-L15): Displayed when an unhandled error occurs in the application.

- [`NotFound.razor`](PlamHill.BlazorChat/Client/Pages/NotFound.razor#L1-L15): Displayed when the user navigates to a non-existent route.

## Getting Started
Provide instructions on how to set up the project locally for development and testing purposes.

## Contributing
Provide guidelines on how others can contribute to the project. This could include steps for creating issues, creating pull requests, and the project's code of conduct.