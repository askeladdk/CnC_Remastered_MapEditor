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
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MobiusEditor.Model {
    public enum InfantryStoppingType {
        Center = 0,
        UpperLeft = 1,
        UpperRight = 2,
        LowerLeft = 3,
        LowerRight = 4
    }

    public class Infantry : INotifyPropertyChanged, ICloneable {
        public event PropertyChangedEventHandler PropertyChanged;

        public InfantryGroup InfantryGroup {
            get; set;
        }

        private InfantryType type;
        public InfantryType Type {
            get => this.type; set => this.SetField(ref this.type, value);
        }

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

        private string mission;
        public string Mission {
            get => this.mission; set => this.SetField(ref this.mission, value);
        }

        private string trigger = Model.Trigger.None;
        public string Trigger {
            get => this.trigger; set => this.SetField(ref this.trigger, value);
        }

        public Color Tint { get; set; } = Color.White;

        public Infantry(InfantryGroup infantryGroup) => this.InfantryGroup = infantryGroup;

        public Infantry Clone() => new Infantry(this.InfantryGroup) {
            Type = Type,
            House = House,
            Strength = Strength,
            Direction = Direction,
            Trigger = Trigger,
            Mission = Mission,
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

    public class InfantryGroup : ICellOverlapper, ICellOccupier {
        private static readonly Point[] stoppingLocations = new Point[Globals.NumInfantryStops];

        public Rectangle OverlapBounds => new Rectangle(-1, -1, 3, 3);

        public bool[,] OccupyMask => new bool[1, 1] { { true } };

        public readonly Infantry[] Infantry = new Infantry[Globals.NumInfantryStops];

        static InfantryGroup() {
            stoppingLocations[(int)InfantryStoppingType.Center] = new Point(Globals.PixelWidth / 2, Globals.PixelHeight / 2);
            stoppingLocations[(int)InfantryStoppingType.UpperLeft] = new Point(Globals.PixelWidth / 4, Globals.PixelHeight / 4);
            stoppingLocations[(int)InfantryStoppingType.UpperRight] = new Point(3 * Globals.PixelWidth / 4, Globals.PixelHeight / 4);
            stoppingLocations[(int)InfantryStoppingType.LowerLeft] = new Point(Globals.PixelWidth / 4, 3 * Globals.PixelHeight / 4);
            stoppingLocations[(int)InfantryStoppingType.LowerRight] = new Point(3 * Globals.PixelWidth / 4, 3 * Globals.PixelHeight / 4);
        }

        public static IEnumerable<InfantryStoppingType> ClosestStoppingTypes(Point subPixel) {
            var stoppingDistances = new (InfantryStoppingType type, float dist)[stoppingLocations.Length];
            for(var i = 0; i < stoppingDistances.Length; ++i) {
                stoppingDistances[i] = ((InfantryStoppingType)i, new Vector2(subPixel.X - stoppingLocations[i].X, subPixel.Y - stoppingLocations[i].Y).LengthSquared());
            }
            return stoppingDistances.OrderBy(sd => sd.dist).Select(sd => sd.type);
        }
    }
}
