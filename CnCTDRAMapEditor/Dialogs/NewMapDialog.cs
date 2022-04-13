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
using System.Windows.Forms;

namespace MobiusEditor.Dialogs {
    public partial class NewMapDialog : Form {
        private GameType gameType = GameType.TiberianDawn;
        public GameType GameType {
            get => this.gameType;
            set {
                if(this.gameType != value) {
                    this.gameType = value;
                    this.UpdateGameType();
                }
            }
        }

        public string TheaterName {
            get {
                if(this.radioTheater1.Checked)
                    return this.radioTheater1.Text;
                if(this.radioTheater2.Checked)
                    return this.radioTheater2.Text;
                if(this.radioTheater3.Checked)
                    return this.radioTheater3.Text;
                return null;
            }
        }

        public NewMapDialog() => this.InitializeComponent();

        private void UpdateGameType() {
            switch(this.GameType) {
            case GameType.TiberianDawn: {
                this.radioTheater1.Text = "Desert";
                this.radioTheater2.Text = "Temperate";
                this.radioTheater3.Text = "Winter";
            }
            break;
            case GameType.RedAlert: {
                this.radioTheater1.Text = "Temperate";
                this.radioTheater2.Text = "Snow";
                this.radioTheater3.Text = "Interior";
            }
            break;
            }
        }

        private void radioGameType_CheckedChanged(object sender, EventArgs e) {
            if(this.radioTD.Checked) {
                this.GameType = GameType.TiberianDawn;
            } else if(this.radioRA.Checked) {
                this.GameType = GameType.RedAlert;
            }
        }
    }
}
