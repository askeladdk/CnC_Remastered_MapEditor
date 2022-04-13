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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace MobiusEditor.Model {
    public enum FacingType {
        None,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    public class CellChangedEventArgs<T> : EventArgs {
        public readonly int Cell;

        public readonly Point Location;

        public readonly T OldValue;

        public readonly T Value;

        public CellChangedEventArgs(CellMetrics metrics, int cell, T oldValue, T value) {
            this.Cell = cell;
            metrics.GetLocation(cell, out this.Location);
            this.OldValue = oldValue;
            this.Value = value;
        }

        public CellChangedEventArgs(CellMetrics metrics, Point location, T oldValue, T value) {
            this.Location = location;
            metrics.GetCell(location, out this.Cell);
            this.OldValue = oldValue;
            this.Value = value;
        }
    }

    public class CellGrid<T> : IEnumerable<(int Cell, T Value)>, IEnumerable {
        private readonly CellMetrics metrics;
        private readonly T[,] cells;

        public T this[int x, int y] {
            get => this.cells[y, x];
            set {
                if(!EqualityComparer<T>.Default.Equals(this.cells[y, x], value)) {
                    var lastValue = this.cells[y, x];
                    this.cells[y, x] = value;
                    this.OnCellChanged(new CellChangedEventArgs<T>(this.metrics, new Point(x, y), lastValue, this.cells[y, x]));
                }
            }
        }

        public T this[Point location] { get => this[location.X, location.Y]; set => this[location.X, location.Y] = value; }

        public T this[int cell] { get => this[cell % this.metrics.Width, cell / this.metrics.Width]; set => this[cell % this.metrics.Width, cell / this.metrics.Width] = value; }

        public Size Size => this.metrics.Size;

        public int Length => this.metrics.Length;

        public event EventHandler<CellChangedEventArgs<T>> CellChanged;
        public event EventHandler<EventArgs> Cleared;

        public CellGrid(CellMetrics metrics) {
            this.metrics = metrics;

            this.cells = new T[metrics.Height, metrics.Width];
        }

        public void Clear() {
            Array.Clear(this.cells, 0, this.cells.Length);
            this.OnCleared();
        }

        public T Adjacent(Point location, FacingType facing) => this.metrics.Adjacent(location, facing, out var adjacent) ? this[adjacent] : default;

        public T Adjacent(int cell, FacingType facing) {
            if(!this.metrics.GetLocation(cell, out var location)) {
                return default;

            }
            return this.metrics.Adjacent(location, facing, out var adjacent) ? this[adjacent] : default;
        }

        public bool CopyTo(CellGrid<T> other) {
            if(this.metrics.Length != other.metrics.Length) {
                return false;
            }

            for(var i = 0; i < this.metrics.Length; ++i) {
                other[i] = this[i];
            }

            return true;
        }

        protected virtual void OnCellChanged(CellChangedEventArgs<T> e) => CellChanged?.Invoke(this, e);

        protected virtual void OnCleared() => Cleared?.Invoke(this, new EventArgs());

        public IEnumerable<(int Cell, T Value)> IntersectsWith(ISet<int> cells) {
            foreach(var i in cells) {
                if(this.metrics.Contains(i)) {
                    var cell = this[i];
                    if(cell != null) {
                        yield return (i, cell);
                    }
                }
            }
        }

        public IEnumerable<(int Cell, T Value)> IntersectsWith(ISet<Point> locations) {
            foreach(var location in locations) {
                if(this.metrics.Contains(location)) {
                    var cell = this[location];
                    if(cell != null) {
                        this.metrics.GetCell(location, out var i);
                        yield return (i, cell);
                    }
                }
            }
        }

        public IEnumerator<(int Cell, T Value)> GetEnumerator() {
            for(var i = 0; i < this.metrics.Length; ++i) {
                var cell = this[i];
                if(cell != null) {
                    yield return (i, cell);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
