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
using System;
using System.Drawing;

namespace MobiusEditor.Model {
    public class CellMetrics {
        public static readonly FacingType[] AdjacentFacings = new FacingType[] {
            FacingType.North, FacingType.NorthEast, FacingType.East, FacingType.SouthEast, FacingType.South, FacingType.SouthWest, FacingType.West, FacingType.NorthWest
        };

        public int Width {
            get; private set;
        }

        public int Height {
            get; private set;
        }

        public Point TopLeft => Point.Empty;

        public Size Size => new Size(this.Width, this.Height);

        public Rectangle Bounds => new Rectangle(this.TopLeft, this.Size);

        public int Length => this.Width * this.Height;

        public bool Contains(Point location) => ((location.X >= 0) && (location.X < this.Width) && (location.Y >= 0) && (location.Y < this.Height));

        public bool Contains(int cell) => ((cell >= 0) && (cell < this.Length));

        public CellMetrics(int width, int height) {
            this.Width = width;
            this.Height = height;
        }

        public CellMetrics(Size size)
            : this(size.Width, size.Height) {
        }

        public bool GetCell(Point location, out int cell) {
            cell = (location.Y * this.Width) + location.X;
            return this.Contains(location);
        }

        public bool GetLocation(int cell, out Point location) {
            location = new Point(cell % this.Width, cell / this.Width);
            return this.Contains(cell);
        }

        public bool Adjacent(Point location, FacingType facing, out Point adjacent) {
            adjacent = location;
            switch(facing) {
            case FacingType.North:
                adjacent.Y--;
                break;
            case FacingType.NorthEast:
                adjacent.X++;
                adjacent.Y--;
                break;
            case FacingType.East:
                adjacent.X++;
                break;
            case FacingType.SouthEast:
                adjacent.X++;
                adjacent.Y++;
                break;
            case FacingType.South:
                adjacent.Y++;
                break;
            case FacingType.SouthWest:
                adjacent.X--;
                adjacent.Y++;
                break;
            case FacingType.West:
                adjacent.X--;
                break;
            case FacingType.NorthWest:
                adjacent.X--;
                adjacent.Y--;
                break;
            }

            return this.Contains(adjacent);
        }

        public bool Adjacent(int cell, FacingType facing, out int adjacent) {
            if(!this.GetLocation(cell, out var location) || !this.Adjacent(location, facing, out var adjacentPoint)) {
                adjacent = -1;
                return false;
            } else {
                return this.GetCell(adjacentPoint, out adjacent);
            }
        }

        public void Clip(ref Point location) {
            location.X = Math.Max(0, Math.Min(this.Width - 1, location.X));
            location.Y = Math.Max(0, Math.Min(this.Height - 1, location.Y));
        }

        public void Clip(ref Point location, Size margin) => this.Clip(ref location, margin, margin);

        public void Clip(ref Point location, Size topLeftMargin, Size bottomRightMargin) {
            location.X = Math.Max(topLeftMargin.Width, Math.Min(this.Width - bottomRightMargin.Width - 1, location.X));
            location.Y = Math.Max(topLeftMargin.Height, Math.Min(this.Height - bottomRightMargin.Height - 1, location.Y));
        }
    }
}
