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
using System.Collections.Generic;

namespace MobiusEditor.Utility {
    public class UndoRedoList<T> {
        private const int DefaultMaxUndoRedo = 50;

        private readonly List<(Action<T> Undo, Action<T> Redo)> undoRedoActions = new List<(Action<T> Undo, Action<T> Redo)>();
        private readonly int maxUndoRedo;
        private int undoRedoPosition = 0;

        public event EventHandler<EventArgs> Tracked;
        public event EventHandler<EventArgs> Undone;
        public event EventHandler<EventArgs> Redone;

        public bool CanUndo => this.undoRedoPosition > 0;

        public bool CanRedo => this.undoRedoActions.Count > this.undoRedoPosition;

        public UndoRedoList(int maxUndoRedo) => this.maxUndoRedo = maxUndoRedo;

        public UndoRedoList()
            : this(DefaultMaxUndoRedo) {
        }

        public void Clear() {
            this.undoRedoActions.Clear();
            this.undoRedoPosition = 0;
            this.OnTracked();
        }

        public void Track(Action<T> undo, Action<T> redo) {
            if(this.undoRedoActions.Count > this.undoRedoPosition) {
                this.undoRedoActions.RemoveRange(this.undoRedoPosition, this.undoRedoActions.Count - this.undoRedoPosition);
            }

            this.undoRedoActions.Add((undo, redo));

            if(this.undoRedoActions.Count > this.maxUndoRedo) {
                this.undoRedoActions.RemoveRange(0, this.undoRedoActions.Count - this.maxUndoRedo);
            }

            this.undoRedoPosition = this.undoRedoActions.Count;
            this.OnTracked();
        }

        public void Undo(T context) {
            if(!this.CanUndo) {
                throw new InvalidOperationException();
            }

            this.undoRedoPosition--;
            this.undoRedoActions[this.undoRedoPosition].Undo(context);
            this.OnUndone();
        }

        public void Redo(T context) {
            if(!this.CanRedo) {
                throw new InvalidOperationException();
            }

            this.undoRedoActions[this.undoRedoPosition].Redo(context);
            this.undoRedoPosition++;
            this.OnRedone();
        }

        protected virtual void OnTracked() => Tracked?.Invoke(this, new EventArgs());

        protected virtual void OnUndone() => Undone?.Invoke(this, new EventArgs());

        protected virtual void OnRedone() => Redone?.Invoke(this, new EventArgs());
    }
}
