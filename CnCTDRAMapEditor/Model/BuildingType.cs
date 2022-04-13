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
using MobiusEditor.Render;
using System;
using System.Drawing;

namespace MobiusEditor.Model {
    public class BuildingType : ICellOverlapper, ICellOccupier, ITechnoType, IBrowsableType {
        public sbyte ID {
            get; private set;
        }

        public string Name {
            get; private set;
        }

        public string DisplayName {
            get; private set;
        }

        public string Tilename {
            get; private set;
        }

        public Rectangle OverlapBounds => new Rectangle(Point.Empty, new Size(this.OccupyMask.GetLength(1), this.OccupyMask.GetLength(0)));

        public bool[,] OccupyMask {
            get; private set;
        }

        public bool[,] BaseOccupyMask {
            get; private set;
        }

        public Size Size {
            get; private set;
        }

        public bool HasBib {
            get; private set;
        }

        public string OwnerHouse {
            get; private set;
        }

        public TheaterType[] Theaters {
            get; private set;
        }

        public bool IsFake {
            get; private set;
        }

        public bool HasTurret {
            get; private set;
        }

        public string FactoryOverlay {
            get; private set;
        }

        public Image Thumbnail {
            get; set;
        }

        public BuildingType(sbyte id, string name, string textId, bool[,] occupyMask, bool hasBib, string ownerHouse, TheaterType[] theaters, bool isFake, bool hasTurret, string factoryOverlay) {
            this.ID = id;
            this.Name = isFake ? (name.Substring(0, name.Length - 1) + "f") : name;
            this.DisplayName = Globals.TheGameTextManager[textId];
            this.Tilename = name;
            this.BaseOccupyMask = occupyMask;
            this.Size = new Size(this.BaseOccupyMask.GetLength(1), this.BaseOccupyMask.GetLength(0));
            this.HasBib = hasBib;
            this.OwnerHouse = ownerHouse;
            this.Theaters = theaters;
            this.IsFake = isFake;
            this.HasTurret = hasTurret;
            this.FactoryOverlay = factoryOverlay;

            if(this.HasBib) {
                this.OccupyMask = new bool[this.BaseOccupyMask.GetLength(0) + 1, this.BaseOccupyMask.GetLength(1)];
                for(var i = 0; i < this.BaseOccupyMask.GetLength(0) - 1; ++i) {
                    for(var j = 0; j < this.BaseOccupyMask.GetLength(1); ++j) {
                        this.OccupyMask[i, j] = this.BaseOccupyMask[i, j];
                    }
                }
                for(var j = 0; j < this.OccupyMask.GetLength(1); ++j) {
                    this.OccupyMask[this.OccupyMask.GetLength(0) - 2, j] = true;
                    this.OccupyMask[this.OccupyMask.GetLength(0) - 1, j] = true;
                }
            } else {
                this.OccupyMask = this.BaseOccupyMask;
            }
        }

        public BuildingType(sbyte id, string name, string textId, bool[,] occupyMask, bool hasBib, string ownerHouse, bool isFake, bool hasTurret, string factoryOverlay)
            : this(id, name, textId, occupyMask, hasBib, ownerHouse, null, isFake, hasTurret, factoryOverlay) {
        }

        public BuildingType(sbyte id, string name, string textId, bool[,] occupyMask, bool hasBib, string ownerHouse)
            : this(id, name, textId, occupyMask, hasBib, ownerHouse, null, false, false, null) {
        }

        public BuildingType(sbyte id, string name, string textId, bool[,] occupyMask, bool hasBib, string ownerHouse, TheaterType[] theaters)
            : this(id, name, textId, occupyMask, hasBib, ownerHouse, theaters, false, false, null) {
        }

        public BuildingType(sbyte id, string name, string textId, bool[,] occupyMask, bool hasBib, string ownerHouse, bool isFake)
            : this(id, name, textId, occupyMask, hasBib, ownerHouse, null, isFake, false, null) {
        }

        public BuildingType(sbyte id, string name, string textId, bool[,] occupyMask, bool hasBib, string ownerHouse, bool isFake, bool hasTurret)
            : this(id, name, textId, occupyMask, hasBib, ownerHouse, null, isFake, hasTurret, null) {
        }

        public override bool Equals(object obj) {
            if(obj is BuildingType) {
                return this == obj;
            } else if(obj is sbyte) {
                return this.ID == (sbyte)obj;
            } else if(obj is string) {
                return string.Equals(this.Name, obj as string, StringComparison.OrdinalIgnoreCase);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode() => this.ID.GetHashCode();

        public override string ToString() => this.Name;

        public void Init(GameType gameType, TheaterType theater, HouseType house, DirectionType direction) {
            var mockBuilding = new Building() {
                Type = this,
                House = house,
                Strength = 256,
                Direction = direction
            };

            var render = MapRenderer.Render(gameType, theater, Point.Empty, Globals.TileSize, Globals.TileScale, mockBuilding);
            if(!render.Item1.IsEmpty) {
                var buildingPreview = new Bitmap(render.Item1.Width, render.Item1.Height);
                using(var g = Graphics.FromImage(buildingPreview)) {
                    render.Item2(g);
                }
                this.Thumbnail = buildingPreview;
            }
        }
    }
}
