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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MobiusEditor.Model {
    public class OccupierAddedEventArgs<T> : EventArgs {
        public readonly int Cell;

        public readonly Point Location;

        public readonly T Occupier;

        public OccupierAddedEventArgs(CellMetrics metrics, int cell, T occupier) {
            this.Cell = cell;
            metrics.GetLocation(cell, out this.Location);
            this.Occupier = occupier;
        }

        public OccupierAddedEventArgs(CellMetrics metrics, Point location, T occupier) {
            this.Location = location;
            metrics.GetCell(location, out this.Cell);
            this.Occupier = occupier;
        }
    }

    public class OccupierRemovedEventArgs<T> : EventArgs {
        public readonly int Cell;

        public readonly Point Location;

        public readonly T Occupier;

        public OccupierRemovedEventArgs(CellMetrics metrics, int cell, T occupier) {
            this.Cell = cell;
            metrics.GetLocation(cell, out this.Location);
            this.Occupier = occupier;
        }

        public OccupierRemovedEventArgs(CellMetrics metrics, Point location, T occupier) {
            this.Location = location;
            metrics.GetCell(location, out this.Cell);
            this.Occupier = occupier;
        }
    }

    public class OccupierSet<T> : IEnumerable<(Point Location, T Occupier)>, IEnumerable where T : class, ICellOccupier {
        private readonly CellMetrics metrics;
        private readonly Dictionary<T, Point> occupiers = new Dictionary<T, Point>();
        private readonly T[,] occupierCells;

        public T this[Point location] => this[location.X, location.Y];

        public T this[int x, int y] => this.Contains(x, y) ? this.occupierCells[y, x] : null;

        public T this[int cell] => this.metrics.GetLocation(cell, out var location) ? this[location] : null;

        public Point? this[T occupier] => this.occupiers.ContainsKey(occupier) ? this.occupiers[occupier] : default;

        public IEnumerable<T> Occupiers => this.occupiers.Keys;

        public event EventHandler<OccupierAddedEventArgs<T>> OccupierAdded;
        public event EventHandler<OccupierRemovedEventArgs<T>> OccupierRemoved;
        public event EventHandler<EventArgs> Cleared;

        public OccupierSet(CellMetrics metrics) {
            this.metrics = metrics;
            this.occupierCells = new T[metrics.Height, metrics.Width];
        }

        public bool CanAdd(Point location, T occupier, bool[,] occupyMask) {
            if((occupier == null) || this.Contains(occupier)) {
                return false;
            }

            var occupyPoints = GetOccupyPoints(location, occupyMask).ToArray();
            return !occupyPoints.Any(p => !this.Contains(p) || (this[p] != null));
        }

        public bool CanAdd(int x, int y, T occupier, bool[,] occupyMask) => this.CanAdd(new Point(x, y), occupier, occupyMask);

        public bool CanAdd(int cell, T occupier, bool[,] occupyMask) => this.metrics.GetLocation(cell, out var location) ? this.CanAdd(location, occupier, occupyMask) : false;

        public bool CanAdd(Point location, T occupier) => (occupier != null) ? this.CanAdd(location, occupier, occupier.OccupyMask) : false;

        public bool CanAdd(int x, int y, T occupier) => (occupier != null) ? this.CanAdd(x, y, occupier, occupier.OccupyMask) : false;

        public bool CanAdd(int cell, T occupier) => (occupier != null) ? this.CanAdd(cell, occupier, occupier.OccupyMask) : false;

        public bool Add(Point location, T occupier, bool[,] occupyMask) {
            if(!this.DoAdd(location, occupier, occupyMask)) {
                return false;
            }

            this.OnOccupierAdded(new OccupierAddedEventArgs<T>(this.metrics, location, occupier));
            return true;
        }

        public bool Add(int x, int y, T occupier, bool[,] occupyMask) => this.Add(new Point(x, y), occupier, occupyMask);

        public bool Add(int cell, T occupier, bool[,] occupyMask) => this.metrics.GetLocation(cell, out var location) ? this.Add(location, occupier, occupyMask) : false;

        public bool Add(Point location, T occupier) => (occupier != null) ? this.Add(location, occupier, occupier.OccupyMask) : false;

        public bool Add(int x, int y, T occupier) => (occupier != null) ? this.Add(x, y, occupier, occupier.OccupyMask) : false;

        public bool Add(int cell, T occupier) => (occupier != null) ? this.Add(cell, occupier, occupier.OccupyMask) : false;

        public void Clear() {
            this.occupiers.Clear();
            Array.Clear(this.occupierCells, 0, this.occupierCells.Length);
            this.OnCleared();
        }

        public bool Contains(int x, int y) => ((x >= 0) && (x < this.occupierCells.GetLength(1)) && (y >= 0) && (y < this.occupierCells.GetLength(0)));

        public bool Contains(Point location) => this.Contains(location.X, location.Y);

        public bool Contains(int cell) => this.metrics.GetLocation(cell, out var location) ? this.Contains(location) : false;

        public bool Contains(T occupier) => this.occupiers.ContainsKey(occupier);

        public IEnumerator<(Point Location, T Occupier)> GetEnumerator() => this.occupiers.Select(kv => (kv.Value, kv.Key)).GetEnumerator();

        public bool Remove(T occupier) {
            var oldLocation = this[occupier];
            if(!this.DoRemove(occupier)) {
                return false;
            }

            this.OnOccupierRemoved(new OccupierRemovedEventArgs<T>(this.metrics, oldLocation.Value, occupier));
            return true;
        }

        public bool Remove(Point location) => this.Remove(this[location]);

        public bool Remove(int x, int y) => this.Remove(new Point(x, y));

        public bool Remove(int cell) => this.metrics.GetLocation(cell, out var location) ? this.Remove(location) : false;

        public IEnumerable<(Point Location, U Occupier)> OfType<U>() where U : T => this.Where(i => i.Occupier is U).Select(i => (i.Location, (U)i.Occupier));

        protected virtual void OnOccupierAdded(OccupierAddedEventArgs<T> e) => OccupierAdded?.Invoke(this, e);

        protected virtual void OnOccupierRemoved(OccupierRemovedEventArgs<T> e) => OccupierRemoved?.Invoke(this, e);

        protected virtual void OnCleared() => Cleared?.Invoke(this, new EventArgs());

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private bool DoAdd(Point location, T occupier, bool[,] occupyMask) {
            if((occupier == null) || this.Contains(occupier)) {
                return false;
            }

            var occupyPoints = GetOccupyPoints(location, occupyMask).ToArray();
            if(occupyPoints.Any(p => !this.Contains(p) || (this[p] != null))) {
                return false;
            }

            this.occupiers[occupier] = location;
            foreach(var p in occupyPoints) {
                this.occupierCells[p.Y, p.X] = occupier;
            }
            return true;
        }

        private bool DoRemove(T occupier) {
            if((occupier == null) || !this.occupiers.TryGetValue(occupier, out var location)) {
                return false;
            }

            this.occupiers.Remove(occupier);
            for(var y = location.Y; y < this.metrics.Height; ++y) {
                for(var x = location.X; x < this.metrics.Width; ++x) {
                    if(this.occupierCells[y, x] == occupier) {
                        this.occupierCells[y, x] = null;
                    }
                }
            }
            return true;
        }

        private static IEnumerable<Point> GetOccupyPoints(Point location, bool[,] occupyMask) {
            for(var y = 0; y < occupyMask.GetLength(0); ++y) {
                for(var x = 0; x < occupyMask.GetLength(1); ++x) {
                    if(occupyMask[y, x]) {
                        yield return location + new Size(x, y);
                    }
                }
            }
        }

        private static IEnumerable<Point> GetOccupyPoints(Point location, T occupier) => GetOccupyPoints(location, occupier.OccupyMask);
    }
}
