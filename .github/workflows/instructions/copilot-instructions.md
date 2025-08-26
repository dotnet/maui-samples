---
applyTo: '**'
---

# GitHub Copilot Development Environment Instructions

This document provides specific guidance for GitHub Copilot when working on the .NET MAUI Samples repository. It serves as context for understanding the project structure, development workflow, and best practices.

## Repository Overview

This repository contains **sample applications and demonstrations** for **.NET MAUI** (Multi-platform App UI), a cross-platform framework for creating mobile and desktop applications with C# and XAML. The samples showcase various features, controls, and capabilities of the .NET MAUI framework across Android, iOS, iPadOS, macOS, and Windows platforms.

### Key Technologies
- **.NET SDK** - Various versions depending on the sample:
  - **8.0 folder**: Samples targeting .NET 8
  - **9.0 folder**: Samples targeting .NET 9
  - **10.0 folder**: Samples targeting .NET 10
  - etc.
- **C#** and **XAML** for application development
- **MSBuild** for building individual samples
- **Various testing frameworks** including NUnit and Appium for UI testing samples

## Development Environment Setup

### Prerequisites

Before working with .NET MAUI samples, ensure your development environment is properly configured according to the official documentation:

1. **Follow the official .NET MAUI installation guide:**
   - Visit [Install .NET MAUI](https://docs.microsoft.com/dotnet/maui/get-started/installation) for comprehensive setup instructions
   - Install the required .NET SDK version (8.0, 9.0, or 10.0 depending on the sample)

2. **Verify installation:**
   ```powershell
   dotnet --version
   dotnet workload list
   ```

### Initial Repository Setup

1. **Clone and navigate to repository:**
   ```powershell
   git clone https://github.com/dotnet/maui-samples.git
   cd maui-samples
   ```

2. **Choose and navigate to the appropriate sample:**
   ```powershell
   # For .NET 10 samples
   cd 10.0/UserInterface/ControlGallery
   
   # For .NET 9 samples
   cd 9.0/Apps/Calculator
   
   # For .NET 8 samples  
   cd 8.0/Apps/WeatherTwentyOne
   ```

3. **Build and run the sample:**
   ```powershell
   dotnet build
   dotnet run
   ```

## Project Structure

### Important Directories
- `8.0/` - Samples targeting .NET 8
- `9.0/` - Samples targeting .NET 9
- `10.0/` - Samples targeting .NET 10
- `eng/` - Build engineering and tooling
- `.github/` - GitHub workflows and configuration
- `Images/` - Shared images used across samples
- `Upgrading/` - Migration examples from Xamarin.Forms to .NET MAUI

### Platform-Specific Code Organization
- **Android** specific code is inside folders labeled `Android`
- **iOS** specific code is inside folders labeled `iOS`
- **MacCatalyst** specific code is inside folders named `MacCatalyst`
- **Windows** specific code is inside folders named `Windows`

### Platform-Specific File Extensions
- Files with `.windows.cs` will only compile for the Windows TFM
- Files with `.android.cs` will only compile for the Android TFM
- Files with `.ios.cs` will only compile for the iOS and MacCatalyst TFM
- Files with `MacCatalyst.cs` will only compile for the MacCatalyst TFM

### Sample Categories
```
├── Apps/ - Complete sample applications
│   ├── Calculator/
│   ├── WeatherTwentyOne/
│   ├── PointOfSale/
│   ├── WhatToEat/
│   └── ...
├── UserInterface/ - UI controls and layout demos
│   ├── ControlGallery/
│   ├── Views/
│   ├── Layouts/
│   └── ...
├── Navigation/ - Navigation pattern samples
├── Data/ - Data access and storage samples  
├── WebServices/ - REST API and web service samples
├── UITesting/ - UI testing with Appium samples
├── Fundamentals/ - Core concepts like data binding, MVVM
├── PlatformIntegration/ - Platform-specific features
├── XAML/ - XAML markup and styling samples
├── SkiaSharp/ - SkiaSharp graphics samples
├── Tutorials/ - Step-by-step learning samples
├── Packaging/ - NuGet packaging and MSBuild samples
├── Sensors/ - Device sensor integration samples
└── Animations/ - Animation and visual effects samples
```

## Development Workflow

### Creating New Samples

When creating new .NET MAUI samples, use the `dotnet new maui` template:

```powershell
# Create a new .NET MAUI sample
dotnet new maui -n SampleName

# Navigate to the sample directory
cd SampleName
```

### Building Individual Samples

Each sample is self-contained and can be built independently:

```powershell
# Navigate to a specific sample
cd 10.0/Apps/Calculator

# Build the sample
dotnet build

# Run the sample (if supported on current platform)
dotnet run
```

### Sample Structure

Most samples follow this structure:
```
SampleName/
├── SampleName.sln - Solution file
├── SampleName/ - Main project folder
├── Screenshots/ - Sample screenshots (PNG format)
└── README.md - Sample documentation
```

## Sample Requirements

### Required Files and Structure

Every sample must include:

1. **README.md** - Documentation file with YAML front matter header
2. **Screenshots folder** - Empty folder for future screenshots
3. **Project files** - Standard .NET MAUI project structure
4. **Solution file** - `.sln` file for the sample

### README.md Format

Each sample's README.md must follow this format:

```markdown
---
name: .NET MAUI - Sample Name
description: Brief description of what the sample demonstrates.
page_type: sample
languages:
- csharp
- xaml
products:
- dotnet-maui
urlFragment: category-samplename
---

# Sample Name

Detailed description of the sample, its purpose, and key features demonstrated.

```

**Key elements:**
- **YAML front matter** with metadata (name, description, page_type, languages, products, urlFragment)
- **Clear sample name** in the `name` field prefixed with ".NET MAUI - "
- **Concise description** explaining what the sample demonstrates
- **URL fragment** following the pattern: `category-samplename` (lowercase, hyphenated)
- **Detailed content** explaining the sample's purpose and features

### Screenshots Requirements

- Create an empty `Screenshots/` folder in each sample directory
- Screenshots will be added later by maintainers

### Testing and Debugging

#### UI Testing Guidelines

Some samples include UI testing with Appium and NUnit. When working with UI testing samples:

1. **Navigate to the UI testing sample:**
   ```powershell
   cd 10.0/UITesting/BasicAppiumNunitSample
   ```

2. **Build the test projects:**
   ```powershell
   dotnet build UITests.Shared/UITests.Shared.csproj
   ```

3. **Run platform-specific tests:**
   ```powershell
   # For Android
   dotnet build UITests.Android/UITests.Android.csproj
   # For iOS (on macOS)
   dotnet build UITests.iOS/UITests.iOS.csproj
   ```

## Additional Resources

- [.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui)
- [.NET MAUI GitHub Repository](https://github.com/dotnet/maui)
- [Samples Browser](https://docs.microsoft.com/samples/browse/?expanded=dotnet&products=dotnet-maui)

---

**Note for Future Updates:** This document should be expanded as new development patterns, tools, or workflows are discovered. Add sections for specific scenarios, debugging techniques, or tooling as they become relevant to the development process.
