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
using System.ComponentModel;

namespace MobiusEditor.RedAlert {
    public class BasicSection : Model.BasicSection {
        private string win2;
        [DefaultValue("x")]
        public string Win2 {
            get => this.win2; set => this.SetField(ref this.win2, value);
        }

        private string win3;
        [DefaultValue("x")]
        public string Win3 {
            get => this.win3; set => this.SetField(ref this.win3, value);
        }

        private string win4;
        [DefaultValue("x")]
        public string Win4 {
            get => this.win4; set => this.SetField(ref this.win4, value);
        }

        private bool toCarryOver;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool ToCarryOver {
            get => this.toCarryOver; set => this.SetField(ref this.toCarryOver, value);
        }

        private bool toInherit;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool ToInherit {
            get => this.toInherit; set => this.SetField(ref this.toInherit, value);
        }

        private bool timerInherit;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool TimerInherit {
            get => this.timerInherit; set => this.SetField(ref this.timerInherit, value);
        }

        private bool endOfGame;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool EndOfGame {
            get => this.endOfGame; set => this.SetField(ref this.endOfGame, value);
        }

        private bool civEvac;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool CivEvac {
            get => this.civEvac; set => this.SetField(ref this.civEvac, value);
        }

        private bool noSpyPlane;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool NoSpyPlane {
            get => this.noSpyPlane; set => this.SetField(ref this.noSpyPlane, value);
        }

        private bool skipScore;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool SkipScore {
            get => this.skipScore; set => this.SetField(ref this.skipScore, value);
        }

        private bool oneTimeOnly;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool OneTimeOnly {
            get => this.oneTimeOnly; set => this.SetField(ref this.oneTimeOnly, value);
        }

        private bool skipMapSelect;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool SkipMapSelect {
            get => this.skipMapSelect; set => this.SetField(ref this.skipMapSelect, value);
        }

        private bool truckCrate;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool TruckCrate {
            get => this.truckCrate; set => this.SetField(ref this.truckCrate, value);
        }

        private bool fillSilos;
        [TypeConverter(typeof(BooleanTypeConverter))]
        [DefaultValue(false)]
        public bool FillSilos {
            get => this.fillSilos; set => this.SetField(ref this.fillSilos, value);
        }
    }
}
