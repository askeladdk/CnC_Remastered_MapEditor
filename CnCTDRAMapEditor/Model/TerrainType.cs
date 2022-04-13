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
    public class TerrainType : ICellOverlapper, ICellOccupier, IBrowsableType {
        public sbyte ID {
            get; private set;
        }

        public string Name {
            get; private set;
        }

        public string DisplayName => this.Name;

        public Rectangle OverlapBounds => new Rectangle(
            Point.Empty,
            new Size(((this.RenderSize.Width + Globals.TileWidth - 1) / Globals.TileWidth), ((this.RenderSize.Height + Globals.TileHeight - 1) / Globals.TileHeight))
        );

        public bool[,] OccupyMask {
            get; private set;
        }

        public Size Size => new Size(this.OccupyMask.GetLength(1), this.OccupyMask.GetLength(0));

        public TheaterType[] Theaters {
            get; private set;
        }

        public bool IsTransformable {
            get; private set;
        }

        public TemplateTypeFlag TemplateType {
            get; private set;
        }

        public Size RenderSize {
            get; set;
        }

        public Image Thumbnail {
            get; set;
        }

        public TerrainType(sbyte id, string name, TheaterType[] theaters, bool[,] occupyMask, bool isTransformable, TemplateTypeFlag templateType) {
            this.ID = id;
            this.Name = name;
            this.Theaters = theaters;
            this.OccupyMask = occupyMask;
            this.IsTransformable = isTransformable;
            this.TemplateType = templateType;
        }

        public TerrainType(sbyte id, string name, TheaterType[] theaters, bool[,] occupyMask, bool isTransformable)
            : this(id, name, theaters, occupyMask, isTransformable, TemplateTypeFlag.None) {
        }

        public TerrainType(sbyte id, string name, TheaterType[] theaters, bool[,] occupyMask, TemplateTypeFlag templateType)
            : this(id, name, theaters, occupyMask, false, templateType) {
        }

        public TerrainType(sbyte id, string name, TheaterType[] theaters, bool[,] occupyMask)
            : this(id, name, theaters, occupyMask, false, TemplateTypeFlag.None) {
        }

        public override bool Equals(object obj) {
            if(obj is TerrainType) {
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
            var tileName = this.Name;
            if((this.TemplateType & TemplateTypeFlag.OreMine) != TemplateTypeFlag.None) {
                tileName = "OREMINE";
            }

            if(Globals.TheTilesetManager.GetTileData(theater.Tilesets, tileName, this.IsTransformable ? 22 : 0, out Tile tile)) {
                this.RenderSize = new Size(tile.Image.Width / Globals.TileScale, tile.Image.Height / Globals.TileScale);
                this.Thumbnail = new Bitmap(tile.Image, tile.Image.Width / 2, tile.Image.Height / 2);
            }
        }
    }
}
