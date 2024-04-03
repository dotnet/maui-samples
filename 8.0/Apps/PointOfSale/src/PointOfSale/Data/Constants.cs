using System;
namespace PointOfSale.Data;

public static class Constants
{
    /// <summary>
    /// The base URI for the Datasync service.
    /// </summary>
    public static string ServiceUri = "https://yojjaontbpmia-service.azurewebsites.net";

    /// <summary>
    /// The application (client) ID for the native app within Azure Active Directory
    /// </summary>
    public static string ApplicationId = "e8b7e84c-1cb6-4619-bee9-ace98d4211e5";

    /// <summary>
    /// The list of scopes to request
    /// </summary>
    public static string[] Scopes = new[]
    {
          "access_as_user"
      };
}