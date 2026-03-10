---
name: .NET MAUI - Local Chat Client with Agents
description: AI Travel Planner using on-device Apple Intelligence with the Microsoft Agent Framework, Microsoft.Extensions.AI, and .NET MAUI.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: local-chat-client-with-agents
---

# LocalChatClientWithAgents (MAUI + Apple Intelligence + Agent Framework)

A .NET MAUI sample that demonstrates a multi-agent AI Travel Planner powered entirely by on-device Apple Intelligence. It uses the Microsoft Agent Framework to orchestrate a pipeline of specialized agents — from parsing user intent, to RAG-based destination matching, to streaming itinerary generation with tool calling, to conditional translation — all running locally on iOS and macCatalyst.

<!-- ![travel planner](images/travel_planner.png) -->

## What you'll learn

- How to register and consume `IChatClient` (Microsoft.Extensions.AI) backed by Apple Intelligence in a MAUI app
- How to build a multi-agent workflow using the Microsoft Agent Framework (`Microsoft.Agents.AI`)
- How to use Retrieval-Augmented Generation (RAG) with on-device `NLEmbeddingGenerator` for semantic search
- How to implement streaming JSON deserialization for progressive UI updates
- How to use tool calling with agents to discover points of interest
- How to conditionally route agents (e.g., skip translation when the language is English)

## Prerequisites

- .NET 10 SDK (preview)
- macOS with Xcode 26 beta (for Apple Intelligence / FoundationModels framework)
- An Apple device or simulator running iOS 26+ or macOS 26+ with Apple Intelligence enabled

> **Note:** This sample only runs on iOS and macCatalyst. Android and Windows will throw `PlatformNotSupportedException` at startup.

## Architecture

The app uses a 4-agent pipeline to generate travel itineraries:

```
User Input → [Travel Planner] → [Researcher] → [Itinerary Planner] → [Translator?] → Output
```

| Agent | Purpose | Key Feature |
|-------|---------|-------------|
| **Travel Planner** | Extracts destination, duration, and language from natural language | NLP intent parsing |
| **Researcher** | Matches user's destination against a local landmark database | RAG with NL embeddings |
| **Itinerary Planner** | Generates a multi-day itinerary with real places | Tool calling + streaming JSON |
| **Translator** | Translates the itinerary if a non-English language was requested | Conditional routing |

## How it's wired

- `MauiProgram.cs`: Registers Apple Intelligence as `IChatClient` (keyed as `local-model` and `cloud-model`), plus `NLEmbeddingGenerator` for embeddings. Throws `PlatformNotSupportedException` on non-Apple platforms.
- `AI/ItineraryWorkflowExtensions.cs`: Configures the 4-agent workflow graph with conditional branching for translation.
- `AI/ItineraryWorkflowTools.cs`: Provides RAG search and `findPointsOfInterest` tool for the agents.
- `Services/ItineraryService.cs`: Orchestrates streaming itinerary generation with progressive JSON deserialization.
- `Services/DataService.cs`: Manages a local landmark database with semantic embedding search.
- `Pages/LandmarksPage.xaml`: Browse world landmarks with semantic search.
- `Pages/TripPlanningPage.xaml`: Generate and view AI-powered travel itineraries.

## Run

Build and run on an iOS 26+ simulator/device or macCatalyst:

```bash
dotnet build -t:Run -f net10.0-maccatalyst
```

Or open `LocalChatClientWithAgents.sln` in Visual Studio / VS Code and select an iOS or Mac Catalyst target.

## NuGet feed configuration

This sample requires the .NET 10 preview NuGet feed for `Microsoft.Maui.Essentials.AI`. The included `NuGet.config` adds:

```
https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet10/nuget/v3/index.json
```

## Useful docs and resources

- Microsoft.Extensions.AI overview — [learn.microsoft.com/dotnet/ai/microsoft-extensions-ai](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai)
- Apple Intelligence in .NET MAUI — [learn.microsoft.com/dotnet/maui/platform-integration/communication/apple-intelligence](https://learn.microsoft.com/dotnet/maui/platform-integration/communication/apple-intelligence)
- Microsoft Agent Framework — [github.com/microsoft/agents](https://github.com/microsoft/agents)
- NuGet packages:
  - [Microsoft.Extensions.AI](https://www.nuget.org/packages/Microsoft.Extensions.AI)
  - [Microsoft.Agents.AI](https://www.nuget.org/packages/Microsoft.Agents.AI)
  - [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)

## Notes

- This sample uses **only on-device Apple Intelligence** — no cloud API keys or endpoints are required.
- The landmark database is embedded as JSON in `Resources/Raw/` and searched using Apple's Natural Language embeddings.
- The itinerary is streamed progressively using a zero-allocation JSON deserializer for smooth UI updates.
- Translation is conditional — if the user requests English, the translator agent is skipped entirely.
