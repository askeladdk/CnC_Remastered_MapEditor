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
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class BasicSettings : UserControl {
        public BasicSettings(IGamePlugin plugin, dynamic basicSection) {
            this.InitializeComponent();

            this.playerComboBox.DataSource = plugin.Map.Houses.Select(h => h.Type.Name).ToArray();
            this.baseComboBox.DataSource = plugin.Map.Houses.Select(h => h.Type.Name).ToArray();
            this.introComboBox.DataSource = plugin.Map.MovieTypes.ToArray();
            this.briefComboBox.DataSource = plugin.Map.MovieTypes.ToArray();
            this.actionComboBox.DataSource = plugin.Map.MovieTypes.ToArray();
            this.winComboBox.DataSource = plugin.Map.MovieTypes.ToArray();
            this.win2ComboBox.DataSource = plugin.Map.MovieTypes.ToArray();
            this.win3ComboBox.DataSource = plugin.Map.MovieTypes.ToArray();
            this.win4ComboBox.DataSource = plugin.Map.MovieTypes.ToArray();
            this.loseComboBox.DataSource = plugin.Map.MovieTypes.ToArray();

            this.carryOverMoneyNud.DataBindings.Add("Value", basicSection, "CarryOverMoney");
            this.nameTxt.DataBindings.Add("Text", basicSection, "Name");
            this.percentNud.DataBindings.Add("Value", basicSection, "Percent");
            this.playerComboBox.DataBindings.Add("SelectedItem", basicSection, "Player");
            this.authorTxt.DataBindings.Add("Text", basicSection, "Author");
            this.isSinglePlayerCheckBox.DataBindings.Add("Checked", basicSection, "SoloMission");
            this.introComboBox.DataBindings.Add("SelectedItem", basicSection, "Intro");
            this.briefComboBox.DataBindings.Add("SelectedItem", basicSection, "Brief");
            this.actionComboBox.DataBindings.Add("SelectedItem", basicSection, "Action");
            this.winComboBox.DataBindings.Add("SelectedItem", basicSection, "Win");
            this.loseComboBox.DataBindings.Add("SelectedItem", basicSection, "Lose");

            switch(plugin.GameType) {
            case GameType.TiberianDawn:
                this.buidLevelNud.DataBindings.Add("Value", basicSection, "BuildLevel");
                this.baseLabel.Visible = this.baseComboBox.Visible = false;
                this.win2Label.Visible = this.win2ComboBox.Visible = false;
                this.win3Label.Visible = this.win3ComboBox.Visible = false;
                this.win4Label.Visible = this.win4ComboBox.Visible = false;
                break;
            case GameType.RedAlert:
                this.buidLevelNud.Visible = this.buildLevelLabel.Visible = false;
                this.baseComboBox.DataBindings.Add("SelectedItem", basicSection, "BasePlayer");
                this.win2ComboBox.DataBindings.Add("SelectedItem", basicSection, "Win2");
                this.win3ComboBox.DataBindings.Add("SelectedItem", basicSection, "Win3");
                this.win4ComboBox.DataBindings.Add("SelectedItem", basicSection, "Win4");
                break;
            }

            this.introComboBox.Enabled = this.briefComboBox.Enabled = this.actionComboBox.Enabled = this.loseComboBox.Enabled = this.isSinglePlayerCheckBox.Checked;
            this.winComboBox.Enabled = this.win2ComboBox.Enabled = this.win3ComboBox.Enabled = this.win4ComboBox.Enabled = this.isSinglePlayerCheckBox.Checked;
        }

        private void isSinglePlayerCheckBox_CheckedChanged(object sender, EventArgs e) {
            this.introComboBox.Enabled = this.briefComboBox.Enabled = this.actionComboBox.Enabled = this.loseComboBox.Enabled = this.isSinglePlayerCheckBox.Checked;
            this.winComboBox.Enabled = this.win2ComboBox.Enabled = this.win3ComboBox.Enabled = this.win4ComboBox.Enabled = this.isSinglePlayerCheckBox.Checked;
        }
    }
}
