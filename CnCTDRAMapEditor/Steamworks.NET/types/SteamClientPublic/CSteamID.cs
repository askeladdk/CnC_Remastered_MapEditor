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
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 4)]
    public struct CSteamID : System.IEquatable<CSteamID>, System.IComparable<CSteamID> {
        public static readonly CSteamID Nil = new CSteamID();
        public static readonly CSteamID OutofDateGS = new CSteamID(new AccountID_t(0), 0, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);
        public static readonly CSteamID LanModeGS = new CSteamID(new AccountID_t(0), 0, EUniverse.k_EUniversePublic, EAccountType.k_EAccountTypeInvalid);
        public static readonly CSteamID NotInitYetGS = new CSteamID(new AccountID_t(1), 0, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);
        public static readonly CSteamID NonSteamGS = new CSteamID(new AccountID_t(2), 0, EUniverse.k_EUniverseInvalid, EAccountType.k_EAccountTypeInvalid);
        public ulong m_SteamID;

        public CSteamID(AccountID_t unAccountID, EUniverse eUniverse, EAccountType eAccountType) {
            this.m_SteamID = 0;
            this.Set(unAccountID, eUniverse, eAccountType);
        }

        public CSteamID(AccountID_t unAccountID, uint unAccountInstance, EUniverse eUniverse, EAccountType eAccountType) {
            this.m_SteamID = 0;
#if _SERVER && Assert
		Assert( ! ( ( EAccountType.k_EAccountTypeIndividual == eAccountType ) && ( unAccountInstance > k_unSteamUserWebInstance ) ) );	// enforce that for individual accounts, instance is always 1
#endif // _SERVER
            this.InstancedSet(unAccountID, unAccountInstance, eUniverse, eAccountType);
        }

        public CSteamID(ulong ulSteamID) => this.m_SteamID = ulSteamID;

        public void Set(AccountID_t unAccountID, EUniverse eUniverse, EAccountType eAccountType) {
            this.SetAccountID(unAccountID);
            this.SetEUniverse(eUniverse);
            this.SetEAccountType(eAccountType);

            if(eAccountType == EAccountType.k_EAccountTypeClan || eAccountType == EAccountType.k_EAccountTypeGameServer) {
                this.SetAccountInstance(0);
            } else {
                // by default we pick the desktop instance
                this.SetAccountInstance(Constants.k_unSteamUserDesktopInstance);
            }
        }

        public void InstancedSet(AccountID_t unAccountID, uint unInstance, EUniverse eUniverse, EAccountType eAccountType) {
            this.SetAccountID(unAccountID);
            this.SetEUniverse(eUniverse);
            this.SetEAccountType(eAccountType);
            this.SetAccountInstance(unInstance);
        }

        public void Clear() => this.m_SteamID = 0;

        public void CreateBlankAnonLogon(EUniverse eUniverse) {
            this.SetAccountID(new AccountID_t(0));
            this.SetEUniverse(eUniverse);
            this.SetEAccountType(EAccountType.k_EAccountTypeAnonGameServer);
            this.SetAccountInstance(0);
        }

        public void CreateBlankAnonUserLogon(EUniverse eUniverse) {
            this.SetAccountID(new AccountID_t(0));
            this.SetEUniverse(eUniverse);
            this.SetEAccountType(EAccountType.k_EAccountTypeAnonUser);
            this.SetAccountInstance(0);
        }

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous game server login that will be filled in?
        //-----------------------------------------------------------------------------
        public bool BBlankAnonAccount() => this.GetAccountID() == new AccountID_t(0) && this.BAnonAccount() && this.GetUnAccountInstance() == 0;

        //-----------------------------------------------------------------------------
        // Purpose: Is this a game server account id?  (Either persistent or anonymous)
        //-----------------------------------------------------------------------------
        public bool BGameServerAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeGameServer || this.GetEAccountType() == EAccountType.k_EAccountTypeAnonGameServer;

        //-----------------------------------------------------------------------------
        // Purpose: Is this a persistent (not anonymous) game server account id?
        //-----------------------------------------------------------------------------
        public bool BPersistentGameServerAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeGameServer;

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous game server account id?
        //-----------------------------------------------------------------------------
        public bool BAnonGameServerAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeAnonGameServer;

        //-----------------------------------------------------------------------------
        // Purpose: Is this a content server account id?
        //-----------------------------------------------------------------------------
        public bool BContentServerAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeContentServer;

        //-----------------------------------------------------------------------------
        // Purpose: Is this a clan account id?
        //-----------------------------------------------------------------------------
        public bool BClanAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeClan;

        //-----------------------------------------------------------------------------
        // Purpose: Is this a chat account id?
        //-----------------------------------------------------------------------------
        public bool BChatAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeChat;

        //-----------------------------------------------------------------------------
        // Purpose: Is this a chat account id?
        //-----------------------------------------------------------------------------
        public bool IsLobby() => (this.GetEAccountType() == EAccountType.k_EAccountTypeChat)
                && (this.GetUnAccountInstance() & (int)EChatSteamIDInstanceFlags.k_EChatInstanceFlagLobby) != 0;

        //-----------------------------------------------------------------------------
        // Purpose: Is this an individual user account id?
        //-----------------------------------------------------------------------------
        public bool BIndividualAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeIndividual || this.GetEAccountType() == EAccountType.k_EAccountTypeConsoleUser;

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous account?
        //-----------------------------------------------------------------------------
        public bool BAnonAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeAnonUser || this.GetEAccountType() == EAccountType.k_EAccountTypeAnonGameServer;

        //-----------------------------------------------------------------------------
        // Purpose: Is this an anonymous user account? ( used to create an account or reset a password )
        //-----------------------------------------------------------------------------
        public bool BAnonUserAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeAnonUser;

        //-----------------------------------------------------------------------------
        // Purpose: Is this a faked up Steam ID for a PSN friend account?
        //-----------------------------------------------------------------------------
        public bool BConsoleUserAccount() => this.GetEAccountType() == EAccountType.k_EAccountTypeConsoleUser;

        public void SetAccountID(AccountID_t other) => this.m_SteamID = (this.m_SteamID & ~(0xFFFFFFFFul << 0)) | (((ulong)(other) & 0xFFFFFFFFul) << 0);

        public void SetAccountInstance(uint other) => this.m_SteamID = (this.m_SteamID & ~(0xFFFFFul << 32)) | ((other & 0xFFFFFul) << 32);

        // This is a non standard/custom function not found in C++ Steamworks
        public void SetEAccountType(EAccountType other) => this.m_SteamID = (this.m_SteamID & ~(0xFul << 52)) | (((ulong)(other) & 0xFul) << 52);

        public void SetEUniverse(EUniverse other) => this.m_SteamID = (this.m_SteamID & ~(0xFFul << 56)) | (((ulong)(other) & 0xFFul) << 56);

        public void ClearIndividualInstance() {
            if(this.BIndividualAccount())
                this.SetAccountInstance(0);
        }

        public bool HasNoIndividualInstance() => this.BIndividualAccount() && (this.GetUnAccountInstance() == 0);

        public AccountID_t GetAccountID() => new AccountID_t((uint)(this.m_SteamID & 0xFFFFFFFFul));

        public uint GetUnAccountInstance() => (uint)((this.m_SteamID >> 32) & 0xFFFFFul);

        public EAccountType GetEAccountType() => (EAccountType)((this.m_SteamID >> 52) & 0xFul);

        public EUniverse GetEUniverse() => (EUniverse)((this.m_SteamID >> 56) & 0xFFul);

        public bool IsValid() {
            if(this.GetEAccountType() <= EAccountType.k_EAccountTypeInvalid || this.GetEAccountType() >= EAccountType.k_EAccountTypeMax)
                return false;

            if(this.GetEUniverse() <= EUniverse.k_EUniverseInvalid || this.GetEUniverse() >= EUniverse.k_EUniverseMax)
                return false;

            if(this.GetEAccountType() == EAccountType.k_EAccountTypeIndividual) {
                if(this.GetAccountID() == new AccountID_t(0) || this.GetUnAccountInstance() > Constants.k_unSteamUserWebInstance)
                    return false;
            }

            if(this.GetEAccountType() == EAccountType.k_EAccountTypeClan) {
                if(this.GetAccountID() == new AccountID_t(0) || this.GetUnAccountInstance() != 0)
                    return false;
            }

            if(this.GetEAccountType() == EAccountType.k_EAccountTypeGameServer) {
                if(this.GetAccountID() == new AccountID_t(0))
                    return false;
                // Any limit on instances?  We use them for local users and bots
            }
            return true;
        }

        #region Overrides
        public override string ToString() => this.m_SteamID.ToString();

        public override bool Equals(object other) => other is CSteamID && this == (CSteamID)other;

        public override int GetHashCode() => this.m_SteamID.GetHashCode();

        public static bool operator ==(CSteamID x, CSteamID y) => x.m_SteamID == y.m_SteamID;

        public static bool operator !=(CSteamID x, CSteamID y) => !(x == y);

        public static explicit operator CSteamID(ulong value) => new CSteamID(value);
        public static explicit operator ulong(CSteamID that) => that.m_SteamID;

        public bool Equals(CSteamID other) => this.m_SteamID == other.m_SteamID;

        public int CompareTo(CSteamID other) => this.m_SteamID.CompareTo(other.m_SteamID);
        #endregion Overrides
    }
}

#endif // !DISABLESTEAMWORKS
