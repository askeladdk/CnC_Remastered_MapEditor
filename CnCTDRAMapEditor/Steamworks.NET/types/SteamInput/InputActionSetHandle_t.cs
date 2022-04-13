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
    public struct InputActionSetHandle_t : System.IEquatable<InputActionSetHandle_t>, System.IComparable<InputActionSetHandle_t> {
        public ulong m_InputActionSetHandle;

        public InputActionSetHandle_t(ulong value) => this.m_InputActionSetHandle = value;

        public override string ToString() => this.m_InputActionSetHandle.ToString();

        public override bool Equals(object other) => other is InputActionSetHandle_t && this == (InputActionSetHandle_t)other;

        public override int GetHashCode() => this.m_InputActionSetHandle.GetHashCode();

        public static bool operator ==(InputActionSetHandle_t x, InputActionSetHandle_t y) => x.m_InputActionSetHandle == y.m_InputActionSetHandle;

        public static bool operator !=(InputActionSetHandle_t x, InputActionSetHandle_t y) => !(x == y);

        public static explicit operator InputActionSetHandle_t(ulong value) => new InputActionSetHandle_t(value);

        public static explicit operator ulong(InputActionSetHandle_t that) => that.m_InputActionSetHandle;

        public bool Equals(InputActionSetHandle_t other) => this.m_InputActionSetHandle == other.m_InputActionSetHandle;

        public int CompareTo(InputActionSetHandle_t other) => this.m_InputActionSetHandle.CompareTo(other.m_InputActionSetHandle);
    }
}

#endif // !DISABLESTEAMWORKS
