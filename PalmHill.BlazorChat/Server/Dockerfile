# Use an NVIDIA CUDA base image
FROM nvidia/cuda:12.3.1-base-ubuntu22.04 AS base

ARG DEBIAN_FRONTEND=noninteractive
# Install .NET runtime
RUN apt-get update \
#    && apt-get install -y nvidia-kernel-open-545 \
    && apt-get install -y cuda-drivers-545 \
    && apt-get install -y cuda-libraries-12-3 \
    && apt-get install -y wget \
    && wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y dotnet-sdk-8.0 \
    && rm packages-microsoft-prod.deb

USER $APP_UID
WORKDIR /app
EXPOSE 5000

ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PalmHill.BlazorChat/Server/PalmHill.BlazorChat.Server.csproj", "PalmHill.BlazorChat/Server/"]
COPY ["PalmHill.Llama/PalmHill.Llama.csproj", "PalmHill.Llama/"]
COPY ["PalmHill.BlazorChat/Shared/PalmHill.BlazorChat.Shared.csproj", "PalmHill.BlazorChat/Shared/"]
COPY ["PalmHill.LlmMemory/PalmHill.LlmMemory.csproj", "PalmHill.LlmMemory/"]
COPY ["PalmHill.BlazorChat/Client/PalmHill.BlazorChat.Client.csproj", "PalmHill.BlazorChat/Client/"]
COPY ["PalmHill.BlazorChat.ApiClient/PalmHill.BlazorChat.ApiClient.csproj", "PalmHill.BlazorChat.ApiClient/"]
RUN dotnet restore "PalmHill.BlazorChat/Server/PalmHill.BlazorChat.Server.csproj"
COPY . .
WORKDIR "/src/PalmHill.BlazorChat/Server"
RUN dotnet build "PalmHill.BlazorChat.Server.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PalmHill.BlazorChat.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy the contents of /home/localadmin/Documents/Models/ to /app/Models
ENTRYPOINT ["dotnet", "PalmHill.BlazorChat.Server.dll", "--interactive"]
