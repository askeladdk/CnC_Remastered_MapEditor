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
    public class Building : ICellOverlapper, ICellOccupier, INotifyPropertyChanged, ICloneable {
        public event PropertyChangedEventHandler PropertyChanged;

        private BuildingType type;
        public BuildingType Type {
            get => this.type; set => this.SetField(ref this.type, value);
        }

        public Rectangle OverlapBounds => this.Type.OverlapBounds;

        public bool[,] OccupyMask => this.Type.OccupyMask;

        private HouseType house;
        public HouseType House {
            get => this.house; set => this.SetField(ref this.house, value);
        }

        private int strength;
        public int Strength {
            get => this.strength; set => this.SetField(ref this.strength, value);
        }

        private DirectionType direction;
        public DirectionType Direction {
            get => this.direction; set => this.SetField(ref this.direction, value);
        }

        private string trigger = Model.Trigger.None;
        public string Trigger {
            get => this.trigger; set => this.SetField(ref this.trigger, value);
        }

        private int basePriority = -1;
        public int BasePriority {
            get => this.basePriority; set => this.SetField(ref this.basePriority, value);
        }

        private bool isPrebuilt = true;
        public bool IsPrebuilt {
            get => this.isPrebuilt; set => this.SetField(ref this.isPrebuilt, value);
        }

        private bool sellable;
        public bool Sellable {
            get => this.sellable; set => this.SetField(ref this.sellable, value);
        }

        private bool rebuild;
        public bool Rebuild {
            get => this.rebuild; set => this.SetField(ref this.rebuild, value);
        }

        public ISet<int> BibCells { get; private set; } = new HashSet<int>();

        private Color tint = Color.White;
        public Color Tint {
            get => this.IsPrebuilt ? this.tint : Color.FromArgb((int)(this.tint.A * 0.75f), this.tint.R, this.tint.G, this.tint.B);
            set => this.tint = value;
        }

        public Building Clone() => new Building() {
            Type = Type,
            House = House,
            Strength = Strength,
            Direction = Direction,
            Trigger = Trigger,
            BasePriority = BasePriority,
            IsPrebuilt = IsPrebuilt,
            Sellable = Sellable,
            Rebuild = Rebuild,
            Tint = Tint
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
