# ChatClientWithVoice

A minimal .NET MAUI sample that extends the ChatClient pattern with two voice-to-text modes using .NET MAUI Community Toolkit:

- Online: `SpeechToText.Default`
- Offline: `OfflineSpeechToText.Default`

It keeps the same credential-distribution API as the mobile sample and focuses on clarity over abstractions.

## Prerequisites

- .NET 10 SDK + MAUI workload
- Start the API project: `ChatVoice.Api`
- Update `appsettings.Development.json` with your Azure OpenAI endpoint/key and Weather API key (for parity with other samples)

## Platforms permissions

- Android: `RECORD_AUDIO` in `Platforms/Android/AndroidManifest.xml`
- iOS/macOS: `NSSpeechRecognitionUsageDescription` and `NSMicrophoneUsageDescription` in `Platforms/iOS/Info.plist`
- Windows: `microphone` capability in `Platforms/Windows/Package.appxmanifest`

## Run

1. Start `ChatVoice.Api` ([http://127.0.0.1:5132/](http://127.0.0.1:5132/) by default in this sample's client)
2. Deploy `ChatVoice.Client` to your device/emulator
3. Tap “Online” or “Offline” to capture speech, then the result auto-sends

## Notes

- This sample omits tool examples for brevity. Add AIFunction tools to `ChatService` if desired, mirroring other samples.
