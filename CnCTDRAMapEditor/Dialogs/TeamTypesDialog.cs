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
    public partial class TeamTypesDialog : Form {
        private readonly IGamePlugin plugin;
        private readonly int maxTeams;
        private readonly IEnumerable<ITechnoType> technoTypes;

        private readonly List<TeamType> teamTypes;
        public IEnumerable<TeamType> TeamTypes => this.teamTypes;

        private ListViewItem SelectedItem => (this.teamTypesListView.SelectedItems.Count > 0) ? this.teamTypesListView.SelectedItems[0] : null;

        private TeamType SelectedTeamType => this.SelectedItem?.Tag as TeamType;

        private TeamTypeClass mockClass;
        private TeamTypeMission mockMission;
        private int classEditRow = -1;
        private int missionEditRow = -1;

        public TeamTypesDialog(IGamePlugin plugin, int maxTeams) {
            this.plugin = plugin;
            this.maxTeams = maxTeams;
            this.technoTypes = plugin.Map.InfantryTypes.Cast<ITechnoType>().Concat(plugin.Map.UnitTypes.Cast<ITechnoType>());

            this.InitializeComponent();

            switch(plugin.GameType) {
            case GameType.TiberianDawn:
                this.triggerLabel.Visible = this.triggerComboBox.Visible = false;
                this.waypointLabel.Visible = this.waypointComboBox.Visible = false;
                break;
            case GameType.RedAlert:
                this.learningCheckBox.Visible = false;
                this.mercernaryCheckBox.Visible = false;
                break;
            }

            this.teamTypes = new List<TeamType>(plugin.Map.TeamTypes.Select(t => t.Clone()));

            this.teamTypesListView.BeginUpdate();
            {
                foreach(var teamType in this.teamTypes) {
                    var item = new ListViewItem(teamType.Name) {
                        Tag = teamType
                    };
                    this.teamTypesListView.Items.Add(item).ToolTipText = teamType.Name;
                }
            }
            this.teamTypesListView.EndUpdate();

            this.houseComboBox.DataSource = plugin.Map.Houses.Select(t => new TypeItem<HouseType>(t.Type.Name, t.Type)).ToArray();
            this.waypointComboBox.DataSource = "(none)".Yield().Concat(plugin.Map.Waypoints.Select(w => w.Name)).ToArray();
            this.triggerComboBox.DataSource = Trigger.None.Yield().Concat(plugin.Map.Triggers.Select(t => t.Name)).ToArray();

            this.teamsTypeColumn.DisplayMember = "Name";
            this.teamsTypeColumn.ValueMember = "Type";
            this.teamsTypeColumn.DataSource = this.technoTypes.Select(t => new TypeItem<ITechnoType>(t.Name, t)).ToArray();

            this.missionsMissionColumn.DataSource = plugin.Map.TeamMissionTypes;

            this.teamTypeTableLayoutPanel.Visible = false;
        }

        private void teamTypesListView_SelectedIndexChanged(object sender, EventArgs e) {
            this.houseComboBox.DataBindings.Clear();
            this.roundaboutCheckBox.DataBindings.Clear();
            this.learningCheckBox.DataBindings.Clear();
            this.suicideCheckBox.DataBindings.Clear();
            this.autocreateCheckBox.DataBindings.Clear();
            this.mercernaryCheckBox.DataBindings.Clear();
            this.reinforcableCheckBox.DataBindings.Clear();
            this.prebuiltCheckBox.DataBindings.Clear();
            this.recruitPriorityNud.DataBindings.Clear();
            this.initNumNud.DataBindings.Clear();
            this.maxAllowedNud.DataBindings.Clear();
            this.fearNud.DataBindings.Clear();
            this.waypointComboBox.DataBindings.Clear();
            this.triggerComboBox.DataBindings.Clear();

            if(this.SelectedTeamType != null) {
                this.houseComboBox.DataBindings.Add("SelectedValue", this.SelectedTeamType, "House");
                this.roundaboutCheckBox.DataBindings.Add("Checked", this.SelectedTeamType, "IsRoundAbout");
                this.learningCheckBox.DataBindings.Add("Checked", this.SelectedTeamType, "IsLearning");
                this.suicideCheckBox.DataBindings.Add("Checked", this.SelectedTeamType, "IsSuicide");
                this.autocreateCheckBox.DataBindings.Add("Checked", this.SelectedTeamType, "IsAutocreate");
                this.mercernaryCheckBox.DataBindings.Add("Checked", this.SelectedTeamType, "IsMercenary");
                this.reinforcableCheckBox.DataBindings.Add("Checked", this.SelectedTeamType, "IsReinforcable");
                this.prebuiltCheckBox.DataBindings.Add("Checked", this.SelectedTeamType, "IsPrebuilt");
                this.recruitPriorityNud.DataBindings.Add("Value", this.SelectedTeamType, "RecruitPriority");
                this.initNumNud.DataBindings.Add("Value", this.SelectedTeamType, "InitNum");
                this.maxAllowedNud.DataBindings.Add("Value", this.SelectedTeamType, "MaxAllowed");
                this.fearNud.DataBindings.Add("Value", this.SelectedTeamType, "Fear");
                this.waypointComboBox.DataBindings.Add("SelectedIndex", this.SelectedTeamType, "Origin");
                this.triggerComboBox.DataBindings.Add("SelectedItem", this.SelectedTeamType, "Trigger");

                this.mockClass = null;
                this.mockMission = null;
                this.classEditRow = -1;
                this.missionEditRow = -1;

                this.teamsDataGridView.Rows.Clear();
                this.missionsDataGridView.Rows.Clear();

                this.teamsDataGridView.RowCount = this.SelectedTeamType.Classes.Count + 1;
                this.missionsDataGridView.RowCount = this.SelectedTeamType.Missions.Count + 1;

                this.updateDataGridViewAddRows(this.teamsDataGridView, Globals.MaxTeamClasses);
                this.updateDataGridViewAddRows(this.missionsDataGridView, Globals.MaxTeamMissions);

                this.teamTypeTableLayoutPanel.Visible = true;
            } else {
                this.teamTypeTableLayoutPanel.Visible = false;
            }
        }

        private void teamTypesListView_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Right) {
                var hitTest = this.teamTypesListView.HitTest(e.Location);

                var canAdd = (hitTest.Item == null) && (this.teamTypesListView.Items.Count < this.maxTeams);
                var canRemove = hitTest.Item != null;
                this.addTeamTypeToolStripMenuItem.Visible = canAdd;
                this.removeTeamTypeToolStripMenuItem.Visible = canRemove;

                if(canAdd || canRemove) {
                    this.teamTypesContextMenuStrip.Show(Cursor.Position);
                }
            }
        }

        private void teamTypesListView_KeyDown(object sender, KeyEventArgs e) {
            if((e.KeyData == Keys.F2) && (this.teamTypesListView.SelectedItems.Count > 0)) {
                this.teamTypesListView.SelectedItems[0].BeginEdit();
            }
        }

        private void addTeamTypeToolStripMenuItem_Click(object sender, EventArgs e) {
            var nameChars = Enumerable.Range(97, 26).Concat(Enumerable.Range(48, 10));

            var name = string.Empty;
            foreach(var nameChar in nameChars) {
                name = new string((char)nameChar, 4);
                if(!this.teamTypes.Where(t => t.Equals(name)).Any()) {
                    break;
                }
            }

            var teamType = new TeamType { Name = name, House = this.plugin.Map.HouseTypes.First() };
            var item = new ListViewItem(teamType.Name) {
                Tag = teamType
            };
            this.teamTypes.Add(teamType);
            this.teamTypesListView.Items.Add(item).ToolTipText = teamType.Name;

            item.Selected = true;
            item.BeginEdit();
        }

        private void removeTeamTypeToolStripMenuItem_Click(object sender, EventArgs e) {
            if(this.SelectedItem != null) {
                this.teamTypes.Remove(this.SelectedTeamType);
                this.teamTypesListView.Items.Remove(this.SelectedItem);
            }
        }

        private void teamTypesListView_AfterLabelEdit(object sender, LabelEditEventArgs e) {
            var maxLength = int.MaxValue;
            switch(this.plugin.GameType) {
            case GameType.TiberianDawn:
                maxLength = 8;
                break;
            case GameType.RedAlert:
                maxLength = 23;
                break;
            }

            if(string.IsNullOrEmpty(e.Label)) {
                e.CancelEdit = true;
            } else if(e.Label.Length > maxLength) {
                e.CancelEdit = true;
                MessageBox.Show(string.Format("Team name is longer than {0} characters.", maxLength), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else if(this.teamTypes.Where(t => (t != this.SelectedTeamType) && t.Equals(e.Label)).Any()) {
                e.CancelEdit = true;
                MessageBox.Show(string.Format("Team with name '{0}' already exists", e.Label), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                this.SelectedTeamType.Name = e.Label;
                this.teamTypesListView.Items[e.Item].ToolTipText = this.SelectedTeamType.Name;
            }
        }

        private void teamsDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            if(this.SelectedTeamType == null) {
                return;
            }

            TeamTypeClass teamTypeClass = null;
            if(e.RowIndex == this.classEditRow) {
                teamTypeClass = this.mockClass;
            } else if(e.RowIndex < this.SelectedTeamType.Classes.Count) {
                teamTypeClass = this.SelectedTeamType.Classes[e.RowIndex];
            }

            if(teamTypeClass == null) {
                return;
            }

            switch(e.ColumnIndex) {
            case 0:
                e.Value = teamTypeClass.Type;
                break;
            case 1:
                e.Value = teamTypeClass.Count;
                break;
            }
        }

        private void teamsDataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
            if(this.SelectedTeamType == null) {
                return;
            }

            if(this.mockClass == null) {
                this.mockClass = (e.RowIndex < this.SelectedTeamType.Classes.Count) ?
                    new TeamTypeClass { Type = this.SelectedTeamType.Classes[e.RowIndex].Type, Count = this.SelectedTeamType.Classes[e.RowIndex].Count } :
                    new TeamTypeClass { Type = this.technoTypes.First(), Count = 0 };
            }
            this.classEditRow = e.RowIndex;

            switch(e.ColumnIndex) {
            case 0:
                this.mockClass.Type = e.Value as ITechnoType;
                break;
            case 1:
                this.mockClass.Count = int.TryParse(e.Value as string, out var value) ? (byte)Math.Max(0, Math.Min(255, value)) : (byte)0;
                break;
            }
        }

        private void teamsDataGridView_NewRowNeeded(object sender, DataGridViewRowEventArgs e) {
            this.mockClass = new TeamTypeClass { Type = this.technoTypes.First(), Count = 0 };
            this.classEditRow = this.teamsDataGridView.RowCount - 1;
        }

        private void teamsDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e) {
            if((this.mockClass != null) && (e.RowIndex >= this.SelectedTeamType.Classes.Count) && ((this.teamsDataGridView.Rows.Count > 1) || (e.RowIndex < (this.teamsDataGridView.Rows.Count - 1)))) {
                this.SelectedTeamType.Classes.Add(this.mockClass);
                this.mockClass = null;
                this.classEditRow = -1;
            } else if((this.mockClass != null) && (e.RowIndex < this.SelectedTeamType.Classes.Count)) {
                this.SelectedTeamType.Classes[e.RowIndex] = this.mockClass;
                this.mockClass = null;
                this.classEditRow = -1;
            } else if(this.teamsDataGridView.ContainsFocus) {
                this.mockClass = null;
                this.classEditRow = -1;
            }
        }

        private void teamsDataGridView_RowDirtyStateNeeded(object sender, QuestionEventArgs e) => e.Response = this.teamsDataGridView.IsCurrentCellDirty;

        private void teamsDataGridView_CancelRowEdit(object sender, QuestionEventArgs e) {
            if((this.classEditRow == (this.teamsDataGridView.Rows.Count - 2)) && (this.classEditRow == this.SelectedTeamType.Classes.Count)) {
                this.mockClass = new TeamTypeClass { Type = this.technoTypes.First(), Count = 0 };
            } else {
                this.mockClass = null;
                this.classEditRow = -1;
            }
        }

        private void teamsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            if(e.Row.Index < this.SelectedTeamType.Classes.Count) {
                this.SelectedTeamType.Classes.RemoveAt(e.Row.Index);
            }

            if(e.Row.Index == this.classEditRow) {
                this.mockClass = null;
                this.classEditRow = -1;
            }
        }

        private void teamsDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e) => this.updateDataGridViewAddRows(this.teamsDataGridView, Globals.MaxTeamClasses);

        private void teamsDataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e) => this.updateDataGridViewAddRows(this.teamsDataGridView, Globals.MaxTeamClasses);

        private void missionsDataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
            if(this.SelectedTeamType == null) {
                return;
            }

            TeamTypeMission teamMissionType = null;
            if(e.RowIndex == this.missionEditRow) {
                teamMissionType = this.mockMission;
            } else if(e.RowIndex < this.SelectedTeamType.Missions.Count) {
                teamMissionType = this.SelectedTeamType.Missions[e.RowIndex];
            }

            if(teamMissionType == null) {
                return;
            }

            switch(e.ColumnIndex) {
            case 0:
                e.Value = teamMissionType.Mission;
                break;
            case 1:
                e.Value = teamMissionType.Argument;
                break;
            }
        }

        private void missionsDataGridView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
            if(this.SelectedTeamType == null) {
                return;
            }

            if(this.mockMission == null) {
                this.mockMission = (e.RowIndex < this.SelectedTeamType.Missions.Count) ?
                    new TeamTypeMission { Mission = this.SelectedTeamType.Missions[e.RowIndex].Mission, Argument = this.SelectedTeamType.Missions[e.RowIndex].Argument } :
                    new TeamTypeMission { Mission = this.plugin.Map.TeamMissionTypes.First(), Argument = 0 };
            }
            this.missionEditRow = e.RowIndex;

            switch(e.ColumnIndex) {
            case 0:
                this.mockMission.Mission = e.Value as string;
                break;
            case 1:
                this.mockMission.Argument = int.TryParse(e.Value as string, out var value) ? value : 0;
                break;
            }
        }

        private void missionsDataGridView_NewRowNeeded(object sender, DataGridViewRowEventArgs e) {
            this.mockMission = new TeamTypeMission { Mission = this.plugin.Map.TeamMissionTypes.First(), Argument = 0 };
            this.missionEditRow = this.missionsDataGridView.RowCount - 1;
        }

        private void missionsDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e) {
            if((this.mockMission != null) && (e.RowIndex >= this.SelectedTeamType.Missions.Count) && ((this.missionsDataGridView.Rows.Count > 1) || (e.RowIndex < (this.missionsDataGridView.Rows.Count - 1)))) {
                this.SelectedTeamType.Missions.Add(this.mockMission);
                this.mockMission = null;
                this.missionEditRow = -1;
            } else if((this.mockMission != null) && (e.RowIndex < this.SelectedTeamType.Missions.Count)) {
                this.SelectedTeamType.Missions[e.RowIndex] = this.mockMission;
                this.mockMission = null;
                this.missionEditRow = -1;
            } else if(this.missionsDataGridView.ContainsFocus) {
                this.mockMission = null;
                this.missionEditRow = -1;
            }
        }

        private void missionsDataGridView_RowDirtyStateNeeded(object sender, QuestionEventArgs e) => e.Response = this.missionsDataGridView.IsCurrentCellDirty;

        private void missionsDataGridView_CancelRowEdit(object sender, QuestionEventArgs e) {
            if((this.missionEditRow == (this.missionsDataGridView.Rows.Count - 2)) && (this.missionEditRow == this.SelectedTeamType.Missions.Count)) {
                this.mockMission = new TeamTypeMission { Mission = this.plugin.Map.TeamMissionTypes.First(), Argument = 0 };
            } else {
                this.mockMission = null;
                this.missionEditRow = -1;
            }
        }

        private void missionsDataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
            if(e.Row.Index < this.SelectedTeamType.Missions.Count) {
                this.SelectedTeamType.Missions.RemoveAt(e.Row.Index);
            }

            if(e.Row.Index == this.missionEditRow) {
                this.mockMission = null;
                this.missionEditRow = -1;
            }
        }

        private void missionsDataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e) => this.updateDataGridViewAddRows(this.missionsDataGridView, Globals.MaxTeamMissions);

        private void missionsDataGridView_UserDeletedRow(object sender, DataGridViewRowEventArgs e) => this.updateDataGridViewAddRows(this.missionsDataGridView, Globals.MaxTeamMissions);

        private void updateDataGridViewAddRows(DataGridView dataGridView, int maxItems) => dataGridView.AllowUserToAddRows = dataGridView.Rows.Count <= maxItems;
    }
}
