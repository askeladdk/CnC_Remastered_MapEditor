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
using MobiusEditor.Controls;
using MobiusEditor.Event;
using MobiusEditor.Interface;
using MobiusEditor.Model;
using MobiusEditor.Utility;
using MobiusEditor.Widgets;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MobiusEditor.Tools {
    public class CellTriggersTool : ViewTool {
        private readonly ComboBox triggerCombo;

        private readonly Dictionary<int, CellTrigger> undoCellTriggers = new Dictionary<int, CellTrigger>();
        private readonly Dictionary<int, CellTrigger> redoCellTriggers = new Dictionary<int, CellTrigger>();

        private bool placementMode;

        public CellTriggersTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, ComboBox triggerCombo, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseUp += this.MapPanel_MouseUp;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.WaypointsTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.WaypointsTool_KeyUp;

            this.triggerCombo = triggerCombo;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            this.UpdateStatus();
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.SetCellTrigger(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveCellTrigger(this.navigationWidget.MouseCell);
                }
            } else if((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right)) {
                this.PickCellTrigger(this.navigationWidget.MouseCell);
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e) {
            if((this.undoCellTriggers.Count > 0) || (this.redoCellTriggers.Count > 0)) {
                this.CommitChange();
            }
        }

        private void WaypointsTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.EnterPlacementMode();
            }
        }

        private void WaypointsTool_KeyUp(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.ExitPlacementMode();
            }
        }

        private void MapPanel_MouseMove(object sender, MouseEventArgs e) {
            if(!this.placementMode && (Control.ModifierKeys == Keys.Shift)) {
                this.EnterPlacementMode();
            } else if(this.placementMode && (Control.ModifierKeys == Keys.None)) {
                this.ExitPlacementMode();
            }


        }

        private void MouseoverWidget_MouseCellChanged(object sender, MouseCellChangedEventArgs e) {
            if(this.placementMode) {
                if(Control.MouseButtons == MouseButtons.Left) {
                    this.SetCellTrigger(e.NewCell);
                } else if(Control.MouseButtons == MouseButtons.Right) {
                    this.RemoveCellTrigger(e.NewCell);
                }
            }
        }

        private void SetCellTrigger(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                if(this.map.CellTriggers[cell] == null) {
                    if(!this.undoCellTriggers.ContainsKey(cell)) {
                        this.undoCellTriggers[cell] = this.map.CellTriggers[cell];
                    }

                    var cellTrigger = new CellTrigger { Trigger = this.triggerCombo.SelectedItem as string };
                    this.map.CellTriggers[cell] = cellTrigger;
                    this.redoCellTriggers[cell] = cellTrigger;

                    this.mapPanel.Invalidate();

                    this.plugin.Dirty = true;
                }
            }
        }

        private void RemoveCellTrigger(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var cellTrigger = this.map.CellTriggers[cell];
                if(cellTrigger != null) {
                    if(!this.undoCellTriggers.ContainsKey(cell)) {
                        this.undoCellTriggers[cell] = this.map.CellTriggers[cell];
                    }

                    this.map.CellTriggers[cell] = null;
                    this.redoCellTriggers[cell] = null;

                    this.mapPanel.Invalidate();

                    this.plugin.Dirty = true;
                }
            }
        }

        private void EnterPlacementMode() {
            if(this.placementMode) {
                return;
            }

            this.placementMode = true;

            this.UpdateStatus();
        }

        private void ExitPlacementMode() {
            if(!this.placementMode) {
                return;
            }

            this.placementMode = false;

            this.UpdateStatus();
        }

        private void PickCellTrigger(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var cellTrigger = this.map.CellTriggers[cell];
                if(cellTrigger != null) {
                    this.triggerCombo.SelectedItem = cellTrigger.Trigger;
                }
            }
        }

        private void CommitChange() {
            var undoCellTriggers2 = new Dictionary<int, CellTrigger>(this.undoCellTriggers);
            void undoAction(UndoRedoEventArgs e) {
                foreach(var kv in undoCellTriggers2) {
                    e.Map.CellTriggers[kv.Key] = kv.Value;
                }
                e.MapPanel.Invalidate();
            }

            var redoCellTriggers2 = new Dictionary<int, CellTrigger>(this.redoCellTriggers);
            void redoAction(UndoRedoEventArgs e) {
                foreach(var kv in redoCellTriggers2) {
                    e.Map.CellTriggers[kv.Key] = kv.Value;
                }
                e.MapPanel.Invalidate();
            }

            this.undoCellTriggers.Clear();
            this.redoCellTriggers.Clear();

            this.url.Track(undoAction, redoAction);
        }

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to set cell trigger, Right-Click to clear cell trigger";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click or Right-Click to pick cell trigger";
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected override void Dispose(bool disposing) {
            if(!this.disposedValue) {
                if(disposing) {
                    this.mapPanel.MouseDown -= this.MapPanel_MouseDown;
                    this.mapPanel.MouseUp -= this.MapPanel_MouseUp;
                    this.mapPanel.MouseMove -= this.MapPanel_MouseMove;
                    (this.mapPanel as Control).KeyDown -= this.WaypointsTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.WaypointsTool_KeyUp;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
