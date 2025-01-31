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
    public struct ScreenshotHandle : System.IEquatable<ScreenshotHandle>, System.IComparable<ScreenshotHandle> {
        public static readonly ScreenshotHandle Invalid = new ScreenshotHandle(0);
        public uint m_ScreenshotHandle;

        public ScreenshotHandle(uint value) => this.m_ScreenshotHandle = value;

        public override string ToString() => this.m_ScreenshotHandle.ToString();

        public override bool Equals(object other) => other is ScreenshotHandle && this == (ScreenshotHandle)other;

        public override int GetHashCode() => this.m_ScreenshotHandle.GetHashCode();

        public static bool operator ==(ScreenshotHandle x, ScreenshotHandle y) => x.m_ScreenshotHandle == y.m_ScreenshotHandle;

        public static bool operator !=(ScreenshotHandle x, ScreenshotHandle y) => !(x == y);

        public static explicit operator ScreenshotHandle(uint value) => new ScreenshotHandle(value);

        public static explicit operator uint(ScreenshotHandle that) => that.m_ScreenshotHandle;

        public bool Equals(ScreenshotHandle other) => this.m_ScreenshotHandle == other.m_ScreenshotHandle;

        public int CompareTo(ScreenshotHandle other) => this.m_ScreenshotHandle.CompareTo(other.m_ScreenshotHandle);
    }
}

#endif // !DISABLESTEAMWORKS
