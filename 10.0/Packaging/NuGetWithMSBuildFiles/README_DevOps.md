# DevOps Pipeline for Building and Publishing NuGet Packages

This sample uses a GitHub Actions workflow to automate the building, packaging, and publishing of the [sample .NET MAUI Library](/README.md) to NuGet. This read me explains the key implementation details of the workflow.

## Workflow Overview

The `build_publish_nuget.yml` defines the end-to-end workflow for building and publishing the .NET MAUI Library NuGet Package. The [versioning step](#versioning-of-the-nuget-package) handles versioning of the NuGet package for every build via GitHub Actions. The workflow consists of two main jobs: `buildLibrary` and `publish`. These jobs use `dotnet` commands to build and pack the .NET project. The `publish` job depends on the `buildLibrary` job completing successfully. Then, once you have created the NuGet package, we can push it to [nuget.org](https://www.nuget.org) (or an Azure DevOps Internal Feed).

## Versioning of the NuGet Package

The versioning of the NuGet package is handled dynamically within the GitHub Actions workflow. The version is composed of the major version and the GitHub run ID, ensuring a unique version number for each workflow run.

### Setting the Version

In the `buildLibrary` job, the `VERSION` variable is set using the `MAJORVERSION` environment variable and the `GITHUB_RUN_ID`:

```yaml
- name: Set VERSION variable from tag
  run: |
    echo "VERSION=$MAJORVERSION-$GITHUB_RUN_ID" >> $GITHUB_ENV
```

This results in a version format like `1.0.0-1234`, where `1.0.0` is the major version and `1234` is the GitHub run ID.

### Using the Version

The `VERSION` variable is then used in the `dotnet pack` command to set the package version:

```yaml
- name: Pack Library
  run: |
    dotnet pack src/SamplePackage/SamplePackage.csproj -p:PackageVersion=$VERSION -c Release
```

This ensures that each NuGet package, built by the workflow, has a unique version preventing conflicts and making it easy to track and manage different builds.

## Secure and Sign NuGet Package

A signed package allows for content integrity verification checks which provides protection against content tampering. The package signature also serves as the single source of truth about the actual origin of the package and bolsters package authenticity for the consumer.

The NuGet Documentation provides detailed steps on how to [sign the package](https://learn.microsoft.com/nuget/create-packages/sign-a-package#sign-the-package) along with how to get a signing certificate. To implement this step in the workflow, update the Pack Library step:

```yaml
- name: Pack Library
  run: |
    dotnet pack src/SamplePackage/SamplePackage.csproj -p:PackageVersion=$VERSION -c Release
    dotnet nuget sign MyPackage.nupkg --certificate-path <PathToTheCertificate> --timestamper <TimestampServiceURL>

```

The certificate can be added to GitHub Actions by following the [deployment documentation](https://docs.github.com/en/actions/use-cases-and-examples/deploying/installing-an-apple-certificate-on-macos-runners-for-xcode-development). GitHub Actions handles the encoding and decoding of the certificate file.

## Pushing to NuGet.org or Azure DevOps Internal Feed

The workflow includes steps for pushing the NuGet package to NuGet.org or Azure DevOps internal feed. These steps can be configured as needed.

### Pushing to NuGet.org

To push the package to NuGet.org, use the following steps in the `publish` job and add your NuGet API key:

```yaml
- name: Push nuget to NuGet.org
  run: |
    dotnet nuget push **/*.nupkg -k ${{ secrets.NUGET_API_KEY }} -s https://api.nuget.org/v3/index.json --skip-duplicate
```

You can obtain the API key from your [NuGet.org account](https://www.nuget.org/account/apikeys). Make sure to set the `NUGET_API_KEY` secret variable in your GitHub repository's secrets. The [publish with netget push](https://learn.microsoft.com/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli#publish-with-dotnet-nuget-push) documentation explains the other overrides available and provides troubleshooting steps for errors.

### Pushing to Azure DevOps Internal Feed

To push the package to an Azure DevOps internal feed, use the following steps in the `publish` job and configure the feed URL and API key:

```yaml

- name: Setup .NET Core
  uses: actions/setup-dotnet@v1
  with:
    dotnet-version: ${{ env.DOTNETVERSION }}
    source-url: ${{ env.AZURE_ARTIFACTS_FEED_URL }}
  env:
    NUGET_AUTH_TOKEN: ${{ secrets.AZURE_DEVOPS_TOKEN }} 

- name: Push nuget to Azure DevOps Internal Feed
  run: |
    dotnet nuget push --api-key AzureArtifacts -s foo.nupkg

```

Replace `<AZURE_ARTIFACTS_FEED_URL>` with the URL of your Azure Artifacts feed. Make sure to set the `AZURE_DEVOPS_TOKEN` secret variable in your GitHub repository's secrets. The [Azure Artifacts Documentation](https://learn.microsoft.com/azure/devops/artifacts/quickstarts/github-actions?view=azure-devops&pivots=pat) explains the how to get the `AZURE_DEVOPS_TOKEN` in detail.

## Storing and Using Secret Values in GitHub Actions

To securely store and use sensitive information, such as API keys and tokens, in your GitHub Actions workflows you can use [GitHub Secrets](https://docs.github.com/actions/security-for-github-actions/security-guides/using-secrets-in-github-actions). Follow these steps to store and access secret values:

1. Navigate to your repository on GitHub and click on the **"Settings"** tab.
2. In the left sidebar, click on **"Secrets and variables"**, then click on **"Actions"**.
3. Click the **"New repository secret"** button and add a new secret:
    - **Name:** Enter a name for the secret (e.g., `NUGET_API_KEY`).
    - **Value:** Enter the secret value (e.g., your NuGet API key).
4. Click the **"Add secret"** button.

### Using Secrets in Workflows

Once you have added secrets to your repository, you can access them in your workflows using the `${{ secrets.SECRET_NAME }}` syntax. For example:

```yaml
- name: Push nuget to NuGet.org
  run: |
    dotnet nuget push **/*.nupkg -k ${{ secrets.NUGET_API_KEY }} -s https://api.nuget.org/v3/index.json --skip-duplicate
```

In this example, `NUGET_API_KEY` is the name of the secret that you added to your repository.

## Create Release in GitHub

Another step in this Github workflow is setup to automatically create a GitHub Release from GitHub Actions. This can be done using the [GitHub CLI](https://cli.github.com/) and then, in this sample, we use the default settings of the task to implement a simple version of release note for each version released.

```yaml
- name: Create Release and Upload Artifact to Release
  run: gh release create ${{env.VERSION_NUMBER}} -t ${{env.VERSION_NUMBER}} *.nupkg --generate-notes
```

In this sample, the syntax for the Release is to use the version number of the nuget package. We also opt to upload the NuGet package, generated at part of the Release, and also use the default `--generate-notes` command. This step is very customizable. For example, you can control triggers based on different tags and also make custom release notes. The [GitHub CLI Release Create](https://cli.github.com/manual/gh_release_create) documentation explains these options in detail.

> [!NOTE]
> To use the GitHub CLI in GitHub Actions, we use an automatic secrets value for [GITHUB_TOKEN](https://docs.github.com/actions/security-for-github-actions/security-guides/automatic-token-authentication) providing the `write-all` [permission](https://docs.github.com/actions/writing-workflows/choosing-what-your-workflow-does/controlling-permissions-for-github_token#overview) in order to generate the release notes.

## Summary

This sample shows how to setup a basic GitHub Actions workflow to build and publish a .NET MAUI Library. It shows how to build, pack, and sign the NuGet package and, based on the use case, publish the package to NuGet.org, Azure DevOps Internal Feed, and create a GitHub Release. This basic workflow is customizable and can be extended to meet more complex requirements by following the linked documentation.
