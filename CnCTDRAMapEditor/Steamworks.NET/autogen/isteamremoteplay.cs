// This file is provided under The MIT License as part of Steamworks.NET.
// Copyright (c) 2013-2019 Riley Labrecque
// Please see the included LICENSE.txt for additional information.

// This file is automatically generated.
// Changes to this file will be reverted when you update Steamworks.NET

#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif

#if !DISABLESTEAMWORKS

namespace Steamworks {
    public static class SteamRemotePlay {
        /// <summary>
        /// <para> Get the number of currently connected Steam Remote Play sessions</para>
        /// </summary>
        public static uint GetSessionCount() {
            InteropHelp.TestIfAvailableClient();
            return NativeMethods.ISteamRemotePlay_GetSessionCount(CSteamAPIContext.GetSteamRemotePlay());
        }

        /// <summary>
        /// <para> Get the currently connected Steam Remote Play session ID at the specified index. Returns zero if index is out of bounds.</para>
        /// </summary>
        public static uint GetSessionID(int iSessionIndex) {
            InteropHelp.TestIfAvailableClient();
            return NativeMethods.ISteamRemotePlay_GetSessionID(CSteamAPIContext.GetSteamRemotePlay(), iSessionIndex);
        }

        /// <summary>
        /// <para> Get the SteamID of the connected user</para>
        /// </summary>
        public static CSteamID GetSessionSteamID(uint unSessionID) {
            InteropHelp.TestIfAvailableClient();
            return (CSteamID)NativeMethods.ISteamRemotePlay_GetSessionSteamID(CSteamAPIContext.GetSteamRemotePlay(), unSessionID);
        }

        /// <summary>
        /// <para> Get the name of the session client device</para>
        /// <para> This returns NULL if the sessionID is not valid</para>
        /// </summary>
        public static string GetSessionClientName(uint unSessionID) {
            InteropHelp.TestIfAvailableClient();
            return InteropHelp.PtrToStringUTF8(NativeMethods.ISteamRemotePlay_GetSessionClientName(CSteamAPIContext.GetSteamRemotePlay(), unSessionID));
        }

        /// <summary>
        /// <para> Get the form factor of the session client device</para>
        /// </summary>
        public static ESteamDeviceFormFactor GetSessionClientFormFactor(uint unSessionID) {
            InteropHelp.TestIfAvailableClient();
            return NativeMethods.ISteamRemotePlay_GetSessionClientFormFactor(CSteamAPIContext.GetSteamRemotePlay(), unSessionID);
        }

        /// <summary>
        /// <para> Get the resolution, in pixels, of the session client device</para>
        /// <para> This is set to 0x0 if the resolution is not available</para>
        /// </summary>
        public static bool BGetSessionClientResolution(uint unSessionID, out int pnResolutionX, out int pnResolutionY) {
            InteropHelp.TestIfAvailableClient();
            return NativeMethods.ISteamRemotePlay_BGetSessionClientResolution(CSteamAPIContext.GetSteamRemotePlay(), unSessionID, out pnResolutionX, out pnResolutionY);
        }
    }
}

#endif // !DISABLESTEAMWORKS
