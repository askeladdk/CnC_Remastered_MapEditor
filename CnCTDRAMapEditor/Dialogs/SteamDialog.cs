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
using MobiusEditor.Controls;
using MobiusEditor.Interface;
using MobiusEditor.Utility;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace MobiusEditor.Dialogs {
    public partial class SteamDialog : Form {
        private static readonly string PreviewDirectory = Path.Combine(Path.GetTempPath(), "CnCRCMapEditor");

        private readonly IGamePlugin plugin;
        private readonly Timer statusUpdateTimer = new Timer();

        public SteamDialog(IGamePlugin plugin) {
            this.plugin = plugin;

            this.InitializeComponent();

            this.visibilityComboBox.DataSource = new[]
            {
                new { Name = "Public", Value = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic },
                new { Name = "Friends Only", Value = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly },
                new { Name = "Private", Value = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate }
            };

            this.statusUpdateTimer.Interval = 500;
            this.statusUpdateTimer.Tick += this.StatusUpdateTimer_Tick;

            Disposed += (o, e) => { (this.previewTxt.Tag as Image)?.Dispose(); };
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            this.titleTxt.Text = this.plugin.Map.SteamSection.Title;
            this.descriptionTxt.Text = this.plugin.Map.SteamSection.Description;
            this.previewTxt.Text = this.plugin.Map.SteamSection.PreviewFile;
            this.visibilityComboBox.SelectedValue = this.plugin.Map.SteamSection.Visibility;

            this.btnPublishMap.SplitWidth = (this.plugin.Map.SteamSection.PublishedFileId != PublishedFileId_t.Invalid.m_PublishedFileId) ? MenuButton.DefaultSplitWidth : 0;

            Directory.CreateDirectory(PreviewDirectory);
            var previewPath = Path.Combine(PreviewDirectory, "Minimap.png");
            this.plugin.Map.GenerateWorkshopPreview().ToBitmap().Save(previewPath, ImageFormat.Png);

            if(this.plugin.Map.BasicSection.SoloMission) {
                var soloBannerPath = Path.Combine(PreviewDirectory, "SoloBanner.png");
                Properties.Resources.UI_CustomMissionPreviewDefault.Save(soloBannerPath, ImageFormat.Png);
                this.previewTxt.Text = soloBannerPath;
            } else {
                this.previewTxt.Text = previewPath;
            }

            this.imageTooltip.SetToolTip(this.previewTxt, "Preview.png");

            this.statusUpdateTimer.Start();

            this.UpdateControls();
        }

        private void StatusUpdateTimer_Tick(object sender, EventArgs e) {
            var status = SteamworksUGC.CurrentOperation?.Status;
            if(!string.IsNullOrEmpty(status)) {
                this.statusLbl.Text = status;
            }
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);

            this.statusUpdateTimer.Stop();
            this.statusUpdateTimer.Dispose();
        }

        protected virtual void OnPublishSuccess() {
            this.statusLbl.Text = "Done.";
            this.EnableControls(true);
        }

        protected virtual void OnOperationFailed(string status) {
            this.statusLbl.Text = status;
            this.EnableControls(true);
        }

        private void EnableControls(bool enable) {
            this.titleTxt.Enabled = enable;
            this.visibilityComboBox.Enabled = enable;
            this.previewTxt.Enabled = enable;
            this.previewBtn.Enabled = enable;
            this.descriptionTxt.Enabled = enable;
            this.btnPublishMap.Enabled = enable;
            this.btnClose.Enabled = enable;
        }

        private void btnGoToSteam_Click(object sender, EventArgs e) {
            var workshopUrl = SteamworksUGC.WorkshopURL;
            if(!string.IsNullOrEmpty(workshopUrl)) {
                Process.Start(workshopUrl);
            }
        }

        private void btnPublishMap_Click(object sender, EventArgs e) {
            if(string.IsNullOrEmpty(this.plugin.Map.BasicSection.Name)) {
                this.plugin.Map.BasicSection.Name = this.titleTxt.Text;
            }

            if(string.IsNullOrEmpty(this.plugin.Map.BasicSection.Author)) {
                this.plugin.Map.BasicSection.Author = SteamFriends.GetPersonaName();
            }

            this.plugin.Map.SteamSection.PreviewFile = this.previewTxt.Text;
            this.plugin.Map.SteamSection.Title = this.titleTxt.Text;
            this.plugin.Map.SteamSection.Description = this.descriptionTxt.Text;
            this.plugin.Map.SteamSection.Visibility = (ERemoteStoragePublishedFileVisibility)this.visibilityComboBox.SelectedValue;

            var tempPath = Path.Combine(Path.GetTempPath(), "CnCRCMapEditorPublishUGC");
            Directory.CreateDirectory(tempPath);
            foreach(var file in new DirectoryInfo(tempPath).EnumerateFiles())
                file.Delete();

            var pgmPath = Path.Combine(tempPath, "MAPDATA.PGM");
            this.plugin.Save(pgmPath, FileType.PGM);

            var tags = new List<string>();
            switch(this.plugin.GameType) {
            case GameType.TiberianDawn:
                tags.Add("TD");
                break;
            case GameType.RedAlert:
                tags.Add("RA");
                break;
            }

            if(this.plugin.Map.BasicSection.SoloMission) {
                tags.Add("SinglePlayer");
            } else {
                tags.Add("MultiPlayer");
            }

            if(SteamworksUGC.PublishUGC(tempPath, this.plugin.Map.SteamSection, tags, this.OnPublishSuccess, this.OnOperationFailed)) {
                this.statusLbl.Text = SteamworksUGC.CurrentOperation.Status;
                this.EnableControls(false);
            }
        }

        private void previewBtn_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog {
                AutoUpgradeEnabled = false,
                RestoreDirectory = true,
                Filter = "Preview Files (*.png)|*.png",
                CheckFileExists = true,
                InitialDirectory = Path.GetDirectoryName(this.previewTxt.Text),
                FileName = Path.GetFileName(this.previewTxt.Text)
            };
            if(!string.IsNullOrEmpty(this.previewTxt.Text)) {
                ofd.FileName = this.previewTxt.Text;
            }
            if(ofd.ShowDialog() == DialogResult.OK) {
                this.previewTxt.Text = ofd.FileName;
            }
        }

        private void publishAsNewToolStripMenuItem_Click(object sender, EventArgs e) {
            this.plugin.Map.SteamSection.PublishedFileId = PublishedFileId_t.Invalid.m_PublishedFileId;
            this.btnPublishMap.PerformClick();
        }

        private void previewTxt_TextChanged(object sender, EventArgs e) {
            try {
                (this.previewTxt.Tag as Image)?.Dispose();

                Bitmap preview = null;
                using(var b = new Bitmap(this.previewTxt.Text)) {
                    preview = new Bitmap(b.Width, b.Height, b.PixelFormat);
                    using(var g = Graphics.FromImage(preview)) {
                        g.DrawImage(b, Point.Empty);
                        g.Flush();
                    }
                }

                this.previewTxt.Tag = preview;
            } catch(Exception) {
                this.previewTxt.Tag = null;
            }

            this.UpdateControls();
        }

        private void descriptionTxt_TextChanged(object sender, EventArgs e) => this.UpdateControls();

        private void UpdateControls() => this.btnPublishMap.Enabled = (this.previewTxt.Tag != null) && !string.IsNullOrEmpty(this.descriptionTxt.Text);
    }
}
