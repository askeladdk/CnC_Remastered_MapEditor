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
    public struct HServerListRequest : System.IEquatable<HServerListRequest> {
        public static readonly HServerListRequest Invalid = new HServerListRequest(System.IntPtr.Zero);
        public System.IntPtr m_HServerListRequest;

        public HServerListRequest(System.IntPtr value) => this.m_HServerListRequest = value;

        public override string ToString() => this.m_HServerListRequest.ToString();

        public override bool Equals(object other) => other is HServerListRequest && this == (HServerListRequest)other;

        public override int GetHashCode() => this.m_HServerListRequest.GetHashCode();

        public static bool operator ==(HServerListRequest x, HServerListRequest y) => x.m_HServerListRequest == y.m_HServerListRequest;

        public static bool operator !=(HServerListRequest x, HServerListRequest y) => !(x == y);

        public static explicit operator HServerListRequest(System.IntPtr value) => new HServerListRequest(value);

        public static explicit operator System.IntPtr(HServerListRequest that) => that.m_HServerListRequest;

        public bool Equals(HServerListRequest other) => this.m_HServerListRequest == other.m_HServerListRequest;
    }
}

#endif // !DISABLESTEAMWORKS
