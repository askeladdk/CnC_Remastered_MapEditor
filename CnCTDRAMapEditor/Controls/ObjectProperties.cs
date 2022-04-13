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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class ObjectProperties : UserControl {
        private bool isMockObject;

        public IGamePlugin Plugin {
            get; private set;
        }

        private INotifyPropertyChanged obj;
        public INotifyPropertyChanged Object {
            get => this.obj;
            set {
                if(this.obj != value) {
                    if(this.obj != null) {
                        this.obj.PropertyChanged -= this.Obj_PropertyChanged;
                    }

                    this.obj = value;

                    if(this.obj != null) {
                        this.obj.PropertyChanged += this.Obj_PropertyChanged;
                    }

                    this.Rebind();
                }
            }
        }

        public ObjectProperties() => this.InitializeComponent();

        public void Initialize(IGamePlugin plugin, bool isMockObject) {
            this.isMockObject = isMockObject;

            this.Plugin = plugin;
            plugin.Map.Triggers.CollectionChanged += this.Triggers_CollectionChanged;

            this.houseComboBox.DataSource = plugin.Map.Houses.Select(t => new TypeItem<HouseType>(t.Type.Name, t.Type)).ToArray();
            this.missionComboBox.DataSource = plugin.Map.MissionTypes;

            this.UpdateDataSource();

            Disposed += (sender, e) => {
                this.Object = null;
                plugin.Map.Triggers.CollectionChanged -= this.Triggers_CollectionChanged;
            };
        }

        private void Triggers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => this.UpdateDataSource();

        private void UpdateDataSource() => this.triggerComboBox.DataSource = Trigger.None.Yield().Concat(this.Plugin.Map.Triggers.Select(t => t.Name).Distinct()).ToArray();

        private void Rebind() {
            this.houseComboBox.DataBindings.Clear();
            this.strengthNud.DataBindings.Clear();
            this.directionComboBox.DataBindings.Clear();
            this.missionComboBox.DataBindings.Clear();
            this.triggerComboBox.DataBindings.Clear();
            this.basePriorityNud.DataBindings.Clear();
            this.prebuiltCheckBox.DataBindings.Clear();
            this.sellableCheckBox.DataBindings.Clear();
            this.rebuildCheckBox.DataBindings.Clear();

            if(this.obj == null) {
                return;
            }

            switch(this.obj) {
            case Infantry infantry: {
                this.houseComboBox.Enabled = true;
                this.directionComboBox.DataSource = this.Plugin.Map.DirectionTypes
                            .Where(t => t.Facing != FacingType.None)
                            .Select(t => new TypeItem<DirectionType>(t.Name, t)).ToArray();

                this.missionComboBox.DataBindings.Add("SelectedItem", this.obj, "Mission");
                this.missionLabel.Visible = this.missionComboBox.Visible = true;
                this.basePriorityLabel.Visible = this.basePriorityNud.Visible = false;
                this.prebuiltCheckBox.Visible = false;
                this.sellableCheckBox.Visible = false;
                this.rebuildCheckBox.Visible = false;
            }
            break;
            case Unit unit: {
                this.houseComboBox.Enabled = true;
                this.directionComboBox.DataSource = this.Plugin.Map.DirectionTypes.Select(t => new TypeItem<DirectionType>(t.Name, t)).ToArray();
                this.missionComboBox.DataBindings.Add("SelectedItem", this.obj, "Mission");
                this.missionLabel.Visible = this.missionComboBox.Visible = true;
                this.basePriorityLabel.Visible = this.basePriorityNud.Visible = false;
                this.prebuiltCheckBox.Visible = false;
                this.sellableCheckBox.Visible = false;
                this.rebuildCheckBox.Visible = false;
            }
            break;
            case Building building: {
                this.houseComboBox.Enabled = building.IsPrebuilt;
                this.directionComboBox.DataSource = this.Plugin.Map.DirectionTypes.Select(t => new TypeItem<DirectionType>(t.Name, t)).ToArray();
                this.directionComboBox.Visible = (building.Type != null) && building.Type.HasTurret;
                this.missionLabel.Visible = this.missionComboBox.Visible = false;
                this.basePriorityLabel.Visible = this.basePriorityNud.Visible = true;
                this.prebuiltCheckBox.Visible = true;
                this.prebuiltCheckBox.Enabled = building.BasePriority >= 0;

                this.basePriorityNud.DataBindings.Add("Value", this.obj, "BasePriority");
                this.prebuiltCheckBox.DataBindings.Add("Checked", this.obj, "IsPrebuilt");

                switch(this.Plugin.GameType) {
                case GameType.TiberianDawn: {
                    this.sellableCheckBox.Visible = false;
                    this.rebuildCheckBox.Visible = false;
                }
                break;
                case GameType.RedAlert: {
                    this.sellableCheckBox.DataBindings.Add("Checked", this.obj, "Sellable");
                    this.rebuildCheckBox.DataBindings.Add("Checked", this.obj, "Rebuild");
                    this.sellableCheckBox.Visible = true;
                    this.rebuildCheckBox.Visible = true;
                }
                break;
                }
            }
            break;
            }

            this.houseComboBox.DataBindings.Add("SelectedValue", this.obj, "House");
            this.strengthNud.DataBindings.Add("Value", this.obj, "Strength");
            this.directionComboBox.DataBindings.Add("SelectedValue", this.obj, "Direction");
            this.triggerComboBox.DataBindings.Add("SelectedItem", this.obj, "Trigger");
        }

        private void Obj_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch(e.PropertyName) {
            case "Type": {
                this.Rebind();
            }
            break;
            case "BasePriority": {
                if(this.obj is Building building) {
                    this.prebuiltCheckBox.Enabled = building.BasePriority >= 0;
                }
            }
            break;
            case "IsPrebuilt": {
                if(this.obj is Building building) {
                    if(!building.IsPrebuilt) {
                        var basePlayer = this.Plugin.Map.HouseTypes.Where(h => h.Equals(this.Plugin.Map.BasicSection.BasePlayer)).FirstOrDefault() ?? this.Plugin.Map.HouseTypes.First();
                        building.House = basePlayer;
                    }
                    this.houseComboBox.Enabled = building.IsPrebuilt;
                }
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

        private void checkBox_CheckedChanged(object sender, EventArgs e) {
            foreach(Binding binding in (sender as CheckBox).DataBindings) {
                binding.WriteValue();
            }
        }
    }

    public class ObjectPropertiesPopup : ToolStripDropDown {
        private readonly ToolStripControlHost host;

        public ObjectProperties ObjectProperties {
            get; private set;
        }

        public ObjectPropertiesPopup(IGamePlugin plugin, INotifyPropertyChanged obj) {
            this.ObjectProperties = new ObjectProperties();
            this.ObjectProperties.Initialize(plugin, false);
            this.ObjectProperties.Object = obj;

            this.host = new ToolStripControlHost(this.ObjectProperties);
            this.Padding = this.Margin = this.host.Padding = this.host.Margin = Padding.Empty;
            this.MinimumSize = this.ObjectProperties.MinimumSize;
            this.ObjectProperties.MinimumSize = this.ObjectProperties.Size;
            this.MaximumSize = this.ObjectProperties.MaximumSize;
            this.ObjectProperties.MaximumSize = this.ObjectProperties.Size;
            this.Size = this.ObjectProperties.Size;
            this.Items.Add(this.host);
            this.ObjectProperties.Disposed += (sender, e) => {
                this.ObjectProperties = null;
                this.Dispose(true);
            };
        }

        protected override void OnClosed(ToolStripDropDownClosedEventArgs e) {
            base.OnClosed(e);

            this.ObjectProperties.Object = null;
        }
    }
}
