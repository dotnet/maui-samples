---
name: .NET MAUI - Passkeys
description: A native .NET MAUI 11 passkeys client with an ASP.NET Core WebAuthn relying-party server.
page_type: sample
languages:
- csharp
- xaml
- powershell
products:
- dotnet-maui
urlFragment: platformintegration-passkeys
---

# Passkeys

This sample demonstrates passkey registration and passwordless sign-in with the .NET MAUI 11
`Microsoft.Maui.Authentication.Passkeys` API. It includes:

- `src/Passkeys.Client`, a native MAUI app for Android, iOS, Mac Catalyst, and Windows.
- `src/Passkeys.Server`, a minimal ASP.NET Core Identity relying-party (RP) server.
- `Configure-Passkeys.ps1`, deterministic dev tunnel and platform-association setup.

The server uses an in-memory SQLite database. Accounts and passkeys are sample data and disappear when
the server stops.

## Security boundary

The native client does **not** validate WebAuthn attestation or assertion responses. It requests
creation/request JSON from the RP server, gives that JSON to the platform authenticator, and returns the
authenticator response. The ASP.NET Core server validates the challenge, RP ID, origin, attestation, and
assertion before it stores a credential or establishes an authenticated session.

This is a development sample, not a production identity service. It intentionally disables email
confirmation, uses an in-memory database, and accepts only origins configured in user-secrets.

## Prerequisites

- The .NET 11 Preview 7 SDK from [`11.0/global.json`](../../global.json).
- The MAUI Android, iOS, and Mac Catalyst workloads needed by your host.
- [PowerShell 7](https://learn.microsoft.com/powershell/scripting/install/installing-powershell).
- [Dev tunnels CLI](https://learn.microsoft.com/azure/developer/dev-tunnels/get-started):
  - macOS: `brew install --cask devtunnel`
  - Windows: `winget install Microsoft.devtunnel`
- A JDK `keytool` for Android fingerprint discovery.
- Platform requirements listed below.

If the Preview 7 SDK is not installed, use the repository-local workflow from
[`11.0/README.md`](../../README.md). From `11.0/`:

```bash
curl -fsSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 11.0.100-preview.7.26381.103 --install-dir ./.dotnet
```

The local `.dotnet/` directory is ignored by Git and isolated by `11.0/global.json`.

## Configure the relying party

Passkeys are scoped to an RP domain. A public HTTPS dev tunnel supplies a stable domain that devices and
the Apple/Android association services can reach.

1. Build Android once if you plan to test Android. This creates the debug keystore actually used by
   .NET for Android:

   ```bash
   dotnet build src/Passkeys.Client/Passkeys.Client.csproj -f net11.0-android
   ```

2. Run setup from this directory:

   ```bash
   pwsh ./Configure-Passkeys.ps1
   ```

   The default tunnel ID is `maui-samples-passkeys`. Use a unique, stable ID if needed:

   ```bash
   pwsh ./Configure-Passkeys.ps1 -TunnelId <your-stable-tunnel-id>
   ```

   On macOS, setup attempts both Android and Apple configuration. Use `-NoApple` when you do not have
   Apple signing configured, or `-NoAndroid` when you are not testing Android. Use `-NoStartHost` to
   configure without starting the blocking tunnel host.

3. In another terminal, start the RP server:

   ```bash
   dotnet run --project src/Passkeys.Server --launch-profile http
   ```

4. Verify the public URL printed by the script:

   ```bash
   curl -fsS https://<tunnel-host>/health
   curl -fsS https://<tunnel-host>/.well-known/apple-app-site-association
   curl -fsS https://<tunnel-host>/.well-known/assetlinks.json
   ```

The script writes only local data:

- RP configuration goes to ASP.NET Core user-secrets.
- `src/Passkeys.Client/Passkeys.Local.props` contains the server URL, local application ID, and optional
  signing settings.
- Platform-specific `Entitlements.Local.plist` files under `Platforms/iOS` and
  `Platforms/MacCatalyst` contain the local Associated Domains entitlement while preserving each
  platform's base entitlements.

Both generated files are ignored by Git. The repository contains no tunnel credentials, fingerprints,
generated entitlements, signing identities, provisioning profiles, or developer-specific bundle IDs.

## Apple setup

Passkeys require all three of these values to match:

1. The app entitlement contains `webcredentials:<tunnel-host>`.
2. `https://<tunnel-host>/.well-known/apple-app-site-association` contains
   `<AppleTeamID>.<BundleID>`.
3. The app is signed by that Apple Developer team with a provisioning profile that has the Associated
   Domains capability.

### Requirements

- iOS 16+ or Mac Catalyst 17+. The native passkey API exists on Mac Catalyst 16+, but the .NET 11
  Mac Catalyst toolchain has a minimum deployment target of 17.0.
- macOS and Xcode.
- A paid Apple Developer account. Personal/free teams cannot provision Associated Domains.
- An explicit App ID registered to your team with Associated Domains enabled.
- An Apple Development certificate and a matching provisioning profile for device or Mac Catalyst use.

The committed `com.companyname.mauipasskeys` ID is only a placeholder. Choose your own globally unique
reverse-DNS ID and pass it to setup without editing the project:

```bash
pwsh ./Configure-Passkeys.ps1 \
  -ApplicationId com.example.mauipasskeys \
  -AppleTeamId ABCDE12345
```

On macOS, `AppleTeamId`, signing identity, and profile are auto-detected when possible. You can provide
`-AppleSigningIdentity` and `-AppleProvisioningProfile` explicitly. The iOS Simulator needs the generated
Associated Domains entitlement but does not need device signing. Mac Catalyst and physical iOS devices
need a profile carrying Associated Domains.

Apple fetches and caches the AASA file. Before launching, confirm it is public JSON and that its app ID
matches the signed app:

```bash
curl -i https://<tunnel-host>/.well-known/apple-app-site-association
```

## Android setup

### Requirements

- Android 14 / API 34 or newer.
- A Google Play system image, not an AOSP-only image.
- A Google account signed in on the emulator.
- A secure screen lock.

Android binds the RP to both the package name and signing certificate. Setup reads `ApplicationId` (or
the `-ApplicationId` value), finds the .NET for Android debug keystore, and calculates:

- The colon-delimited SHA-256 certificate fingerprint served by Digital Asset Links.
- The matching `android:apk-key-hash:<base64url>` native origin accepted by the server.

The default .NET for Android debug keystore is under the OS local application-data directory at
`Xamarin/Mono for Android/debug.keystore`. It is not Android Studio's `~/.android/debug.keystore`.

Confirm that the served document exactly matches the installed app:

```bash
curl -i https://<tunnel-host>/.well-known/assetlinks.json
```

No Android intent filter is needed. Digital Asset Links credential delegation is separate from Android
App Links.

## Windows setup

Use Windows 10 version 1903 or newer with:

- The Windows WebAuthn API.
- Windows Hello configured, or a compatible FIDO2 security key.
- Network access to the HTTPS RP URL.

Windows trusts the HTTPS RP origin directly, so it does not use AASA or Digital Asset Links.

## Run

Keep the server and tunnel running, then use the target appropriate for your host:

```bash
dotnet build -t:Run src/Passkeys.Client/Passkeys.Client.csproj -f net11.0-android
dotnet build -t:Run src/Passkeys.Client/Passkeys.Client.csproj -f net11.0-ios
dotnet build -t:Run src/Passkeys.Client/Passkeys.Client.csproj -f net11.0-maccatalyst
dotnet build -t:Run src/Passkeys.Client/Passkeys.Client.csproj -f net11.0-windows10.0.19041.0
```

In the app:

1. Create a test account or sign in with its password.
2. Select **Create a passkey** and approve the platform prompt.
3. Sign out.
4. Select **Sign in with a passkey** and choose the credential.

## Key files

| File | Purpose |
| --- | --- |
| `src/Passkeys.Client/ViewModels/PasskeysViewModel.cs` | Calls the native Passkeys API and transports WebAuthn JSON. |
| `src/Passkeys.Server/PasskeyEndpoints.cs` | Creates options and verifies/stores attestation and assertions. |
| `src/Passkeys.Server/Program.cs` | Configures Identity, in-memory storage, RP ID, and allowed origins. |
| `Configure-Passkeys.ps1` | Creates/reuses the tunnel and writes local-only trust configuration. |
| `src/Passkeys.Client/Passkeys.Local.in.props` | Template for generated URL, application ID, entitlements, and signing settings. |

## Troubleshooting

| Symptom | Check |
| --- | --- |
| Placeholder tunnel URL appears in the app | Re-run `Configure-Passkeys.ps1`, then rebuild the app. |
| Android reports no credential provider/create options | Use API 34+ with Google Play services, a signed-in Google account, and screen lock. |
| Android request cannot be validated | Package name, installed APK signing certificate, assetlinks fingerprint, and `android:apk-key-hash` origin must match. |
| Apple says the domain is not associated | Compare the signed Team ID/bundle ID, generated entitlement, and AASA response exactly. |
| AASA or assetlinks returns HTML | The tunnel interstitial is responding. Allow anonymous access and ensure the port is configured as HTTP. |
| `/finish` reports no ceremony in progress | Preserve the cookie from `/begin`; the sample uses one `HttpClient` with a `CookieContainer`. |
| Windows reports unsupported | Use Windows 10 1903+ and configure Windows Hello or a FIDO2 authenticator. |

## Source and release references

This sample adapts the implementation merged in
[dotnet/maui#36837](https://github.com/dotnet/maui/pull/36837) from the
[`release/11.0.1xx-preview7`](https://github.com/dotnet/maui/tree/release/11.0.1xx-preview7)
branch. See the [.NET 11 preview release notes](https://github.com/dotnet/core/tree/main/release-notes/11.0/preview)
and [.NET MAUI releases](https://github.com/dotnet/maui/releases) for the corresponding Preview 7 release
notes when published.
