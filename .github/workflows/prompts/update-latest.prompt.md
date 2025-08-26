---
mode: agent
---
I would like to update the 9.0 samples to the latest .NET 10 version. Make sure to use the Microsoft Learn MCP. Please follow these steps:

1. **Create a folder called 10.0**: Create a new folder named `10.0` in the root directory of the repository.
2. **Copy the 9.0 samples**: Copy all the contents from the `9.0` folder into the newly created `10.0` folder. Don't copy bin and obj if present.
3. **Update Target Framework**: Change the target framework in the project files from `net9.0` to `net10.0`.
4. **Update SupportedOSPlatformVersion**: Update SupportedOSPlatformVersion to their latest versions compatible with .NET 10.
5. **Update Deprecated APIs**: Review and update any deprecated APIs or obsolete methods to use the latest recommended alternatives in .NET 10. Address any compiler warnings and follow guidance from Microsoft Learn documentation.
6. **Address Build Warnings**: Resolve any build warnings that appear during compilation, referring to Microsoft Learn for best practices and updated guidance.
7. **Test the Samples**: Build each sample to ensure they compile with the newest version and run without warnings.
