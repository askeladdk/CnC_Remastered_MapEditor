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
    public struct InputHandle_t : System.IEquatable<InputHandle_t>, System.IComparable<InputHandle_t> {
        public ulong m_InputHandle;

        public InputHandle_t(ulong value) => this.m_InputHandle = value;

        public override string ToString() => this.m_InputHandle.ToString();

        public override bool Equals(object other) => other is InputHandle_t && this == (InputHandle_t)other;

        public override int GetHashCode() => this.m_InputHandle.GetHashCode();

        public static bool operator ==(InputHandle_t x, InputHandle_t y) => x.m_InputHandle == y.m_InputHandle;

        public static bool operator !=(InputHandle_t x, InputHandle_t y) => !(x == y);

        public static explicit operator InputHandle_t(ulong value) => new InputHandle_t(value);

        public static explicit operator ulong(InputHandle_t that) => that.m_InputHandle;

        public bool Equals(InputHandle_t other) => this.m_InputHandle == other.m_InputHandle;

        public int CompareTo(InputHandle_t other) => this.m_InputHandle.CompareTo(other.m_InputHandle);
    }
}

#endif // !DISABLESTEAMWORKS
