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
    public struct SiteId_t : System.IEquatable<SiteId_t>, System.IComparable<SiteId_t> {
        public static readonly SiteId_t Invalid = new SiteId_t(0);
        public ulong m_SiteId;

        public SiteId_t(ulong value) => this.m_SiteId = value;

        public override string ToString() => this.m_SiteId.ToString();

        public override bool Equals(object other) => other is SiteId_t && this == (SiteId_t)other;

        public override int GetHashCode() => this.m_SiteId.GetHashCode();

        public static bool operator ==(SiteId_t x, SiteId_t y) => x.m_SiteId == y.m_SiteId;

        public static bool operator !=(SiteId_t x, SiteId_t y) => !(x == y);

        public static explicit operator SiteId_t(ulong value) => new SiteId_t(value);

        public static explicit operator ulong(SiteId_t that) => that.m_SiteId;

        public bool Equals(SiteId_t other) => this.m_SiteId == other.m_SiteId;

        public int CompareTo(SiteId_t other) => this.m_SiteId.CompareTo(other.m_SiteId);
    }
}

#endif // !DISABLESTEAMWORKS
