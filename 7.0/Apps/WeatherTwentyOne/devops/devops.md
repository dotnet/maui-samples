# .NET MAUI DevOps

Resources for the DevOps pipelines setup for WeatherTwentyOne sample app.

## Pipeline Source Files

#### GitHub Actions Pipeline

- MacOS Hosted Agent : [macos-build.yml](GitHubActions/macos-build.yml)
- Windows Hosted Agent: [windowsCI.yml](GitHubActions/windowsCI.yml)

Useful Documentation :

Installed Software List on Hosted Agents for GitHub Actions : [Available Environments](https://github.com/actions/virtual-environments#available-environments)

#### Azure DevOps Pipeline

- MacOS Hosted Agent : [azdo_mac.yml](AzureDevOps/azdo_mac.yml)
- Windows Hosted Agent: [azdo_windows.yml](AzureDevOps/azdo_windows.yml)

Useful Documentation :

Installed Software List on Hosted Agents for Azure DevOps : [Installed Software](https://docs.microsoft.com/en-us/azure/devops/pipelines/agents/hosted?view=azure-devops&tabs=yaml#software)