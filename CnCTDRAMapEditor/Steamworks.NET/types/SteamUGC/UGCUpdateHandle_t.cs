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
    public struct UGCUpdateHandle_t : System.IEquatable<UGCUpdateHandle_t>, System.IComparable<UGCUpdateHandle_t> {
        public static readonly UGCUpdateHandle_t Invalid = new UGCUpdateHandle_t(0xffffffffffffffff);
        public ulong m_UGCUpdateHandle;

        public UGCUpdateHandle_t(ulong value) => this.m_UGCUpdateHandle = value;

        public override string ToString() => this.m_UGCUpdateHandle.ToString();

        public override bool Equals(object other) => other is UGCUpdateHandle_t && this == (UGCUpdateHandle_t)other;

        public override int GetHashCode() => this.m_UGCUpdateHandle.GetHashCode();

        public static bool operator ==(UGCUpdateHandle_t x, UGCUpdateHandle_t y) => x.m_UGCUpdateHandle == y.m_UGCUpdateHandle;

        public static bool operator !=(UGCUpdateHandle_t x, UGCUpdateHandle_t y) => !(x == y);

        public static explicit operator UGCUpdateHandle_t(ulong value) => new UGCUpdateHandle_t(value);

        public static explicit operator ulong(UGCUpdateHandle_t that) => that.m_UGCUpdateHandle;

        public bool Equals(UGCUpdateHandle_t other) => this.m_UGCUpdateHandle == other.m_UGCUpdateHandle;

        public int CompareTo(UGCUpdateHandle_t other) => this.m_UGCUpdateHandle.CompareTo(other.m_UGCUpdateHandle);
    }
}

#endif // !DISABLESTEAMWORKS
