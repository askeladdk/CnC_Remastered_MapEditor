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
using MobiusEditor.Utility;
using System;
using System.Drawing;

namespace MobiusEditor.Model {
    [Flags]
    public enum OverlayTypeFlag {
        None = 0,
        TiberiumOrGold = (1 << 0),
        Gems = (1 << 1),
        Wall = (1 << 2),
        Crate = (1 << 3),
        Flag = (1 << 4),
    }

    public class OverlayType : ICellOccupier, IBrowsableType {
        public sbyte ID {
            get; private set;
        }

        public string Name {
            get; private set;
        }

        public string DisplayName {
            get; private set;
        }

        public TheaterType[] Theaters {
            get; private set;
        }

        public OverlayTypeFlag Flag {
            get; private set;
        }

        public Image Thumbnail {
            get; set;
        }

        public bool[,] OccupyMask => new bool[1, 1] { { true } };

        public bool IsResource => (this.Flag & (OverlayTypeFlag.TiberiumOrGold | OverlayTypeFlag.Gems)) != OverlayTypeFlag.None;

        public bool IsTiberiumOrGold => (this.Flag & OverlayTypeFlag.TiberiumOrGold) != OverlayTypeFlag.None;

        public bool IsGem => (this.Flag & OverlayTypeFlag.Gems) != OverlayTypeFlag.None;

        public bool IsWall => (this.Flag & OverlayTypeFlag.Wall) != OverlayTypeFlag.None;

        public bool IsCrate => (this.Flag & OverlayTypeFlag.Crate) != OverlayTypeFlag.None;

        public bool IsFlag => (this.Flag & OverlayTypeFlag.Flag) != OverlayTypeFlag.None;

        public bool IsPlaceable => (this.Flag & ~OverlayTypeFlag.Crate) == OverlayTypeFlag.None;

        public OverlayType(sbyte id, string name, string textId, TheaterType[] theaters, OverlayTypeFlag flag) {
            this.ID = id;
            this.Name = name;
            this.DisplayName = Globals.TheGameTextManager[textId];
            this.Theaters = theaters;
            this.Flag = flag;
        }

        public OverlayType(sbyte id, string name, string textId, OverlayTypeFlag flag)
            : this(id, name, textId, null, flag) {
        }

        public OverlayType(sbyte id, string name, string textId, TheaterType[] theaters)
            : this(id, name, textId, theaters, OverlayTypeFlag.None) {
        }

        public OverlayType(sbyte id, string name, OverlayTypeFlag flag)
            : this(id, name, name, null, flag) {
        }

        public OverlayType(sbyte id, string name, string textId)
            : this(id, name, textId, null, OverlayTypeFlag.None) {
        }

        public override bool Equals(object obj) {
            if(obj is OverlayType) {
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

        public void Init(TheaterType theater) {
            if(Globals.TheTilesetManager.GetTileData(theater.Tilesets, this.Name, 0, out Tile tile)) {
                this.Thumbnail = new Bitmap(tile.Image, tile.Image.Width, tile.Image.Height);
            }
        }
    }
}
