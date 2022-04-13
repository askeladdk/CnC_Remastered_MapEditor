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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MobiusEditor.Tools {
    public class WaypointsTool : ViewTool {
        private readonly ComboBox waypointCombo;

        private (Waypoint waypoint, int? cell)? undoWaypoint;
        private (Waypoint waypoint, int? cell)? redoWaypoint;

        private bool placementMode;

        public WaypointsTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, ComboBox waypointCombo, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.WaypointsTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.WaypointsTool_KeyUp;

            this.waypointCombo = waypointCombo;

            this.UpdateStatus();
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.SetWaypoint(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveWaypoint(this.navigationWidget.MouseCell);
                }
            } else if((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right)) {
                this.PickWaypoint(this.navigationWidget.MouseCell);
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

        private void SetWaypoint(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var waypoint = this.map.Waypoints[this.waypointCombo.SelectedIndex];
                if(waypoint.Cell != cell) {
                    if(this.undoWaypoint == null) {
                        this.undoWaypoint = (waypoint, waypoint.Cell);
                    } else if(this.undoWaypoint.Value.cell == cell) {
                        this.undoWaypoint = null;
                    }

                    waypoint.Cell = cell;
                    this.redoWaypoint = (waypoint, waypoint.Cell);

                    this.CommitChange();

                    this.mapPanel.Invalidate();

                    this.plugin.Dirty = true;
                }
            }
        }

        private void RemoveWaypoint(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var waypoint = this.map.Waypoints.Where(w => w.Cell == cell).FirstOrDefault();
                if(waypoint != null) {
                    if(this.undoWaypoint == null) {
                        this.undoWaypoint = (waypoint, waypoint.Cell);
                    }

                    waypoint.Cell = null;
                    this.redoWaypoint = (waypoint, null);

                    this.CommitChange();

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

        private void PickWaypoint(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                for(var i = 0; i < this.map.Waypoints.Length; ++i) {
                    if(this.map.Waypoints[i].Cell == cell) {
                        this.waypointCombo.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void CommitChange() {
            var undoWaypoint2 = this.undoWaypoint;
            void undoAction(UndoRedoEventArgs e) {
                undoWaypoint2.Value.waypoint.Cell = undoWaypoint2.Value.cell;
                this.mapPanel.Invalidate();
            }

            var redoWaypoint2 = this.redoWaypoint;
            void redoAction(UndoRedoEventArgs e) {
                redoWaypoint2.Value.waypoint.Cell = redoWaypoint2.Value.cell;
                this.mapPanel.Invalidate();
            }

            this.undoWaypoint = null;
            this.redoWaypoint = null;

            this.url.Track(undoAction, redoAction);
        }

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to set cell waypoint, Right-Click to clear cell waypoint";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click or Right-Click to pick cell waypoint";
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected override void Dispose(bool disposing) {
            if(!this.disposedValue) {
                if(disposing) {
                    this.mapPanel.MouseDown -= this.MapPanel_MouseDown;
                    this.mapPanel.MouseMove -= this.MapPanel_MouseMove;
                    (this.mapPanel as Control).KeyDown -= this.WaypointsTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.WaypointsTool_KeyUp;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
