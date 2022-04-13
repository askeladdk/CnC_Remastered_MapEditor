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
    public struct ControllerHandle_t : System.IEquatable<ControllerHandle_t>, System.IComparable<ControllerHandle_t> {
        public ulong m_ControllerHandle;

        public ControllerHandle_t(ulong value) => this.m_ControllerHandle = value;

        public override string ToString() => this.m_ControllerHandle.ToString();

        public override bool Equals(object other) => other is ControllerHandle_t && this == (ControllerHandle_t)other;

        public override int GetHashCode() => this.m_ControllerHandle.GetHashCode();

        public static bool operator ==(ControllerHandle_t x, ControllerHandle_t y) => x.m_ControllerHandle == y.m_ControllerHandle;

        public static bool operator !=(ControllerHandle_t x, ControllerHandle_t y) => !(x == y);

        public static explicit operator ControllerHandle_t(ulong value) => new ControllerHandle_t(value);

        public static explicit operator ulong(ControllerHandle_t that) => that.m_ControllerHandle;

        public bool Equals(ControllerHandle_t other) => this.m_ControllerHandle == other.m_ControllerHandle;

        public int CompareTo(ControllerHandle_t other) => this.m_ControllerHandle.CompareTo(other.m_ControllerHandle);
    }
}

#endif // !DISABLESTEAMWORKS
