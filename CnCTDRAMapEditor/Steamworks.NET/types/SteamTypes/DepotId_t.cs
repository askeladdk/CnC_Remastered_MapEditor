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
    public struct DepotId_t : System.IEquatable<DepotId_t>, System.IComparable<DepotId_t> {
        public static readonly DepotId_t Invalid = new DepotId_t(0x0);
        public uint m_DepotId;

        public DepotId_t(uint value) => this.m_DepotId = value;

        public override string ToString() => this.m_DepotId.ToString();

        public override bool Equals(object other) => other is DepotId_t && this == (DepotId_t)other;

        public override int GetHashCode() => this.m_DepotId.GetHashCode();

        public static bool operator ==(DepotId_t x, DepotId_t y) => x.m_DepotId == y.m_DepotId;

        public static bool operator !=(DepotId_t x, DepotId_t y) => !(x == y);

        public static explicit operator DepotId_t(uint value) => new DepotId_t(value);

        public static explicit operator uint(DepotId_t that) => that.m_DepotId;

        public bool Equals(DepotId_t other) => this.m_DepotId == other.m_DepotId;

        public int CompareTo(DepotId_t other) => this.m_DepotId.CompareTo(other.m_DepotId);
    }
}

#endif // !DISABLESTEAMWORKS
