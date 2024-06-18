# PalmHill.Llama

PalmHill.Llama is a .NET library designed to integrate with the LLama machine learning framework. It provides essential utilities and models to enhance the capabilities of LLama-based applications, particularly in the realm of AI and data processing.

## Features

- **Llama Extensions**: Enhancements to the LLama context for improved functionality and usability.
- **Thread Locking**: Utilizes `SemaphoreSlim` for effective thread management in concurrent environments.
- **Model Injection**: Facilitates the injection of pre-trained LLama models with configurable parameters.
- **Configuration Management**: Offers robust options to configure the LLama models and their operational parameters.

## Dependencies

- .NET 8.0
- LLamaSharp 0.8.1
- LLamaSharp.Backend.Cuda12 0.8.1
- Microsoft.Extensions.* various libraries

## Installation

Include the PalmHill.Llama library in your .NET project using the provided `.csproj` file. NuGet coming soon.

## Usage

### Llama Extensions

Utilize the `LlamaExtensions` class to add enhanced capabilities to your LLama context.

### Thread Management

Use the `ThreadLock` class to manage concurrency in your applications, ensuring safe and efficient processing.

### Model Injection

Create instances of `InjectedModel` to work with pre-trained LLama models. Configure using `ModelConfig`.

### Configuration

Configure your models and operational parameters with the `ModelConfig` class, allowing for flexible and robust application setups.

## To Do

- [ ] Add NuGet package
- [ ] Add unit tests
- [ ] Support for Model Switching



