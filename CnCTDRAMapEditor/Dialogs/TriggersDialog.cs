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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MobiusEditor.Dialogs {
    public partial class TriggersDialog : Form {
        private readonly IGamePlugin plugin;
        private readonly int maxTriggers;

        private readonly List<Trigger> triggers;
        public IEnumerable<Trigger> Triggers => this.triggers;

        private ListViewItem SelectedItem => (this.triggersListView.SelectedItems.Count > 0) ? this.triggersListView.SelectedItems[0] : null;

        private Trigger SelectedTrigger => this.SelectedItem?.Tag as Trigger;

        public TriggersDialog(IGamePlugin plugin, int maxTriggers) {
            this.plugin = plugin;
            this.maxTriggers = maxTriggers;

            this.InitializeComponent();

            switch(plugin.GameType) {
            case GameType.TiberianDawn:
                this.existenceLabel.Text = "Loop";
                this.event1Label.Text = "Event";
                this.action1Label.Text = "Action";
                this.typeLabel.Visible = this.typeComboBox.Visible = false;
                this.event2Label.Visible = this.event2ComboBox.Visible = this.event2Flp.Visible = false;
                this.action2Label.Visible = this.action2ComboBox.Visible = this.action2Flp.Visible = false;
                break;
            case GameType.RedAlert:
                this.teamLabel.Visible = this.teamComboBox.Visible = false;
                break;
            }

            this.triggers = new List<Trigger>(plugin.Map.Triggers.Select(t => t.Clone()));

            this.triggersListView.BeginUpdate();
            {
                foreach(var trigger in this.triggers) {
                    var item = new ListViewItem(trigger.Name) {
                        Tag = trigger
                    };
                    this.triggersListView.Items.Add(item).ToolTipText = trigger.Name;
                }
            }
            this.triggersListView.EndUpdate();

            var existenceNames = Enum.GetNames(typeof(TriggerPersistantType));
            switch(plugin.GameType) {
            case GameType.TiberianDawn:
                existenceNames = new string[] { "No", "And", "Or" };
                break;
            case GameType.RedAlert:
                existenceNames = new string[] { "Temporary", "Semi-Constant", "Constant" };
                break;
            }

            var typeNames = new string[]
            {
                "E => A1 [+ A2]",
                "E1 && E2 => A1 [+ A2]",
                "E1 || E2 => A1 [+ A2]",
                "E1 => A1; E2 => A2",
            };

            this.houseComboBox.DataSource = "None".Yield().Concat(plugin.Map.Houses.Select(t => t.Type.Name)).ToArray();
            this.existenceComboBox.DataSource = Enum.GetValues(typeof(TriggerPersistantType)).Cast<int>()
                .Select(v => new { Name = existenceNames[v], Value = (TriggerPersistantType)v })
                .ToArray();
            this.typeComboBox.DataSource = Enum.GetValues(typeof(TriggerMultiStyleType)).Cast<int>()
                .Select(v => new { Name = typeNames[v], Value = (TriggerMultiStyleType)v })
                .ToArray();
            this.event1ComboBox.DataSource = plugin.Map.EventTypes.Where(t => !string.IsNullOrEmpty(t)).ToArray();
            this.event2ComboBox.DataSource = plugin.Map.EventTypes.Where(t => !string.IsNullOrEmpty(t)).ToArray();
            this.action1ComboBox.DataSource = plugin.Map.ActionTypes.Where(t => !string.IsNullOrEmpty(t)).ToArray();
            this.action2ComboBox.DataSource = plugin.Map.ActionTypes.Where(t => !string.IsNullOrEmpty(t)).ToArray();
            this.teamComboBox.DataSource = "None".Yield().Concat(plugin.Map.TeamTypes.Select(t => t.Name)).ToArray();

            this.triggersTableLayoutPanel.Visible = false;
        }

        private void triggersListView_SelectedIndexChanged(object sender, EventArgs e) {
            this.houseComboBox.DataBindings.Clear();
            this.existenceComboBox.DataBindings.Clear();
            this.typeComboBox.DataBindings.Clear();
            this.event1ComboBox.DataBindings.Clear();
            this.event2ComboBox.DataBindings.Clear();
            this.action1ComboBox.DataBindings.Clear();
            this.action2ComboBox.DataBindings.Clear();
            this.teamComboBox.DataBindings.Clear();

            if(this.SelectedTrigger != null) {
                this.houseComboBox.DataBindings.Add("SelectedItem", this.SelectedTrigger, "House");
                this.existenceComboBox.DataBindings.Add("SelectedValue", this.SelectedTrigger, "PersistantType");
                this.event1ComboBox.DataBindings.Add("SelectedItem", this.SelectedTrigger.Event1, "EventType");
                this.action1ComboBox.DataBindings.Add("SelectedItem", this.SelectedTrigger.Action1, "ActionType");

                this.UpdateTriggerControls(this.SelectedTrigger,
                    this.SelectedTrigger?.Event1, this.SelectedTrigger?.Action1,
                    this.event1ComboBox, this.event1Nud, this.event1ValueComboBox,
                    this.action1ComboBox, this.action1Nud, this.action1ValueComboBox);

                switch(this.plugin.GameType) {
                case GameType.TiberianDawn:
                    this.teamComboBox.DataBindings.Add("SelectedItem", this.SelectedTrigger.Action1, "Team");
                    break;
                case GameType.RedAlert:
                    this.typeComboBox.DataBindings.Add("SelectedValue", this.SelectedTrigger, "EventControl");
                    this.event2ComboBox.DataBindings.Add("SelectedItem", this.SelectedTrigger.Event2, "EventType");
                    this.action2ComboBox.DataBindings.Add("SelectedItem", this.SelectedTrigger.Action2, "ActionType");
                    this.UpdateTriggerControls(this.SelectedTrigger,
                        this.SelectedTrigger?.Event2, this.SelectedTrigger?.Action2,
                            this.event2ComboBox, this.event2Nud, this.event2ValueComboBox,
                            this.action2ComboBox, this.action2Nud, this.action2ValueComboBox);
                    break;
                }

                this.triggersTableLayoutPanel.Visible = true;
            } else {
                this.triggersTableLayoutPanel.Visible = false;
            }
        }

        private void teamTypesListView_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Right) {
                var hitTest = this.triggersListView.HitTest(e.Location);

                var canAdd = (hitTest.Item == null) && (this.triggersListView.Items.Count < this.maxTriggers);
                var canRemove = hitTest.Item != null;
                this.addTriggerToolStripMenuItem.Visible = canAdd;
                this.removeTriggerToolStripMenuItem.Visible = canRemove;

                if(canAdd || canRemove) {
                    this.triggersContextMenuStrip.Show(Cursor.Position);
                }
            }
        }

        private void teamTypesListView_KeyDown(object sender, KeyEventArgs e) {
            if((e.KeyData == Keys.F2) && (this.triggersListView.SelectedItems.Count > 0)) {
                this.triggersListView.SelectedItems[0].BeginEdit();
            }
        }

        private void addTriggerToolStripMenuItem_Click(object sender, EventArgs e) {
            var nameChars = Enumerable.Range(97, 26).Concat(Enumerable.Range(48, 10));

            var name = string.Empty;
            foreach(var nameChar in nameChars) {
                name = new string((char)nameChar, 4);
                if(!this.triggers.Where(t => t.Equals(name)).Any()) {
                    break;
                }
            }

            var trigger = new Trigger { Name = name, House = this.plugin.Map.HouseTypes.First().Name };
            var item = new ListViewItem(trigger.Name) {
                Tag = trigger
            };
            this.triggers.Add(trigger);
            this.triggersListView.Items.Add(item).ToolTipText = trigger.Name;

            item.Selected = true;
            item.BeginEdit();
        }

        private void removeTriggerToolStripMenuItem_Click(object sender, EventArgs e) {
            if(this.SelectedItem != null) {
                this.triggers.Remove(this.SelectedTrigger);
                this.triggersListView.Items.Remove(this.SelectedItem);
            }
        }

        private void teamTypesListView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            var maxLength = int.MaxValue;
            switch(this.plugin.GameType) {
            case GameType.TiberianDawn:
                maxLength = 4;
                break;
            case GameType.RedAlert:
                maxLength = 23;
                break;
            }

            if(string.IsNullOrEmpty(e.Label)) {
                e.CancelEdit = true;
            } else if(e.Label.Length > maxLength) {
                e.CancelEdit = true;
                MessageBox.Show(string.Format("Trigger name is longer than {0} characters.", maxLength), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if(this.triggers.Where(t => (t != this.SelectedTrigger) && t.Equals(e.Label)).Any()) {
                e.CancelEdit = true;
                MessageBox.Show(string.Format("Trigger with name '{0}' already exists", e.Label), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                this.SelectedTrigger.Name = e.Label;
                this.triggersListView.Items[e.Item].ToolTipText = this.SelectedTrigger.Name;
            }
        }

        private void typeComboBox_SelectedValueChanged(object sender, EventArgs e) {
            if(this.plugin.GameType == GameType.RedAlert) {
                var eventType = (TriggerMultiStyleType)this.typeComboBox.SelectedValue;
                this.event2Label.Visible = this.event2ComboBox.Visible = this.event2Flp.Visible = eventType != TriggerMultiStyleType.Only;
            }
        }

        private void trigger1ComboBox_SelectedIndexChanged(object sender, EventArgs e) => this.UpdateTriggerControls(this.SelectedTrigger,
                this.SelectedTrigger?.Event1, this.SelectedTrigger?.Action1,
                this.event1ComboBox, this.event1Nud, this.event1ValueComboBox,
                this.action1ComboBox, this.action1Nud, this.action1ValueComboBox);

        private void trigger2ComboBox_SelectedIndexChanged(object sender, EventArgs e) => this.UpdateTriggerControls(this.SelectedTrigger,
                this.SelectedTrigger?.Event2, this.SelectedTrigger?.Action2,
                this.event2ComboBox, this.event2Nud, this.event2ValueComboBox,
                this.action2ComboBox, this.action2Nud, this.action2ValueComboBox);

        private void UpdateTriggerControls(Trigger trigger, TriggerEvent triggerEvent, TriggerAction triggerAction, ComboBox eventComboBox, NumericUpDown eventNud, ComboBox eventValueComboBox, ComboBox actionComboBox, NumericUpDown actionNud, ComboBox actionValueComboBox) {
            eventNud.Visible = false;
            eventNud.DataBindings.Clear();
            eventValueComboBox.Visible = false;
            eventValueComboBox.DataBindings.Clear();
            eventValueComboBox.DataSource = null;
            eventValueComboBox.DisplayMember = null;
            eventValueComboBox.ValueMember = null;

            if(triggerEvent != null) {
                switch(this.plugin.GameType) {
                case GameType.TiberianDawn:
                    switch(eventComboBox.SelectedItem) {
                    case TiberianDawn.EventTypes.EVENT_TIME:
                    case TiberianDawn.EventTypes.EVENT_CREDITS:
                    case TiberianDawn.EventTypes.EVENT_NUNITS_DESTROYED:
                    case TiberianDawn.EventTypes.EVENT_NBUILDINGS_DESTROYED:
                        eventNud.Visible = true;
                        eventNud.DataBindings.Add("Value", triggerEvent, "Data");
                        break;
                    case TiberianDawn.EventTypes.EVENT_BUILD:
                        eventValueComboBox.Visible = true;
                        eventValueComboBox.DisplayMember = "Name";
                        eventValueComboBox.ValueMember = "Value";
                        eventValueComboBox.DataSource = this.plugin.Map.BuildingTypes.Select(t => new { Name = t.DisplayName, Value = (long)t.ID }).ToArray();
                        eventValueComboBox.DataBindings.Add("SelectedValue", triggerEvent, "Data");
                        break;
                    default:
                        break;
                    }
                    break;
                case GameType.RedAlert:
                    switch(eventComboBox.SelectedItem) {
                    case RedAlert.EventTypes.TEVENT_LEAVES_MAP:
                        eventValueComboBox.Visible = true;
                        eventValueComboBox.DataSource = this.plugin.Map.TeamTypes.Select(t => t.Name).ToArray();
                        eventValueComboBox.DataBindings.Add("SelectedItem", triggerEvent, "Team");
                        break;
                    case RedAlert.EventTypes.TEVENT_PLAYER_ENTERED:
                    case RedAlert.EventTypes.TEVENT_CROSS_HORIZONTAL:
                    case RedAlert.EventTypes.TEVENT_CROSS_VERTICAL:
                    case RedAlert.EventTypes.TEVENT_ENTERS_ZONE:
                    case RedAlert.EventTypes.TEVENT_LOW_POWER:
                    case RedAlert.EventTypes.TEVENT_THIEVED:
                    case RedAlert.EventTypes.TEVENT_HOUSE_DISCOVERED:
                    case RedAlert.EventTypes.TEVENT_BUILDINGS_DESTROYED:
                    case RedAlert.EventTypes.TEVENT_UNITS_DESTROYED:
                    case RedAlert.EventTypes.TEVENT_ALL_DESTROYED:
                        eventValueComboBox.Visible = true;
                        eventValueComboBox.DisplayMember = "Name";
                        eventValueComboBox.ValueMember = "Value";
                        eventValueComboBox.DataSource = new {
                            Name = "None",
                            Value = (long)-1
                        }.Yield().Concat(this.plugin.Map.Houses.Select(t => new { t.Type.Name, Value = (long)t.Type.ID })).ToArray();
                        eventValueComboBox.DataBindings.Add("SelectedValue", triggerEvent, "Data");
                        break;
                    case RedAlert.EventTypes.TEVENT_BUILDING_EXISTS:
                    case RedAlert.EventTypes.TEVENT_BUILD:
                        eventValueComboBox.Visible = true;
                        eventValueComboBox.DisplayMember = "Name";
                        eventValueComboBox.ValueMember = "Value";
                        eventValueComboBox.DataSource = this.plugin.Map.BuildingTypes.Select(t => new { Name = t.DisplayName, Value = (long)t.ID }).ToArray();
                        eventValueComboBox.DataBindings.Add("SelectedValue", triggerEvent, "Data");
                        break;
                    case RedAlert.EventTypes.TEVENT_BUILD_UNIT:
                        eventValueComboBox.Visible = true;
                        eventValueComboBox.DisplayMember = "Name";
                        eventValueComboBox.ValueMember = "Value";
                        eventValueComboBox.DataSource = this.plugin.Map.UnitTypes.Where(t => t.IsUnit).Select(t => new { Name = t.DisplayName, Value = (long)t.ID }).ToArray();
                        eventValueComboBox.DataBindings.Add("SelectedValue", triggerEvent, "Data");
                        break;
                    case RedAlert.EventTypes.TEVENT_BUILD_INFANTRY:
                        eventValueComboBox.Visible = true;
                        eventValueComboBox.DisplayMember = "Name";
                        eventValueComboBox.ValueMember = "Value";
                        eventValueComboBox.DataSource = this.plugin.Map.InfantryTypes.Select(t => new { Name = t.DisplayName, Value = (long)t.ID }).ToArray();
                        eventValueComboBox.DataBindings.Add("SelectedValue", triggerEvent, "Data");
                        break;
                    case RedAlert.EventTypes.TEVENT_BUILD_AIRCRAFT:
                        eventValueComboBox.Visible = true;
                        eventValueComboBox.DisplayMember = "Name";
                        eventValueComboBox.ValueMember = "Value";
                        eventValueComboBox.DataSource = this.plugin.Map.UnitTypes.Where(t => t.IsAircraft).Select(t => new { Name = t.DisplayName, Value = (long)t.ID }).ToArray();
                        eventValueComboBox.DataBindings.Add("SelectedValue", triggerEvent, "Data");
                        break;
                    case RedAlert.EventTypes.TEVENT_NUNITS_DESTROYED:
                    case RedAlert.EventTypes.TEVENT_NBUILDINGS_DESTROYED:
                    case RedAlert.EventTypes.TEVENT_CREDITS:
                    case RedAlert.EventTypes.TEVENT_TIME:
                    case RedAlert.EventTypes.TEVENT_GLOBAL_SET:
                    case RedAlert.EventTypes.TEVENT_GLOBAL_CLEAR:
                        eventNud.Visible = true;
                        eventNud.DataBindings.Add("Value", triggerEvent, "Data");
                        break;
                    default:
                        break;
                    }
                    break;
                }
            }

            actionNud.Visible = false;
            actionNud.DataBindings.Clear();
            actionNud.Minimum = long.MinValue;
            actionNud.Maximum = long.MaxValue;
            actionValueComboBox.Visible = false;
            actionValueComboBox.DataBindings.Clear();
            actionValueComboBox.DataSource = null;
            actionValueComboBox.DisplayMember = null;
            actionValueComboBox.ValueMember = null;

            if(triggerAction != null) {
                switch(this.plugin.GameType) {
                case GameType.RedAlert:
                    switch(actionComboBox.SelectedItem) {
                    case RedAlert.ActionTypes.TACTION_CREATE_TEAM:
                    case RedAlert.ActionTypes.TACTION_DESTROY_TEAM:
                    case RedAlert.ActionTypes.TACTION_REINFORCEMENTS:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DataSource = this.plugin.Map.TeamTypes.Select(t => t.Name).ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedItem", triggerAction, "Team");
                        break;
                    case RedAlert.ActionTypes.TACTION_WIN:
                    case RedAlert.ActionTypes.TACTION_LOSE:
                    case RedAlert.ActionTypes.TACTION_BEGIN_PRODUCTION:
                    case RedAlert.ActionTypes.TACTION_FIRE_SALE:
                    case RedAlert.ActionTypes.TACTION_AUTOCREATE:
                    case RedAlert.ActionTypes.TACTION_ALL_HUNT:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DisplayMember = "Name";
                        actionValueComboBox.ValueMember = "Value";
                        actionValueComboBox.DataSource = new {
                            Name = "None",
                            Value = (long)-1
                        }.Yield().Concat(this.plugin.Map.Houses.Select(t => new { t.Type.Name, Value = (long)t.Type.ID })).ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedValue", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_FORCE_TRIGGER:
                    case RedAlert.ActionTypes.TACTION_DESTROY_TRIGGER:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DataSource = this.plugin.Map.Triggers.Select(t => t.Name).ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedItem", triggerAction, "Trigger");
                        break;
                    case RedAlert.ActionTypes.TACTION_DZ:
                    case RedAlert.ActionTypes.TACTION_REVEAL_SOME:
                    case RedAlert.ActionTypes.TACTION_REVEAL_ZONE:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DisplayMember = "Name";
                        actionValueComboBox.ValueMember = "Value";
                        actionValueComboBox.DataSource = new {
                            Name = "None",
                            Value = (long)-1
                        }.Yield().Concat(this.plugin.Map.Waypoints.Select((t, i) => new { t.Name, Value = (long)i })).ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedValue", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_1_SPECIAL:
                    case RedAlert.ActionTypes.TACTION_FULL_SPECIAL:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DisplayMember = "Name";
                        actionValueComboBox.ValueMember = "Value";
                        actionValueComboBox.DataSource = Enum.GetValues(typeof(RedAlert.ActionDataTypes.SpecialWeaponType)).Cast<int>()
                            .Select(v => new { Name = Enum.GetName(typeof(RedAlert.ActionDataTypes.SpecialWeaponType), v), Value = (long)v })
                            .ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedValue", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_PLAY_MUSIC:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DisplayMember = "Name";
                        actionValueComboBox.ValueMember = "Value";
                        actionValueComboBox.DataSource = Enum.GetValues(typeof(RedAlert.ActionDataTypes.ThemeType)).Cast<int>()
                            .Select(v => new { Name = Enum.GetName(typeof(RedAlert.ActionDataTypes.ThemeType), v), Value = (long)v })
                            .ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedValue", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_PLAY_MOVIE:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DisplayMember = "Name";
                        actionValueComboBox.ValueMember = "Value";
                        actionValueComboBox.DataSource = Enum.GetValues(typeof(RedAlert.ActionDataTypes.VQType)).Cast<int>()
                            .Select(v => new { Name = Enum.GetName(typeof(RedAlert.ActionDataTypes.VQType), v), Value = (long)v })
                            .ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedValue", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_PLAY_SOUND:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DisplayMember = "Name";
                        actionValueComboBox.ValueMember = "Value";
                        actionValueComboBox.DataSource = Enum.GetValues(typeof(RedAlert.ActionDataTypes.VocType)).Cast<int>()
                            .Select(v => new { Name = Enum.GetName(typeof(RedAlert.ActionDataTypes.VocType), v), Value = (long)v })
                            .ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedValue", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_PLAY_SPEECH:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DisplayMember = "Name";
                        actionValueComboBox.ValueMember = "Value";
                        actionValueComboBox.DataSource = Enum.GetValues(typeof(RedAlert.ActionDataTypes.VoxType)).Cast<int>()
                            .Select(v => new { Name = Enum.GetName(typeof(RedAlert.ActionDataTypes.VoxType), v), Value = (long)v })
                            .ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedValue", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_PREFERRED_TARGET:
                        actionValueComboBox.Visible = true;
                        actionValueComboBox.DisplayMember = "Name";
                        actionValueComboBox.ValueMember = "Value";
                        actionValueComboBox.DataSource = Enum.GetValues(typeof(RedAlert.ActionDataTypes.QuarryType)).Cast<int>()
                            .Select(v => new { Name = Enum.GetName(typeof(RedAlert.ActionDataTypes.QuarryType), v), Value = (long)v })
                            .ToArray();
                        actionValueComboBox.DataBindings.Add("SelectedValue", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_TEXT_TRIGGER:
                        actionNud.Visible = true;
                        actionNud.Minimum = 1;
                        actionNud.Maximum = 209;
                        actionNud.DataBindings.Add("Value", triggerAction, "Data");
                        break;
                    case RedAlert.ActionTypes.TACTION_ADD_TIMER:
                    case RedAlert.ActionTypes.TACTION_SUB_TIMER:
                    case RedAlert.ActionTypes.TACTION_SET_TIMER:
                    case RedAlert.ActionTypes.TACTION_SET_GLOBAL:
                    case RedAlert.ActionTypes.TACTION_CLEAR_GLOBAL:
                    case RedAlert.ActionTypes.TACTION_BASE_BUILDING:
                        actionNud.Visible = true;
                        actionNud.DataBindings.Add("Value", triggerAction, "Data");
                        break;
                    default:
                        break;
                    }
                    break;
                }
            }
        }
    }
}
