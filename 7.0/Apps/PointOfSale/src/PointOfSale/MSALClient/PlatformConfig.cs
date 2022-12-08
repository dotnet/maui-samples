// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace MAUI.MSALClient
{
    /// <summary>
    /// Platform specific configuration holder.
    /// </summary>
    public class PlatformConfig
    {
        /// <summary>
        /// The singleton instance to hold data per platform
        /// </summary>
        public static PlatformConfig Instance { get; } = new PlatformConfig();

        /// <summary>
        /// Platform specific Redirect URI
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Platform specific parent window
        /// </summary>
        public object ParentWindow { get; set; }

        // private constructor to ensure singleton
        private PlatformConfig()
        {
        }
    }
}