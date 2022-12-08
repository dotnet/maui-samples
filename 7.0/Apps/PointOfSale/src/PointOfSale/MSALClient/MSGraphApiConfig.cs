// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace MAUI.MSALClient
{
    public class MSGraphApiConfig
    {
        /// <summary>
        /// Gets or sets the MS Graph base URL.
        /// </summary>
        /// <value>
        /// The Microsoft Graph base URL.
        /// </value>
        public string MSGraphBaseUrl { get; set; }

        /// <summary>
        /// Gets or sets the scopes for MS graph call.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public string Scopes { get; set; }

        /// <summary>
        /// Gets the scopes in a format as expected by the various MSAL SDK methods.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public string[] ScopesArray
        {
            get
            {
                return Scopes.Split(' ');
            }
        }
    }
}