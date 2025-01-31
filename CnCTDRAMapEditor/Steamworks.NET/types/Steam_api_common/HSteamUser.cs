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
    public struct HSteamUser : System.IEquatable<HSteamUser>, System.IComparable<HSteamUser> {
        public int m_HSteamUser;

        public HSteamUser(int value) => this.m_HSteamUser = value;

        public override string ToString() => this.m_HSteamUser.ToString();

        public override bool Equals(object other) => other is HSteamUser && this == (HSteamUser)other;

        public override int GetHashCode() => this.m_HSteamUser.GetHashCode();

        public static bool operator ==(HSteamUser x, HSteamUser y) => x.m_HSteamUser == y.m_HSteamUser;

        public static bool operator !=(HSteamUser x, HSteamUser y) => !(x == y);

        public static explicit operator HSteamUser(int value) => new HSteamUser(value);

        public static explicit operator int(HSteamUser that) => that.m_HSteamUser;

        public bool Equals(HSteamUser other) => this.m_HSteamUser == other.m_HSteamUser;

        public int CompareTo(HSteamUser other) => this.m_HSteamUser.CompareTo(other.m_HSteamUser);
    }
}

#endif // !DISABLESTEAMWORKS
