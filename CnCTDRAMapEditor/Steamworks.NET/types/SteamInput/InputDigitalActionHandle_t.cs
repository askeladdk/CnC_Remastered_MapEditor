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
    public struct InputDigitalActionHandle_t : System.IEquatable<InputDigitalActionHandle_t>, System.IComparable<InputDigitalActionHandle_t> {
        public ulong m_InputDigitalActionHandle;

        public InputDigitalActionHandle_t(ulong value) => this.m_InputDigitalActionHandle = value;

        public override string ToString() => this.m_InputDigitalActionHandle.ToString();

        public override bool Equals(object other) => other is InputDigitalActionHandle_t && this == (InputDigitalActionHandle_t)other;

        public override int GetHashCode() => this.m_InputDigitalActionHandle.GetHashCode();

        public static bool operator ==(InputDigitalActionHandle_t x, InputDigitalActionHandle_t y) => x.m_InputDigitalActionHandle == y.m_InputDigitalActionHandle;

        public static bool operator !=(InputDigitalActionHandle_t x, InputDigitalActionHandle_t y) => !(x == y);

        public static explicit operator InputDigitalActionHandle_t(ulong value) => new InputDigitalActionHandle_t(value);

        public static explicit operator ulong(InputDigitalActionHandle_t that) => that.m_InputDigitalActionHandle;

        public bool Equals(InputDigitalActionHandle_t other) => this.m_InputDigitalActionHandle == other.m_InputDigitalActionHandle;

        public int CompareTo(InputDigitalActionHandle_t other) => this.m_InputDigitalActionHandle.CompareTo(other.m_InputDigitalActionHandle);
    }
}

#endif // !DISABLESTEAMWORKS
