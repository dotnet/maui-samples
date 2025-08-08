You are an agent - please keep going until the user’s query is completely resolved, before ending your turn and yielding back to the user. Only terminate your turn when you are sure that the problem is solved.

If you are not sure about file content or codebase structure pertaining to the user’s request, use your tools to read files and gather the relevant information: do NOT guess or make up an answer.

You MUST plan extensively before each function call, and reflect extensively on the outcomes of the previous function calls. DO NOT do this entire process by making function calls only, as this can impair your ability to solve the problem and think insightfully.

When in Agent mode, work directly in the code files.

## NuGet dependencies

- Prefer the latest stable release versions of NuGet dependencies when adding or updating packages.
- If choosing the latest stable diverges from versions used elsewhere in this repository, call it out to the user with a brief note summarizing the differences before proceeding (or in the same turn when making the change).

## About the Project

This application is a .NET MAUI mobile and desktop application that helps users organize their "to do" lists into projects.

The solution file is in the /src folder, and the project file is in the /src/Telepathic folder. When issuing a `dotnet build` command you must include a Target Framework Moniker like `dotnet build -f net9.0-maccatalst`. Use the TFM that VS is currently targeting, or if you cannot read that use the version in the csproj file.

Here are some general .NET MAUI tips:

- Use `Border` instead of `Frame`
- Use `Grid` instead of `StackLayout`
- Use `CollectionView` instead of `ListView` for lists of greater than 20 items that should be virtualized
- Use `BindableLayout` with an appropriate layout inside a `ScrollView` for items of 20 or less that don't need to be virtualized
- Use `Background` instead of `BackgroundColor`


This project uses C# and XAML with an MVVM architecture. 

Use the .NET Community Toolkit for MVVM. Here are some helpful tips:

## Commands

- Use `RelayCommand` for commands that do not return a value.

```csharp
[RelayCommand]
Task DoSomethingAsync()
{
    // Your code here
}
```

This produces a `DoSomethingCommand` through code generation that can be used in XAML.

```xml
<Button Command="{Binding DoSomethingCommand}" Text="Do Something" />
```

