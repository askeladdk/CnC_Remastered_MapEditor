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
using MobiusEditor.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MobiusEditor.Model {
    public class BooleanTypeConverter : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => (context is MapContext) && (sourceType == typeof(string));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => (context is MapContext) && (destinationType == typeof(string));

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if(!(value is bool boolean) || !this.CanConvertTo(context, destinationType)) {
                return null;
            }

            return boolean ? "1" : "0";
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if(!(value is string str) || !this.CanConvertFrom(context, value?.GetType())) {
                return null;
            }

            var first = (str.Length > 0) ? str.ToUpper()[0] : 0;
            return (first == 'T') || (first == 'Y') || (first == '1');
        }
    }

    public class PercentageTypeConverter : TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) => (context is MapContext) && (sourceType == typeof(string));

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) => (context is MapContext) && (destinationType == typeof(string));

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if(!(value is int percent) || !this.CanConvertTo(context, destinationType)) {
                return null;
            }

            var mapContext = context as MapContext;
            return mapContext.FractionalPercentages ? (percent / 100M).ToString("D2") : percent.ToString();
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if(!(value is string str) || !this.CanConvertFrom(context, value?.GetType())) {
                return null;
            }

            var mapContext = context as MapContext;
            if(mapContext.FractionalPercentages && str.Contains(".")) {
                if(!decimal.TryParse(str, out var percent)) {
                    return null;
                }
                return (int)(percent * 100);
            } else {
                if(!int.TryParse(str, out var percent)) {
                    return null;
                }
                return percent;
            }
        }
    }

    public class BasicSection : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        [DefaultValue(null)]
        public string Name {
            get => this.name; set => this.SetField(ref this.name, value);
        }

        private int carryOverCap;
        [TypeConverter(typeof(PercentageTypeConverter))]
        [DefaultValue(-1)]
        public int CarryOverCap {
            get => this.carryOverCap; set => this.SetField(ref this.carryOverCap, value);
        }

        private int carryOverMoney;
        [TypeConverter(typeof(PercentageTypeConverter))]
        [DefaultValue(100)]
        public int CarryOverMoney {
            get => this.carryOverMoney; set => this.SetField(ref this.carryOverMoney, value);
        }

        private string intro;
        [DefaultValue(null)]
        public string Intro {
            get => this.intro; set => this.SetField(ref this.intro, value);
        }

        private string theme;
        [DefaultValue("No Theme")]
        public string Theme {
            get => this.theme; set => this.SetField(ref this.theme, value);
        }

        private int percent;
        [TypeConverter(typeof(PercentageTypeConverter))]
        [DefaultValue(100)]
        public int Percent {
            get => this.percent; set => this.SetField(ref this.percent, value);
        }

        public string player;
        [DefaultValue(null)]
        public string Player {
            get => this.player; set => this.SetField(ref this.player, value);
        }

        private string action;
        [DefaultValue("x")]
        public string Action {
            get => this.action; set => this.SetField(ref this.action, value);
        }

        private string lose;
        [DefaultValue("x")]
        public string Lose {
            get => this.lose; set => this.SetField(ref this.lose, value);
        }

        private string win;
        [DefaultValue("x")]
        public string Win {
            get => this.win; set => this.SetField(ref this.win, value);
        }

        private string brief;
        [DefaultValue("x")]
        public string Brief {
            get => this.brief; set => this.SetField(ref this.brief, value);
        }

        private string author;
        [DefaultValue(null)]
        public string Author {
            get => this.author; set => this.SetField(ref this.author, value);
        }

        private string basePlayer;
        [NonSerializedINIKey]
        [DefaultValue(null)]
        public string BasePlayer {
            get => this.basePlayer; set => this.SetField(ref this.basePlayer, value);
        }

        private bool soloMission;
        [DefaultValue(false)]
        public bool SoloMission {
            get => this.soloMission; set => this.SetField(ref this.soloMission, value);
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
            if(EqualityComparer<T>.Default.Equals(field, value)) {
                return false;
            }
            field = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
