#PalmHill.BlazorChat.Shared

Contains models shared between the server and client projects.

## Table of Contents
1. [WebSocketInferenceStatusUpdate.cs](#websocketinferencestatusupdatecs)
2. [WebSocketInferenceString.cs](#websocketinferencestringcs)
3. [AttachmentInfo.cs](#attachmentinfocs)
4. [ChatMessage.cs](#chatmessagecs)
5. [InferenceRequest.cs](#inferencerequestcs)
6. [InferenceSettings.cs](#inferencesettingscs)

---

## WebSocketInferenceStatusUpdate.cs
### Namespace
- `PalmHill.BlazorChat.Shared.Models.WebSocket`

### Class: `WebSocketInferenceStatusUpdate`
#### Summary
- A status update sent over a WebSocket.

#### Properties
- **`MessageId`** (`Guid?`)
  - **Description**: Not documented.
- **`IsComplete`** (`bool`)
  - **Description**: Indicates whether the status update is complete. Default value is `false`.
- **`Success`** (`bool?`)
  - **Description**: Indicates the success status of the update. Not documented.

---

## WebSocketInferenceString.cs
### Namespace
- `PalmHill.BlazorChat.Shared.Models.WebSocket`

### Class: `WebSocketInferenceString`
#### Summary
- An inference string sent over a WebSocket.

#### Properties
- **`WebSocketChatMessageId`** (`Guid`)
  - **Description**: Not explicitly documented. Default value is a new `Guid`.
- **`InferenceString`** (`string`)
  - **Description**: The inference string content. Default value is an empty string.

---

## AttachmentInfo.cs
### Namespace
- `PalmHill.BlazorChat.Shared.Models`

### Class: `AttachmentInfo`
#### Summary
- Information about an attachment.

#### Properties
- **`Id`** (`Guid`)
  - **Summary**: The Id of the attachment.
  - **Description**: Default value is a new `Guid`.
- **`Name`** (`string`)
  - **Summary**: The file name of the attachment.
  - **Description**: Default value is an empty string.
- **`FileBytes`** (`byte[]?`)
  - **Summary**: The file bytes of the attachment. (Not serialized). Used for import/memorization.
  - **Description**: This property is not serialized and is used for internal processing.
- **`ContentType`** (`string`)
  - **Summary**: The content type of the attachment.
  - **Description**: Default value is an empty string.

---

## ChatMessage.cs
### Namespace
- Not explicitly defined in the provided content.

### Class: `ChatMessage`
#### Summary
- Represents a chat message in a conversation.

#### Properties
- **`Id`** (`Guid`)
  - **Description**: Represents the unique identifier of the chat message. Default value is a new `Guid`.
- **`Role`** (`ChatMessageRole?`)
  - **Summary**: Gets or sets the role of the entity that generated the message.
  - **Description**: The role of the entity. Default value is `ChatMessageRole.User`.
- **`Message`** (`string?`)
  - **Summary**: Gets or sets the content of the message.
  - **Description**: The actual message content. Default value is "What are cats?".
- **`AttachmentIds`** (`List<string>`)
  - **Summary**: The user can add attachments to the message, the attachment will become part of the context.
  - **Description**: A list of attachment identifiers. Default value is an empty list.

---

## InferenceRequest.cs
### Namespace
- `PalmHill.BlazorChat.Shared.Models`

### Class: `InferenceRequest`
#### Summary
- Represents a chat conversation.

#### Properties
- **`Id`** (`Guid`)
  - **Description**: Represents the unique identifier of the inference request. Default value is a new `Guid`.
- **`SystemMessage`** (`string`)
  - **Summary**: Gets or sets the system message for the chat conversation.
  - **Description**: The system message content. Default value is "You are a helpful assistant."
- **`ChatMessages`** (`List<ChatMessage>`)
  - **Summary**: Gets or sets the chat messages for the chat conversation.
  - **Description**: A collection of `ChatMessage` objects. Default value is a new list.

---

## InferenceSettings.cs
### Namespace
- `PalmHill.BlazorChat.Shared.Models`

### Class: `InferenceSettings`
#### Summary
- Represents the settings for inference with a large language model.

#### Properties
- **`Temperature`** (`float`)
  - **Summary**: Gets or sets the temperature for the inference. Higher values (closer to 1) make the output more random, while lower values make the output more deterministic.
  - **Description**: The temperature setting for inference. Default
