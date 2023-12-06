# Components

This folder contains the Razor components used in the Blazor Chat application. Here is a brief overview of each component:

## [Chat.razor](PlamHill.BlazorChat/Client/Components/Chat.razor#L1-L133)
This component manages the chat functionality. It establishes a connection with a SignalR hub, listens for messages from the server, and sends messages to the server when a user sends a chat message. The chat messages are displayed in a list, with each message being represented by a `ModelResponse` object.

## [ModelMarkdown.razor](PlamHill.BlazorChat/Client/Components/ModelMarkdown.razor#L1-L53)
This component is used to display the response from the model in markdown format. It takes a `ModelResponse` object as a parameter and listens for changes in the response. When the response changes, it updates the markdown content to be rendered.

## [SettingContainer.razor](PlamHill.BlazorChat/Client/Components/SettingContainer.razor#L1-L53)
This component is a UI element for displaying and adjusting settings. It includes a label, a number field, and a slider. The value of the setting can be adjusted either by typing into the number field or by moving the slider. The component uses generic programming to support any numeric type.

## [ChatInput.razor](PlamHill.BlazorChat/Client/Components/ChatInput.razor#L1-L91)
This component handles the input area for the chat. It includes a text area for the user to type their message and a send button to send the message. It also has a button to show the chat settings.

## [ChatSettings.razor](PlamHill.BlazorChat/Client/Components/ChatSettings.razor#L1-L37)
This component is used to display and adjust the chat settings. It includes fields for various settings such as temperature, max length, top P, frequency penalty, and presence penalty.