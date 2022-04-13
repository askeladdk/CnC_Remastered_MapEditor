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

namespace MobiusEditor.Controls {
    public partial class PlayerSettings : UserControl {
        private readonly PropertyTracker<House> houseSettingsTracker;
        private readonly dynamic house;

        public PlayerSettings(IGamePlugin plugin, PropertyTracker<House> houseSettingsTracker) {
            this.houseSettingsTracker = houseSettingsTracker;
            this.house = houseSettingsTracker;

            this.InitializeComponent();

            this.edgeComboBox.Items.Clear();
            this.edgeComboBox.Items.AddRange(new string[] { "North", "South", "West", "East" });

            this.creditsNud.DataBindings.Add("Value", houseSettingsTracker, "Credits");
            this.maxBuildingsNud.DataBindings.Add("Value", houseSettingsTracker, "MaxBuilding");
            this.maxUnitsNud.DataBindings.Add("Value", houseSettingsTracker, "MaxUnit");
            this.edgeComboBox.DataBindings.Add("SelectedItem", houseSettingsTracker, "Edge");

            switch(plugin.GameType) {
            case GameType.TiberianDawn:
                this.maxInfantryNud.Visible = this.maxInfantryLbl.Visible = false;
                this.maxVesselsNud.Visible = this.maxVesselsLbl.Visible = false;
                this.techLevelNud.Visible = this.techLevelLbl.Visible = false;
                this.iqNud.Visible = this.iqLbl.Visible = false;
                this.playerControlCheckBox.Visible = this.playerControlLbl.Visible = false;
                break;
            case GameType.RedAlert:
                this.maxInfantryNud.DataBindings.Add("Value", houseSettingsTracker, "MaxInfantry");
                this.maxVesselsNud.DataBindings.Add("Value", houseSettingsTracker, "MaxVessel");
                this.techLevelNud.DataBindings.Add("Value", houseSettingsTracker, "TechLevel");
                this.iqNud.DataBindings.Add("Value", houseSettingsTracker, "IQ");
                this.playerControlCheckBox.DataBindings.Add("Checked", houseSettingsTracker, "PlayerControl");
                break;
            }

            this.playersListBox.Items.Clear();
            this.playersListBox.Items.AddRange(plugin.Map.Houses.Select(h => h.Type.Name).ToArray());

            var selectedIndices = new List<int>();
            foreach(var id in this.house.Allies) {
                this.playersListBox.SetSelected(id, true);
            }

            this.playersListBox.SelectedIndexChanged += this.playersListBox_SelectedIndexChanged;
        }

        private void playersListBox_SelectedIndexChanged(object sender, EventArgs e) {
            var allies = 0;
            foreach(int selectedIndex in this.playersListBox.SelectedIndices) {
                allies |= 1 << selectedIndex;
            }
            this.house.Allies = new AlliesMask(allies);
        }
    }
}
