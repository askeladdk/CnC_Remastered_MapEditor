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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MobiusEditor.Model {
    public class Terrain : ICellOverlapper, ICellOccupier, INotifyPropertyChanged, ICloneable {
        public event PropertyChangedEventHandler PropertyChanged;

        private TerrainType type;
        public TerrainType Type {
            get => this.type; set => this.SetField(ref this.type, value);
        }

        private int icon;
        public int Icon {
            get => this.icon; set => this.SetField(ref this.icon, value);
        }

        public Rectangle OverlapBounds => this.Type.OverlapBounds;

        public bool[,] OccupyMask => this.Type.OccupyMask;

        private string trigger = Model.Trigger.None;
        public string Trigger {
            get => this.trigger; set => this.SetField(ref this.trigger, value);
        }

        public Color Tint { get; set; } = Color.White;

        public Terrain Clone() => new Terrain() {
            Type = Type,
            Icon = Icon,
            Trigger = Trigger
        };

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
            if(EqualityComparer<T>.Default.Equals(field, value)) {
                return false;
            }
            field = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        object ICloneable.Clone() => this.Clone();
    }
}
