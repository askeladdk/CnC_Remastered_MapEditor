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
using MobiusEditor.Interface;
using MobiusEditor.Model;
using MobiusEditor.Utility;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class TerrainProperties : UserControl {
        private bool isMockObject;

        public IGamePlugin Plugin {
            get; private set;
        }

        private Terrain terrain;
        public Terrain Terrain {
            get => this.terrain;
            set {
                if(this.terrain != value) {
                    this.terrain = value;
                    this.Rebind();
                }
            }
        }

        public TerrainProperties() {
            this.InitializeComponent();
        }

        public void Initialize(IGamePlugin plugin, bool isMockObject) {
            this.isMockObject = isMockObject;

            this.Plugin = plugin;
            plugin.Map.Triggers.CollectionChanged += this.Triggers_CollectionChanged;

            this.UpdateDataSource();

            Disposed += (sender, e) => {
                this.Terrain = null;
                plugin.Map.Triggers.CollectionChanged -= this.Triggers_CollectionChanged;
            };
        }

        private void Triggers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            this.UpdateDataSource();
        }

        private void UpdateDataSource() {
            this.triggerComboBox.DataSource = Trigger.None.Yield().Concat(this.Plugin.Map.Triggers.Select(t => t.Name).Distinct()).ToArray();
        }

        private void Rebind() {
            this.triggerComboBox.DataBindings.Clear();

            if(this.terrain == null) {
                return;
            }

            this.triggerComboBox.DataBindings.Add("SelectedItem", this.terrain, "Trigger");
        }

        private void Obj_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch(e.PropertyName) {
            case "Type": {
                this.Rebind();
            }
            break;
            }

            if(!this.isMockObject) {
                this.Plugin.Dirty = true;
            }
        }

        private void comboBox_SelectedValueChanged(object sender, EventArgs e) {
            foreach(Binding binding in (sender as ComboBox).DataBindings) {
                binding.WriteValue();
            }
        }

        private void nud_ValueChanged(object sender, EventArgs e) {
            foreach(Binding binding in (sender as NumericUpDown).DataBindings) {
                binding.WriteValue();
            }
        }
    }

    public class TerrainPropertiesPopup : ToolStripDropDown {
        private readonly ToolStripControlHost host;

        public TerrainProperties TerrainProperties {
            get; private set;
        }

        public TerrainPropertiesPopup(IGamePlugin plugin, Terrain terrain) {
            this.TerrainProperties = new TerrainProperties();
            this.TerrainProperties.Initialize(plugin, false);
            this.TerrainProperties.Terrain = terrain;

            this.host = new ToolStripControlHost(this.TerrainProperties);
            this.Padding = this.Margin = this.host.Padding = this.host.Margin = Padding.Empty;
            this.MinimumSize = this.TerrainProperties.MinimumSize;
            this.TerrainProperties.MinimumSize = this.TerrainProperties.Size;
            this.MaximumSize = this.TerrainProperties.MaximumSize;
            this.TerrainProperties.MaximumSize = this.TerrainProperties.Size;
            this.Size = this.TerrainProperties.Size;
            this.Items.Add(this.host);
            this.TerrainProperties.Disposed += (sender, e) => {
                this.TerrainProperties = null;
                this.Dispose(true);
            };
        }

        protected override void OnClosed(ToolStripDropDownClosedEventArgs e) {
            base.OnClosed(e);

            this.TerrainProperties.Terrain = null;
        }
    }
}
