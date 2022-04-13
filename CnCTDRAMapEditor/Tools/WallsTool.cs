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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MobiusEditor.Tools {
    public class WallsTool : ViewTool {
        private readonly TypeComboBox wallTypeComboBox;
        private readonly MapPanel wallTypeMapPanel;

        private readonly Dictionary<int, Overlay> undoOverlays = new Dictionary<int, Overlay>();
        private readonly Dictionary<int, Overlay> redoOverlays = new Dictionary<int, Overlay>();

        private Map previewMap;
        protected override Map RenderMap => this.previewMap;

        private bool placementMode;

        private OverlayType selectedWallType;
        private OverlayType SelectedWallType {
            get => this.selectedWallType;
            set {
                if(this.selectedWallType != value) {
                    if(this.placementMode && (this.selectedWallType != null)) {
                        this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
                    }

                    this.selectedWallType = value;
                    this.wallTypeComboBox.SelectedValue = this.selectedWallType;

                    this.RefreshMapPanel();
                }
            }
        }

        public WallsTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, TypeComboBox wallTypeComboBox, MapPanel wallTypeMapPanel, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.previewMap = this.map;

            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseUp += this.MapPanel_MouseUp;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.WallTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.WallTool_KeyUp;

            this.wallTypeComboBox = wallTypeComboBox;
            this.wallTypeComboBox.SelectedIndexChanged += this.WallTypeComboBox_SelectedIndexChanged;

            this.wallTypeMapPanel = wallTypeMapPanel;
            this.wallTypeMapPanel.BackColor = Color.White;
            this.wallTypeMapPanel.MaxZoom = 1;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            this.SelectedWallType = this.wallTypeComboBox.Types.First() as OverlayType;

            this.UpdateStatus();
        }

        private void WallTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) => this.SelectedWallType = this.wallTypeComboBox.SelectedValue as OverlayType;

        private void WallTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.EnterPlacementMode();
            }
        }

        private void WallTool_KeyUp(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.ExitPlacementMode();
            }
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.AddWall(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveWall(this.navigationWidget.MouseCell);
                }
            } else if((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right)) {
                this.PickWall(this.navigationWidget.MouseCell);
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e) {
            if((this.undoOverlays.Count > 0) || (this.redoOverlays.Count > 0)) {
                this.CommitChange();
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
                    this.AddWall(e.NewCell);
                } else if(Control.MouseButtons == MouseButtons.Right) {
                    this.RemoveWall(e.NewCell);
                }

                if(this.SelectedWallType != null) {
                    this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(e.OldCell, new Size(1, 1)), 1, 1));
                    this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(e.NewCell, new Size(1, 1)), 1, 1));
                }
            }
        }

        private void AddWall(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                if(this.SelectedWallType != null) {
                    var overlay = new Overlay { Type = SelectedWallType, Icon = 0 };
                    if(this.map.Technos.CanAdd(cell, overlay) && this.map.Buildings.CanAdd(cell, overlay)) {
                        if(!this.undoOverlays.ContainsKey(cell)) {
                            this.undoOverlays[cell] = this.map.Overlay[cell];
                        }

                        this.map.Overlay[cell] = overlay;
                        this.redoOverlays[cell] = overlay;

                        this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(location, new Size(1, 1)), 1, 1));

                        this.plugin.Dirty = true;
                    }
                }
            }
        }

        private void RemoveWall(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var overlay = this.map.Overlay[cell];
                if(overlay?.Type.IsWall ?? false) {
                    if(!this.undoOverlays.ContainsKey(cell)) {
                        this.undoOverlays[cell] = this.map.Overlay[cell];
                    }

                    this.map.Overlay[cell] = null;
                    this.redoOverlays[cell] = null;

                    this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(location, new Size(1, 1)), 1, 1));

                    this.plugin.Dirty = true;
                }
            }
        }

        private void CommitChange() {
            var undoOverlays2 = new Dictionary<int, Overlay>(this.undoOverlays);
            void undoAction(UndoRedoEventArgs e) {
                foreach(var kv in undoOverlays2) {
                    e.Map.Overlay[kv.Key] = kv.Value;
                }
                e.MapPanel.Invalidate(e.Map, undoOverlays2.Keys.Select(k => {
                    e.Map.Metrics.GetLocation(k, out var location);
                    return Rectangle.Inflate(new Rectangle(location, new Size(1, 1)), 1, 1);
                }));
            }

            var redoOverlays2 = new Dictionary<int, Overlay>(this.redoOverlays);
            void redoAction(UndoRedoEventArgs e) {
                foreach(var kv in redoOverlays2) {
                    e.Map.Overlay[kv.Key] = kv.Value;
                }
                e.MapPanel.Invalidate(e.Map, redoOverlays2.Keys.Select(k => {
                    e.Map.Metrics.GetLocation(k, out var location);
                    return Rectangle.Inflate(new Rectangle(location, new Size(1, 1)), 1, 1);
                }));
            }

            this.undoOverlays.Clear();
            this.redoOverlays.Clear();

            this.url.Track(undoAction, redoAction);
        }

        private void EnterPlacementMode() {
            if(this.placementMode) {
                return;
            }

            this.placementMode = true;

            this.navigationWidget.MouseoverSize = Size.Empty;

            if(this.SelectedWallType != null) {
                this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(this.navigationWidget.MouseCell, new Size(1, 1)), 1, 1));
            }

            this.UpdateStatus();
        }

        private void ExitPlacementMode() {
            if(!this.placementMode) {
                return;
            }

            this.placementMode = false;

            this.navigationWidget.MouseoverSize = new Size(1, 1);

            if(this.SelectedWallType != null) {
                this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(this.navigationWidget.MouseCell, new Size(1, 1)), 1, 1));
            }

            this.UpdateStatus();
        }

        private void PickWall(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var overlay = this.map.Overlay[cell];
                if((overlay != null) && overlay.Type.IsWall) {
                    this.SelectedWallType = overlay.Type;
                }
            }
        }

        private void RefreshMapPanel() => this.wallTypeMapPanel.MapImage = this.SelectedWallType?.Thumbnail;

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click drag to add walls, Right-Click drag to remove walls";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click or Right-Click to pick wall";
            }
        }

        protected override void PreRenderMap() {
            base.PreRenderMap();

            this.previewMap = this.map.Clone();
            if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedWallType != null) {
                    if(this.previewMap.Metrics.GetCell(location, out var cell)) {
                        var overlay = new Overlay { Type = SelectedWallType, Icon = 0, Tint = Color.FromArgb(128, Color.White) };
                        if(this.previewMap.Technos.CanAdd(cell, overlay) && this.previewMap.Buildings.CanAdd(cell, overlay)) {
                            this.previewMap.Overlay[cell] = overlay;
                            this.mapPanel.Invalidate(this.previewMap, Rectangle.Inflate(new Rectangle(location, new Size(1, 1)), 1, 1));
                        }
                    }
                }
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            var wallPen = new Pen(Color.Green, 4.0f);
            foreach(var (cell, overlay) in this.previewMap.Overlay) {
                if(overlay.Type.IsWall) {
                    this.previewMap.Metrics.GetLocation(cell, out var topLeft);
                    var bounds = new Rectangle(new Point(topLeft.X * Globals.TileWidth, topLeft.Y * Globals.TileHeight), Globals.TileSize);
                    graphics.DrawRectangle(wallPen, bounds);
                }
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
                    (this.mapPanel as Control).KeyDown -= this.WallTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.WallTool_KeyUp;

                    this.wallTypeComboBox.SelectedIndexChanged -= this.WallTypeComboBox_SelectedIndexChanged;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
