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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MobiusEditor.Utility {
    public class MRU {
        private readonly RegistryKey registryKey;
        private readonly int maxFiles;

        private readonly List<FileInfo> files = new List<FileInfo>();

        private readonly ToolStripMenuItem menu;
        private readonly ToolStripMenuItem[] fileItems;

        public event EventHandler<FileInfo> FileSelected;

        public MRU(string registryPath, int maxFiles, ToolStripMenuItem menu) {
            var subKey = Registry.CurrentUser;
            foreach(var key in registryPath.Split('\\')) {
                subKey = subKey.CreateSubKey(key, true);
            }
            this.registryKey = subKey.CreateSubKey("MRU");

            this.maxFiles = maxFiles;
            this.menu = menu;
            this.menu.DropDownItems.Clear();

            this.fileItems = new ToolStripMenuItem[maxFiles];
            for(var i = 0; i < this.fileItems.Length; ++i) {
                var fileItem = this.fileItems[i] = new ToolStripMenuItem();
                fileItem.Visible = false;
                menu.DropDownItems.Add(fileItem);
            }

            this.LoadMRU();
            this.ShowMRU();
        }

        public void Add(FileInfo file) {
            this.files.RemoveAll(f => f.FullName == file.FullName);
            this.files.Insert(0, file);

            if(this.files.Count > this.maxFiles) {
                this.files.RemoveAt(this.files.Count - 1);
            }

            this.SaveMRU();
            this.ShowMRU();
        }

        public void Remove(FileInfo file) {
            this.files.RemoveAll(f => f.FullName == file.FullName);

            this.SaveMRU();
            this.ShowMRU();
        }

        private void LoadMRU() {
            this.files.Clear();
            for(var i = 0; i < this.maxFiles; ++i) {
                var fileName = this.registryKey.GetValue(i.ToString()) as string;
                if(!string.IsNullOrEmpty(fileName)) {
                    this.files.Add(new FileInfo(fileName));
                }
            }
        }

        private void SaveMRU() {
            for(var i = 0; i < this.files.Count; ++i) {
                this.registryKey.SetValue(i.ToString(), this.files[i].FullName);
            }
            for(var i = this.files.Count; i < this.maxFiles; ++i) {
                this.registryKey.DeleteValue(i.ToString(), false);
            }
        }

        private void ShowMRU() {
            for(var i = 0; i < this.files.Count; ++i) {
                var file = this.files[i];
                var fileItem = this.fileItems[i];

                fileItem.Text = string.Format("&{0} {1}", i + 1, file.FullName);
                fileItem.Tag = file;
                fileItem.Click -= this.FileItem_Click;
                fileItem.Click += this.FileItem_Click;
                fileItem.Visible = true;
            }
            for(var i = this.files.Count; i < this.maxFiles; ++i) {
                var fileItem = this.fileItems[i];

                fileItem.Visible = false;
                fileItem.Click -= this.FileItem_Click;
            }
            this.menu.Enabled = this.files.Count > 0;
        }

        private void FileItem_Click(object sender, EventArgs e) => FileSelected?.Invoke(this, (sender as ToolStripMenuItem).Tag as FileInfo);
    }
}
