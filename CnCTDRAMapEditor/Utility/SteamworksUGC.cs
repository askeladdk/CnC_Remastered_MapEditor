//
// Copyright 2020 Electronic Arts Inc.
//
// The Command & Conquer Map Editor and corresponding source code is free 
// software: you can redistribute it and/or modify it under the terms of 
// the GNU General Public License as published by the Free Software Foundation, 
// either version 3 of the License, or (at your option) any later version.

// The Command & Conquer Map Editor and corresponding source code is distributed 
// in the hope that it will be useful, but with permitted additional restrictions 
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT 
// distributed with this program. You should have received a copy of the 
// GNU General Public License along with permitted additional restrictions 
// with this program. If not, see https://github.com/electronicarts/CnC_Remastered_Collection
using MobiusEditor.Model;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MobiusEditor.Utility {
    public interface ISteamworksOperation : IDisposable {
        bool Done {
            get;
        }

        bool Failed {
            get;
        }

        string Status {
            get;
        }

        void OnSuccess();

        void OnFailed();
    }

    public class SteamworksUGCPublishOperation : ISteamworksOperation {
        private readonly string ugcPath;
        private readonly IList<string> tags;
        private readonly Action onSuccess;
        private readonly Action<string> onFailed;

        private CallResult<CreateItemResult_t> createItemResult;
        private CallResult<SubmitItemUpdateResult_t> submitItemUpdateResult;
        private readonly SteamSection steamSection = new SteamSection();

        public bool Done => !(this.createItemResult?.IsActive() ?? false) && !(this.submitItemUpdateResult?.IsActive() ?? false);

        public bool Failed {
            get; private set;
        }

        public string Status {
            get; private set;
        }

        public SteamworksUGCPublishOperation(string ugcPath, SteamSection steamSection, IList<string> tags, Action onSuccess, Action<string> onFailed) {
            this.ugcPath = ugcPath;
            this.steamSection = steamSection;
            this.tags = tags;
            this.onSuccess = onSuccess;
            this.onFailed = onFailed;

            if(steamSection.PublishedFileId == PublishedFileId_t.Invalid.m_PublishedFileId) {
                this.CreateUGCItem();
            } else {
                this.UpdateUGCItem();
            }

            this.Status = "Publishing UGC...";
        }

        public void OnSuccess() => this.onSuccess();

        public void OnFailed() => this.onFailed(this.Status);

        private void CreateUGCItem() {
            var steamAPICall = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
            if(steamAPICall == SteamAPICall_t.Invalid) {
                this.Failed = true;
                this.Status = "Publishing failed.";
                return;
            }

            this.createItemResult = CallResult<CreateItemResult_t>.Create(this.OnCreateItemResult);
            this.createItemResult.Set(steamAPICall);
        }

        private void UpdateUGCItem() {
            var updateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), new PublishedFileId_t(this.steamSection.PublishedFileId));
            if(updateHandle == UGCUpdateHandle_t.Invalid) {
                this.Failed = true;
                this.Status = "Publishing failed.";
                return;
            }

            var success = true;
            success = success && SteamUGC.SetItemContent(updateHandle, this.ugcPath);
            success = success && SteamUGC.SetItemPreview(updateHandle, this.steamSection.PreviewFile);
            success = success && SteamUGC.SetItemUpdateLanguage(updateHandle, "English");
            success = success && SteamUGC.SetItemTitle(updateHandle, this.steamSection.Title);
            success = success && SteamUGC.SetItemDescription(updateHandle, this.steamSection.Description);
            success = success && SteamUGC.SetItemVisibility(updateHandle, this.steamSection.Visibility);
            success = success && SteamUGC.SetItemTags(updateHandle, this.tags);
            if(!success) {
                this.Failed = true;
                this.Status = "Publishing failed.";
                return;
            }

            var steamAPICall = SteamUGC.SubmitItemUpdate(updateHandle, "");
            if(steamAPICall == SteamAPICall_t.Invalid) {
                this.Failed = true;
                this.Status = "Publishing failed.";
                return;
            }

            this.submitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(this.OnSubmitItemUpdateResult);
            this.submitItemUpdateResult.Set(steamAPICall);
        }

        private void OnCreateItemResult(CreateItemResult_t callback, bool ioFailure) {
            if(ioFailure) {
                this.Failed = true;
                this.Status = "Publishing failed.";
                return;
            }

            switch(callback.m_eResult) {
            case EResult.k_EResultOK:
                this.steamSection.PublishedFileId = callback.m_nPublishedFileId.m_PublishedFileId;
                this.UpdateUGCItem();
                break;
            case EResult.k_EResultFileNotFound:
                this.Failed = true;
                this.Status = "UGC not found.";
                break;
            case EResult.k_EResultNotLoggedOn:
                this.Failed = true;
                this.Status = "Not logged on.";
                break;
            default:
                this.Failed = true;
                this.Status = "Publishing failed.";
                break;
            }
        }

        private void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t callback, bool ioFailure) {
            if(ioFailure) {
                this.Failed = true;
                this.Status = "Publishing failed.";
                return;
            }

            switch(callback.m_eResult) {
            case EResult.k_EResultOK:
                this.Status = "Done.";
                this.steamSection.PublishedFileId = callback.m_nPublishedFileId.m_PublishedFileId;
                break;
            case EResult.k_EResultFileNotFound:
                this.Failed = true;
                this.Status = "UGC not found.";
                break;
            case EResult.k_EResultLimitExceeded:
                this.Failed = true;
                this.Status = "Size limit exceeded.";
                break;
            default:
                this.Failed = true;
                this.Status = string.Format("Publishing failed. ({0})", callback.m_eResult);
                break;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if(!this.disposedValue) {
                if(disposing) {
                    this.createItemResult?.Dispose();
                    this.submitItemUpdateResult?.Dispose();
                }
                this.disposedValue = true;
            }
        }

        public void Dispose() => this.Dispose(true);
        #endregion
    }

    public static class SteamworksUGC {
        public static bool IsInit {
            get; private set;
        }

        public static ISteamworksOperation CurrentOperation {
            get; private set;
        }

        public static string WorkshopURL {
            get {
                var app_id = IsInit ? SteamUtils.GetAppID() : AppId_t.Invalid;
                if(app_id == AppId_t.Invalid) {
                    return string.Empty;
                }
                return string.Format("http://steamcommunity.com/app/{0}/workshop/", app_id.ToString());
            }
        }

        public static bool IsSteamBuild => File.Exists("steam_appid.txt");

        private static Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;

        public static bool Init() {
            if(IsInit) {
                return true;
            }

            if(!IsSteamBuild) {
                return false;
            }

            if(!Packsize.Test()) {
                return false;
            }

            if(!DllCheck.Test()) {
                return false;
            }

            if(!SteamAPI.Init()) {
                return false;
            }

            SteamClient.SetWarningMessageHook(new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook));

            GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);

            IsInit = true;
            return IsInit;
        }

        public static void Shutdown() {
            if(IsInit) {
                GameLobbyJoinRequested?.Dispose();
                GameLobbyJoinRequested = null;

                CurrentOperation?.Dispose();
                CurrentOperation = null;

                SteamAPI.Shutdown();
                IsInit = false;
            }
        }

        public static void Service() {
            SteamAPI.RunCallbacks();

            if(CurrentOperation?.Done ?? false) {
                if(CurrentOperation.Failed) {
                    CurrentOperation.OnFailed();
                } else {
                    CurrentOperation.OnSuccess();
                }
                CurrentOperation.Dispose();
                CurrentOperation = null;
            }
        }

        public static bool PublishUGC(string ugcPath, SteamSection steamSection, IList<string> tags, Action onSuccess, Action<string> onFailed) {
            if(CurrentOperation != null) {
                return false;
            }

            CurrentOperation = new SteamworksUGCPublishOperation(ugcPath, steamSection, tags, onSuccess, onFailed);

            return true;
        }

        private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText) => Debug.WriteLine(pchDebugText);

        private static void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t data) => MessageBox.Show("You cannot accept an invitation to a multiplayer game while using the map editor.", "Steam", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
