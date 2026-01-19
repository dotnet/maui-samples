---
name: appium-automation
description: Cross-platform mobile app automation using Appium. Supports iOS Simulator, Android Emulator, and Mac Catalyst. Use for UI testing, reproducing issues, and validating fixes on device.
---

# Appium Automation Skill

Cross-platform mobile automation agent for iOS, Android, and Mac Catalyst apps.

## When to Use

- ✅ Automating UI interactions on iOS Simulator
- ✅ Automating UI interactions on Android Emulator
- ✅ Testing Mac Catalyst apps
- ✅ Reproducing issues on device
- ✅ Validating PR fixes with real device testing
- ✅ Exploring app UI structure (list elements, buttons)

## When NOT to Use

- ❌ Writing NUnit UI tests → Use `uitest-coding-agent`
- ❌ Full PR workflow → Use `pr` agent
- ❌ Just building/deploying Sandbox → Use `sandbox-agent`

## Important: AutomationId Setup

For reliable element targeting, **add `AutomationId` to your XAML elements**:

```xml
<!-- Before: No way to reliably target this element -->
<Entry Placeholder="Enter amount" />

<!-- After: Can target with --tap AmountEntry or --type AmountEntry "100" -->
<Entry AutomationId="AmountEntry" Placeholder="Enter amount" />
```

### Recommended Pattern

Add `AutomationId` to all interactive elements:

```xml
<Entry AutomationId="SubtotalEntry" ... />
<Button AutomationId="CalculateButton" ... />
<Label AutomationId="ResultLabel" ... />
<Slider AutomationId="TipSlider" ... />
```

### Platform Mapping

| Platform | AutomationId maps to |
|----------|---------------------|
| iOS | `name` attribute |
| Android | `resource-id` (e.g., `com.app:id/SubtotalEntry`) |
| Mac Catalyst | `identifier` attribute |

Without `AutomationId`, you must use `--tap-text` or `--tap-button` which are less reliable.

## Prerequisites

### 1. Appium Server

```bash
npm install -g appium
appium driver install xcuitest      # iOS/Catalyst
appium driver install uiautomator2  # Android
appium driver install mac2          # Mac Catalyst
```

**Note**: Appium auto-starts if not running when you run an automation command.

### 2. Python Dependencies

```bash
pip install Appium-Python-Client selenium
```

### 3. Platform Requirements

| Platform | Requirements |
|----------|--------------|
| iOS | Xcode, booted iOS Simulator |
| Android | Android SDK, running emulator or connected device |
| Mac Catalyst | Xcode, app installed on Mac, **WebDriverAgentMac built** (see below) |

### 4. Mac Catalyst Setup (Required for Mac Catalyst only)

The Mac2 driver requires WebDriverAgentMac to be built before first use:

```bash
# Navigate to the WebDriverAgentMac project
cd ~/.appium/node_modules/appium-mac2-driver/WebDriverAgentMac

# Build WebDriverAgentRunner for macOS
xcodebuild -project WebDriverAgentMac.xcodeproj \
  -scheme WebDriverAgentRunner \
  -destination 'platform=macOS' \
  -configuration Debug \
  build
```

**Note**: This only needs to be done once after installing the mac2 driver. If you update the driver (`appium driver update mac2`), rebuild WebDriverAgentMac.

## Quick Start

### Start Appium Server

```bash
# Start in background
appium --relaxed-security &

# Or use the script
python .github/skills/appium-automation/scripts/automate.py --start-appium
```

### List Available Devices

```bash
python .github/skills/appium-automation/scripts/automate.py --list-devices
```

### Boot iOS Simulator

```bash
python .github/skills/appium-automation/scripts/automate.py --boot-simulator "iPhone 16 Pro"
```

## CLI Reference

### Basic Syntax

```bash
python .github/skills/appium-automation/scripts/automate.py \
  --platform <ios|android|maccatalyst> \
  --app-id <bundle.id.or.package> \
  [actions...]
```

### Actions

| Action | Description | Example |
|--------|-------------|---------|
| `--tap ID` | Tap element by AutomationId | `--tap SubmitButton` |
| `--tap-button TEXT` | Tap button by visible text | `--tap-button "Sign In"` |
| `--tap-text TEXT` | Tap any element with text | `--tap-text "Welcome"` |
| `--tap-like PARTIAL` | Tap element with partial ID match | `--tap-like Submit` |
| `--double-tap ID` | Double-tap element | `--double-tap ImageView` |
| `--long-press ID` | Long press element | `--long-press ListItem` |
| `--type ID TEXT` | Type into text field (clears first) | `--type EmailField "user@test.com"` |
| `--clear ID` | Clear text from field | `--clear SearchField` |
| `--get-text ID` | Get element text content | `--get-text ResultLabel` |
| `--exists ID` | Check if element exists | `--exists ErrorMessage` |
| `--is-enabled ID` | Check if element is enabled | `--is-enabled SubmitButton` |
| `--is-visible ID` | Check if element is visible | `--is-visible LoadingSpinner` |
| `--expect ID TEXT` | Assert element contains text | `--expect TotalLabel "€ 100"` |
| `--wait N` | Wait N seconds | `--wait 2` |
| `--wait-for ID` | Wait for element to appear | `--wait-for LoadingSpinner` |

### Keyboard & Input

| Action | Description | Example |
|--------|-------------|---------|
| `--dismiss-keyboard` | Dismiss on-screen keyboard | `--dismiss-keyboard` |
| `--press-key KEY` | Press key (Enter/Tab/Escape) | `--press-key Enter` |

### Alerts & Dialogs

| Action | Description | Example |
|--------|-------------|---------|
| `--accept-alert` | Accept/tap OK on alert | `--accept-alert` |
| `--dismiss-alert` | Dismiss/cancel alert | `--dismiss-alert` |
| `--get-alert` | Get alert text | `--get-alert` |

### Sliders

| Action | Description | Example |
|--------|-------------|---------|
| `--set-slider ID PCT` | Set slider to percentage (0-100) | `--set-slider TipSlider 25` |
| `--drag ID X Y [SEC]` | Drag element by offset | `--drag Handle 100 0 0.5` |

### Gestures

| Gesture | Description | Example |
|---------|-------------|---------|
| `--swipe DIR` | Swipe up/down/left/right | `--swipe up` |
| `--scroll DIR` | Scroll screen in direction | `--scroll down` |
| `--scroll-to ID` | Scroll until element visible | `--scroll-to BottomButton` |
| `--tap-coords X Y` | Tap at coordinates | `--tap-coords 100 200` |

### App Lifecycle

| Command | Description |
|---------|-------------|
| `--activate` | Bring app to foreground |
| `--terminate` | Close/kill app |
| `--install PATH` | Install .app or .apk |

### Debug & Inspection

| Command | Description |
|---------|-------------|
| `--screenshot PATH` | Save screenshot |
| `--page-source` | Print XML element tree |
| `--list-buttons` | List all button labels |
| `--list-elements` | List all elements with IDs |
| `--find-text TEXT` | Find elements with text |
| `--get-rect ID` | Get element position/size |

## Common Workflows

### .NET MAUI iOS Testing

```bash
# 1. Boot simulator and start Appium
python .github/skills/appium-automation/scripts/automate.py --boot-simulator "iPhone 16 Pro"
python .github/skills/appium-automation/scripts/automate.py --start-appium

# 2. Build and install app (using dotnet)
dotnet build src/Controls/samples/Controls.Sample.Sandbox -f net9.0-ios
xcrun simctl install booted path/to/Controls.Sample.Sandbox.app

# 3. Automate
python .github/skills/appium-automation/scripts/automate.py \
  --platform ios \
  --app-id com.microsoft.maui.sandbox \
  --tap NavigateButton --wait 1 --get-text ResultLabel
```

### .NET MAUI Android Testing

```bash
# 1. Start emulator and Appium
emulator -avd Pixel_6_API_33 &
python .github/skills/appium-automation/scripts/automate.py --start-appium

# 2. Build and install (IMPORTANT: use EmbedAssembliesIntoApk for automation)
dotnet build src/Controls/samples/Controls.Sample.Sandbox -f net9.0-android -p:EmbedAssembliesIntoApk=true
adb install path/to/com.microsoft.maui.sandbox-Signed.apk

# 3. Automate
python .github/skills/appium-automation/scripts/automate.py \
  --platform android \
  --app-id com.microsoft.maui.sandbox \
  --tap NavigateButton --wait 1 --screenshot result.png
```

### Chained Actions

```bash
# Multiple actions in sequence (RECOMMENDED for performance)
python .github/skills/appium-automation/scripts/automate.py \
  --platform ios --app-id com.example.app \
  --tap UsernameField --type UsernameField "testuser" \
  --tap PasswordField --type PasswordField "password123" \
  --tap-button "Sign In" --wait 2 \
  --get-text WelcomeLabel
```

> **Performance Tip**: Chain multiple operations in a single CLI call. Each invocation creates a new Appium session (~10-50s on iOS). Chaining operations runs them all in one session, making subsequent operations nearly instant.

### Debug: Explore App Structure

```bash
# List all buttons
python .github/skills/appium-automation/scripts/automate.py \
  --platform ios --app-id com.example.app \
  --list-buttons

# Full element tree
python .github/skills/appium-automation/scripts/automate.py \
  --platform ios --app-id com.example.app \
  --page-source > elements.xml
```

## Python API

```python
from appium_agent import AppiumAgent

# iOS
with AppiumAgent(platform="ios", app_id="com.microsoft.maui.sandbox") as agent:
    agent.tap("NavigateButton")
    agent.type_text("SearchField", "hello world")
    agent.wait(1)
    result = agent.get_text("ResultLabel")
    agent.screenshot("result.png")

# Android
with AppiumAgent(platform="android", app_id="com.microsoft.maui.sandbox") as agent:
    agent.tap("SubmitButton")
    agent.swipe("up")
    buttons = agent.list_buttons()

# Mac Catalyst
with AppiumAgent(platform="maccatalyst", app_id="com.example.macapp") as agent:
    agent.tap_button("Preferences")
```

## .NET MAUI Element Identification

In .NET MAUI, use `AutomationId` for element identification:

```xml
<Button Text="Submit" AutomationId="SubmitButton" />
<Entry Placeholder="Email" AutomationId="EmailField" />
<Label x:Name="resultLabel" AutomationId="ResultLabel" />
```

The agent finds elements by:
1. `AutomationId` (accessibility ID)
2. `x:Name` (name attribute)
3. Visible text (label/text attribute)

## Troubleshooting

### "No booted iOS simulator found"

```bash
xcrun simctl boot "iPhone 16 Pro"
# or
python automate.py --boot-simulator "iPhone 16 Pro"
```

### "No Android device/emulator found"

```bash
# Start emulator
emulator -avd <avd_name> &

# Or check connected devices
adb devices
```

### "Appium server not running"

```bash
appium --relaxed-security &
# or
python automate.py --start-appium
```

### Element not found

1. Use `--list-buttons` to see available buttons
2. Use `--list-elements` to see all elements with IDs
3. Use `--page-source` to get full XML tree
4. Verify `AutomationId` is set in XAML

### Android "Fast Deployment" crash

**Symptom**: App crashes immediately on launch with error:
```
No assemblies found in '.../__override__/arm64-v8a' or '<unavailable>'. 
Assuming this is part of Fast Deployment. Exiting...
```

**Cause**: Debug builds use "Fast Deployment" which doesn't embed assemblies in the APK. The assemblies are pushed separately by Visual Studio/dotnet tooling. When launched via Appium (outside VS), the app can't find the assemblies.

**Fix**: Build with embedded assemblies:
```bash
dotnet build -f net9.0-android -p:EmbedAssembliesIntoApk=true
```

Or use a Release build which embeds assemblies by default.

**Note**: The agent already sets `appium:noReset=true` by default to preserve app state between sessions.

### Slow performance / Session startup

**Symptom**: Each CLI call takes 10-50 seconds on iOS.

**Cause**: Each CLI invocation creates a new Appium/WebDriverAgent session. Session startup is slow, especially on iOS (~10-50s).

**Fix 1**: Use session caching with `--keep-session` and `--reuse-session`:
```bash
# First call creates session and keeps it alive (~10s)
python automate.py --platform ios --app-id com.example --keep-session --tap Field1

# Subsequent calls reuse the session (~2s each!)
python automate.py --platform ios --app-id com.example --reuse-session --keep-session --type Field1 "text"
python automate.py --platform ios --app-id com.example --reuse-session --keep-session --list-elements

# When done, end the session
python automate.py --end-session
```

**Fix 2**: Chain multiple operations in a single CLI call:
```bash
# 1 session for all operations - executed IN ORDER specified!
python automate.py --platform ios --app-id com.example \
  --tap Field1 --type Field1 "text" --get-text Result --expect Result "success"
```

**Note**: Actions are executed in the order you specify on the command line. You can interleave actions as needed:
```bash
# Type, check value, type more, check again
--type Amount 100 --get-text Total --type Tip 20 --expect Total "€ 120"
```

## Assertions

The `--expect` action is used for automated testing. It checks if an element contains expected text:

```bash
# Pass: exits 0
python automate.py --platform ios --app-id com.example --expect Label "expected text"

# Fail: prints failure and exits 1
python automate.py --platform ios --app-id com.example --expect Label "wrong"
# Output: ✗ expect 'Label' contains 'wrong': FAIL (actual: expected text)
```

Use `--expect` in test scripts to verify UI state after actions:
```bash
python automate.py --platform ios --app-id com.example \
  --type SubtotalEntry 100 \
  --set-slider TipSlider 20 \
  --expect TipAmountValue "€ 20,00" \
  --expect TotalValue "€ 120,00"
```

## Platform Feature Matrix

| Feature | iOS | Android | Mac Catalyst |
|---------|-----|---------|--------------|
| tap | ✅ | ✅ | ✅ |
| type_text | ✅ | ✅ | ✅ |
| swipe | ✅ | ✅ | ✅ |
| long_press | ✅ | ✅ | ✅ |
| double_tap | ✅ | ✅ | ✅ |
| set_slider | ✅ | ✅ | ✅ |
| expect | ✅ | ✅ | ✅ |
| dismiss_keyboard | ✅ | ✅ | ❌ |
| accept_alert | ✅ | ✅ | ✅ |
| press_back | ❌ | ✅ | ❌ |
| orientation | ✅ | ✅ | ❌ |
