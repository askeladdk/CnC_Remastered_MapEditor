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
    [System.Serializable]
    public struct SteamAPICall_t : System.IEquatable<SteamAPICall_t>, System.IComparable<SteamAPICall_t> {
        public static readonly SteamAPICall_t Invalid = new SteamAPICall_t(0x0);
        public ulong m_SteamAPICall;

        public SteamAPICall_t(ulong value) => this.m_SteamAPICall = value;

        public override string ToString() => this.m_SteamAPICall.ToString();

        public override bool Equals(object other) => other is SteamAPICall_t && this == (SteamAPICall_t)other;

        public override int GetHashCode() => this.m_SteamAPICall.GetHashCode();

        public static bool operator ==(SteamAPICall_t x, SteamAPICall_t y) => x.m_SteamAPICall == y.m_SteamAPICall;

        public static bool operator !=(SteamAPICall_t x, SteamAPICall_t y) => !(x == y);

        public static explicit operator SteamAPICall_t(ulong value) => new SteamAPICall_t(value);

        public static explicit operator ulong(SteamAPICall_t that) => that.m_SteamAPICall;

        public bool Equals(SteamAPICall_t other) => this.m_SteamAPICall == other.m_SteamAPICall;

        public int CompareTo(SteamAPICall_t other) => this.m_SteamAPICall.CompareTo(other.m_SteamAPICall);
    }
}

#endif // !DISABLESTEAMWORKS
