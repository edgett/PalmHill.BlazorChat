# PalmHill.BlazorChat

[<img src="https://github.com/edgett/PalmHill.BlazorChat/raw/main/Documentation/settings-light-mode.png" alt="Click to watch the video" height="360">](https://github.com/edgett/PalmHill.BlazorChat/raw/main/Documentation/palmhill-chat-demo.mp4)


## Table of Contents
1. [Introduction](#introduction)
2. [Features](#features)
3. [Project Structure](#project-structure)
4. [Getting Started](#getting-started)
5. [Contributing](#contributing)


# Introduction
## Welcome to the Alpha Release of Our Blazor WebAssembly / SignalR / WebAPI Chat Application!

🚀 **Alpha Release: Expanding Horizons**  
We're excited to announce the alpha release of our chat application built entirely in C#! This software showcases the foundational features and sets the stage for more exciting updates to come. Your feedback and contributions during this phase are invaluable as we strive to enhance and refine the application.

🌐 **All C# Magic**  
This application is entirely written in C#. This choice reflects our commitment to robust, efficient, and scalable solutions. We've harnessed the power of C# to bring you an application that's not just functional but also a joy to interact with.

🤖 **ChatGPT's Legacy, Reimagined**  
Inspired by the original capabilities of ChatGPT at its launch, our application integrates all those pioneering features. We've built upon this foundation to offer a seamless and engaging chat experience that mirrors the sophistication and versatility of ChatGPT.

📱 **Mobile-Ready and Responsive**  
This application provides a mobile-ready design that adapts flawlessly across various devices.

🧠 **Choose Your Language Model - Powered by Llama 2**  
Flexibility to select your preferred Llama 2 based large language model. Explore different models and discover the unique strengths of each one.

🔜 **Stay Tuned: More to Come!**  
The journey doesn't end here! We're constantly working to bring new features and improvements. Keep an eye out for updates as we evolve and grow. Your suggestions and feedback will shape the future of this application.

💡 **Your Input Matters**  
Join us in refining and enhancing this application. Try it out, push its limits, and let us know what you think. Your insights are crucial in this alpha phase and will guide us in creating an application that truly resonates with its users.

🌟 **Get Involved**  
Excited about what you see? Star us on GitHub and share with your network. Every star, fork, and pull request brings us closer to our goal of creating an outstanding chat application.

Thank you for being a part of our journey. Let's make chatting smarter and more fun together!


## Features

PalmHill.BlazorChat offers a range of features to provide a seamless and interactive chat experience:

- **Real-Time Chat**: Engage in real-time conversations with the help of SignalR, which ensures instant message delivery.

- **Markdown Support**: The [`ModelMarkdown.razor`](PalmHill.BlazorChat/Client/Components/ModelMarkdown.razor#L1-L53) component allows for markdown formatting in chat messages, enhancing readability and user experience.

- **Chat Settings**: Customize your chat experience with adjustable settings such as temperature, max length, top P, frequency penalty, and presence penalty, all managed by the [`ChatSettings.razor`](PalmHill.BlazorChat/Client/Components/ChatSettings.razor#L1-L37) component.

- **Error Handling**: The application gracefully handles errors and displays a user-friendly error page [`Error.razor`](PalmHill.BlazorChat/Client/Pages/Error.razor#L1-L15) when an unhandled error occurs.

- **Responsive Layout**: The application layout is responsive and provides a consistent look and feel across different screen sizes, thanks to the `FluentContainer` and `FluentRow` components from the Fluent UI library.

## Project Structure
The project is structured into three main directories:

- [Client](PalmHill.BlazorChat/Client/README.md): Contains the Blazor WebAssembly user interface.

- [Server](PalmHill.BlazorChat/Server/README.md): Contains the server-side websocket/SignalR and REST/WebAPI logic for the application. You can set the model file location in this project in appsettings.json. 

- [Llama](PalmHill.Llama/README.md): This is a wrapper for [LlamaSharp](https://github.com/SciSharp/LLamaSharp). Contains the code related to the Llama models.

 
## Getting Started

To get started with PalmHill.BlazorChat, follow these simple steps to set up the project on your local machine for development and testing:

### Prerequisites
Before you begin, ensure you have the following installed on your system:
- **.NET 8**: This application requires the .NET 8 framework. You can download it from [the official .NET website](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- **CUDA 12**: Currently CUDA 12 is required. Download and install CUDA 12 from [NVIDIA's CUDA Toolkit page](https://developer.nvidia.com/cuda-12.0-download-archive).


### Step 1: Download and Place the Language Model
1. **Download the Language Model:** First, you'll need to download the appropriate Llama 2 language model for the application. Any GGUF/Llama2 model should work and can be downloaded from Hugginface. We reccomend selecting a model that will fit your VRAM and RAM from [this list](https://huggingface.co/TheBloke/Orca-2-13B-GGUF).

    For testing [TheBloke/Orca-2-13B-GGUF](https://huggingface.co/TheBloke/Orca-2-13B-GGUF/blob/main/orca-2-13b.Q6_K.gguf) was used and requires at least 13gb VRAM.

2. **Place the Model:** Once downloaded, place the model file in a designated directory on your system. Remember the path of this directory, as you'll need it for configuring the application.

### Step 2: Clone the Repository
1. **Clone the Repository:** Use Git to clone the PalmHill.BlazorChat repository to your local machine. Alternatively, you can open the project directly from GitHub using Visual Studio. 
   - To clone, use the command: `git clone [https://github.com/edgett/PalmHill.BlazorChat.git]`.
   - If using Visual Studio, select 'Clone a repository' and enter the repository URL.

### Step 3: Configure the Startup Project
1. **Open the Solution in Visual Studio:** Once the repository is cloned, open the solution file in Visual Studio.
2. **Set the Startup Project:** In the Solution Explorer, right-click on the `Server` project and select 'Set as StartUp Project'. This ensures that the server-side logic is the entry point of the application.

### Step 4: Set the Model Path
1. **Locate `appsettings.json`**: In the root of `Server` project.
2. **Set the Model Path:** Edit `appsettings.json` to include the path to the language model file you placed in step 1. Use double backslashes (`\\`) for the file path to escape the backslashes properly in JSON syntax.

    Example:
    ```json
    "ModelPath": "C:\\path\\to\\your\\model\\model-file-name"
    ```

### Step 5: Run the Application
1. **Build the Solution:** Build the solution in Visual Studio to ensure all dependencies are correctly resolved.
2. **Run the Application:** Click 'Play' in Visual Studio to start the application. This will launch the application in your default web browser.
3. **Application URLs:** The application will open to the URLs specified in Blazorchat/Server/`launchSettings.json`, typically `https://localhost:7233` and `http://localhost:5222`. You can access the application via either of these URLs in your browser.

### Troubleshooting and Tips
- Ensure that all necessary NuGet packages are installed and up to date.
- Check the .NET and Blazor WebAssembly versions required for the project.
- If you encounter issues, refer to the 'FAQs' section or reach out to the community for support.

By following these steps, you should be able to set up and run PalmHill.BlazorChat on your local machine for development and testing. Happy coding!


## Contributing
Open a pull request and go nuts!
