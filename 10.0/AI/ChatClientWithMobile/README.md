# ChatClientWithMobile (Secure Client-Side AI with Tools)

A .NET solution showing secure client-side AI with Microsoft.Extensions.AI function calling in .NET MAUI, using a minimal Web API for credential distribution. The MAUI client builds an IChatClient and executes tools locally (Weather, Calculator, File Operations, System Info, Timers).

![Client-Side AI Architecture Diagram](images/architecture-diagram.png)

## Architecture Overview

This sample implements a **client-centric AI architecture** with secure credential management:

- **Web API Backend** (`ChatMobile.Api`): Simple credential distribution service
- **MAUI Frontend** (`ChatMobile.Client`): Full AI functionality with Microsoft.Extensions.AI, secure credential storage, and tool execution

## Key Benefits

### Security

- **API keys securely stored on device** - using MAUI SecureStorage for encrypted credential storage
- **Minimal server exposure** - Web API only distributes credentials, no AI processing
- **Client-side control** - All AI interactions happen locally with direct Azure OpenAI communication

### Simplified Architecture

1. User opens MAUI app
2. App checks for stored credentials using SecureStorage
3. If no credentials, user clicks "Setup" → app fetches credentials from Web API once
4. Credentials stored securely on device using MAUI SecureStorage
5. All subsequent AI interactions bypass server - direct MAUI → Azure OpenAI communication
6. AI function calling with Weather, Calculator, File Operations, System Info, and Timer tools executed locally

## Project Structure

```
ChatClientWithMobile/
├── README.md
├── images/
└── src/
    ├── ChatClientWithMobile.sln
    ├── ChatMobile.Api/              # ASP.NET Core Web API (Credential Distribution)
    │   ├── Program.cs               # Minimal API with credential endpoint
    │   ├── Services/
    │   │   ├── ICredentialService.cs
    │   │   └── CredentialService.cs
    │   ├── Models/CredentialResponse.cs
    │   └── appsettings.json         # Server-side credential storage
    └── ChatMobile.Client/           # .NET MAUI App (Full AI Functionality)
        ├── Views/
        │   ├── MainPage.xaml        # Chat interface
        │   └── SetupPage.xaml       # Credential setup UI
        ├── ViewModels/
        │   ├── ChatViewModel.cs     # Chat logic with local AI
        │   └── SetupViewModel.cs    # Credential setup logic
        ├── Services/
        │   ├── IChatService.cs      # Local AI chat service
        │   ├── ChatService.cs
        │   ├── ISecureCredentialService.cs  # Secure storage interface
        │   ├── SecureCredentialService.cs   # MAUI SecureStorage implementation
        │   └── HostingExtensions.cs # DI configuration with IChatClient
   ├── Tools/                   # AI Function Tools (AIFunction subclasses)
   │   ├── WeatherTool.cs       # Geocode-first → weather by lat/lon
   │   ├── CalculatorTool.cs    # Mathematical calculations with percentage support
   │   ├── FileOperationsTool.cs# List directories/files on device (safe paths)
   │   ├── SystemInfoTool.cs    # Battery/device info (platform-appropriate)
   │   └── TimerTool.cs         # Simple one-shot timers with titles
        ├── Converters/              # XAML value converters
        └── Models/                  # Chat and result models
```

## Tool Categories

### Client-Side AI Tools (MAUI)

- WeatherTool (`get_weather`): Geocoding first, then weather by coordinates (OpenWeatherMap)
- CalculatorTool (`calculate`): Expressions, including percentages
- FileOperationsTool (`list_files`): Enumerate directories/files (bounded results)
- SystemInfoTool (`system_info`): Battery level/state, device info
- TimerTool (`set_timer`): One-shot timers with friendly summaries

All Microsoft.Extensions.AI processing and tool execution happens locally in the MAUI client. The Web API is used only once to supply credentials which are then stored securely.

## Prerequisites

- .NET 10 SDK
- Azure OpenAI endpoint + API key
- OpenWeatherMap API key (for weather functionality)
- Visual Studio 2022 or Visual Studio Code with C# Dev Kit

## Setup Instructions

### 1. Web API Configuration (One-time)

Update `appsettings.Development.json` in `ChatMobile.Api`:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-endpoint.openai.azure.com/",
    "ApiKey": "your-api-key-here",
    "Model": "gpt-4o-mini"
  },
  "WeatherApiKey": "your-openweather-api-key-here"
}
```

### 2. MAUI Client Setup

The MAUI app will automatically retrieve and store credentials securely:

1. Launch the app
2. Navigate to the "Setup" tab
3. Click "Load from Server" to fetch credentials from the Web API
4. Credentials are stored securely using MAUI SecureStorage
5. Return to "Chat" tab to start using AI features

Notes

- The client’s HttpClient for setup targets `http://127.0.0.1:5132/` (configured in `MauiProgram.cs`).
- The chat input uses Syncfusion `SfTextInputLayout` with a trailing send icon (`IconSend`).

### 3. Alternative: User Secrets (Development)

For local development, configure the Web API using .NET User Secrets:

```powershell
cd src/ChatMobile.Api
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://your-endpoint.openai.azure.com/"
dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key-here"
dotnet user-secrets set "WeatherApiKey" "your-openweather-api-key-here"
```

## Running the Sample

### Option 1: Visual Studio

1. Open `ChatClientWithMobile.sln`
2. Set both projects as startup projects:
   - Right-click solution → Properties → Startup Project → Multiple startup projects
   - Set both `ChatMobile.Api` and `ChatMobile.Client` to "Start"
3. Press F5 to run both projects

### Option 2: Command Line

Terminal 1 (Web API):

```powershell
cd src/ChatMobile.Api
dotnet run
```

Terminal 2 (MAUI - Windows):

```powershell
cd src/ChatMobile.Client
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

The Web API runs on `http://127.0.0.1:5132/` and MAUI connects to fetch credentials once.

## Sample Prompts

Try these prompts to see AI function calling in action:

### Weather Tool

- "What's the weather in Seattle, Washington?"
- "Check the weather in Tokyo and give me the temperature in Celsius"
- "Is it raining in London right now?"

### Calculator Tool

- "Calculate 15% tip on $47.50"
- "What's 25 * 18 + 150?"
- "Calculate 30% of 250"
- "What is the square root of 144?"

### File Operations (Mobile)

- "List the files in my Documents folder"
- "Show the top 10 largest files in Downloads"

### System Info (Mobile)

- "Show me current battery level and state"
- "What device am I running on?"

### Timer (Mobile)

- "Set a 5-minute timer for my coffee break"
- "Set a 20-minute focus timer"

### General AI Chat

- "Explain how weather forecasting works"
- "What's the best way to calculate compound interest?"
- "Help me plan a trip considering the weather"

## API Endpoints

### Web API (`ChatMobile.Api`)

- **GET** `/api/credentials`
  - Returns credentials for Azure OpenAI and weather services
  - Used once by MAUI app during initial setup
  - Response: `{ "AzureOpenAI": {...}, "WeatherApiKey": "..." }`

## Security Architecture

### Credential Flow

```text
Initial Setup:
MAUI App → Web API `/credentials` → Secure Storage (encrypted)

Ongoing Usage:  
MAUI App → Secure Storage → Direct Azure OpenAI → AI Response
```

### Security Features

✅ **Server credentials isolated** - Web API holds master credentials  
✅ **Client credentials encrypted** - MAUI SecureStorage provides platform-native encryption  
✅ **Minimal server interaction** - Credentials fetched once, stored securely  
✅ **Direct AI communication** - No server intermediation for AI requests  
✅ **Tool execution security** - All function calling happens in controlled client environment

### Production Considerations

- Use Azure Key Vault for Web API credential storage
- Implement authentication for credential distribution endpoint
- Consider credential refresh mechanisms for long-lived apps
- Add certificate pinning for enhanced security
- Implement proper logging without exposing secrets

## Troubleshooting

### Common Issues

1. **"No credentials configured"**
   - Navigate to Setup tab and click "Load from Server"
   - Ensure Web API is running and accessible
   - Check Web API configuration has valid credentials

2. **"Failed to connect to server"**
   - Verify Web API is running on expected port (5132)
   - Check MAUI HttpClient configuration in MauiProgram.cs
   - Try using IP address (127.0.0.1) instead of localhost

3. **Weather tool returns mock data**
   - Expected when WeatherApiKey is not configured
   - Add valid OpenWeatherMap API key to Web API configuration

4. **AI responses fail**
   - Verify Azure OpenAI endpoint and API key are correct
   - Check network connectivity for direct Azure OpenAI access
   - Review application logs for specific error messages

## Architecture Highlights

### Client-Centric Design Benefits

- **Reduced Server Load**: No AI processing on server
- **Better Performance**: Direct client-to-Azure communication
- **Enhanced Privacy**: User conversations never traverse your server
- **Simplified Deployment**: Minimal Web API requirements
- **Offline Capability**: Once configured, works without constant server connection

### Microsoft.Extensions.AI Integration

```csharp
// Build the chat client from Azure OpenAI and enable function invocation
var client = new ChatClientBuilder(aoaiClient.GetChatClient(model))
   .UseFunctionInvocation()
   .Build();

// Tools are AIFunction subclasses and are provided per request via ChatOptions.Tools
var tools = new AIFunction[] { weather, calculator, fileOps, systemInfo, timer };
var response = await client.RespondAsync(messages, new ChatOptions { Tools = tools });
```

### Technology Stack

- **AI**: Microsoft.Extensions.AI with Azure OpenAI
- **Security**: MAUI SecureStorage (platform-native encryption)
- **Backend**: Minimal ASP.NET Core Web API (.NET 10)
- **Frontend**: .NET MAUI with CommunityToolkit.Mvvm
- **UI Framework**: Syncfusion.Maui.Toolkit
- **Communication**: Direct Azure OpenAI + minimal HTTP for setup

### UI Highlights

- Input: Syncfusion `SfTextInputLayout` (Outlined) with a trailing send `ImageButton` using `IconSend`
- Resources: `IconSend` defined as `FontImageSource` (Fluent System Icons) in app styles

## Related Samples

- **ChatClientWithTools**: Single-app version with environment variable configuration
- **SimpleChatClient**: Basic chat without tools for comparison

---

🔧 **Architecture**: Client-side AI with secure credential distribution  
🔒 **Security**: Platform-native encrypted credential storage  
📱 **Platform**: Cross-platform MAUI with Windows focus  
🤖 **AI**: Microsoft.Extensions.AI function calling with weather and calculator tools  
⚡ **Performance**: Direct client-to-Azure OpenAI communication
