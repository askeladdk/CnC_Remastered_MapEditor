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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MobiusEditor.Tools {
    public class OverlaysTool : ViewTool {
        private readonly TypeComboBox overlayTypeComboBox;
        private readonly MapPanel overlayTypeMapPanel;

        private Map previewMap;
        protected override Map RenderMap => this.previewMap;

        private bool placementMode;

        private OverlayType selectedOverlayType;
        private OverlayType SelectedOverlayType {
            get => this.selectedOverlayType;
            set {
                if(this.selectedOverlayType != value) {
                    if(this.placementMode && (this.selectedOverlayType != null)) {
                        this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
                    }

                    this.selectedOverlayType = value;
                    this.overlayTypeComboBox.SelectedValue = this.selectedOverlayType;

                    this.RefreshMapPanel();
                }
            }
        }

        public OverlaysTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, TypeComboBox overlayTypeComboBox, MapPanel overlayTypeMapPanel, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.previewMap = this.map;

            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.OverlaysTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.OverlaysTool_KeyUp;

            this.overlayTypeComboBox = overlayTypeComboBox;
            this.overlayTypeComboBox.SelectedIndexChanged += this.OverlayTypeComboBox_SelectedIndexChanged;

            this.overlayTypeMapPanel = overlayTypeMapPanel;
            this.overlayTypeMapPanel.BackColor = Color.White;
            this.overlayTypeMapPanel.MaxZoom = 1;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            this.SelectedOverlayType = this.overlayTypeComboBox.Types.First() as OverlayType;

            this.UpdateStatus();
        }

        private void OverlayTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) => this.SelectedOverlayType = this.overlayTypeComboBox.SelectedValue as OverlayType;

        private void OverlaysTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.EnterPlacementMode();
            }
        }

        private void OverlaysTool_KeyUp(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.ExitPlacementMode();
            }
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.AddOverlay(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveOverlay(this.navigationWidget.MouseCell);
                }
            } else if((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right)) {
                this.PickOverlay(this.navigationWidget.MouseCell);
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
                if(this.SelectedOverlayType != null) {
                    this.mapPanel.Invalidate(this.map, new Rectangle(e.OldCell, new Size(1, 1)));
                    this.mapPanel.Invalidate(this.map, new Rectangle(e.NewCell, new Size(1, 1)));
                }
            }
        }

        private void AddOverlay(Point location) {
            if((location.Y == 0) || (location.Y == (this.map.Metrics.Height - 1))) {
                return;
            }

            if(this.map.Overlay[location] == null) {
                if(this.SelectedOverlayType != null) {
                    var overlay = new Overlay {
                        Type = SelectedOverlayType,
                        Icon = 0
                    };
                    this.map.Overlay[location] = overlay;
                    this.mapPanel.Invalidate(this.map, location);

                    void undoAction(UndoRedoEventArgs e) {
                        e.MapPanel.Invalidate(e.Map, location);
                        e.Map.Overlay[location] = null;
                    }

                    void redoAction(UndoRedoEventArgs e) {
                        e.Map.Overlay[location] = overlay;
                        e.MapPanel.Invalidate(e.Map, location);
                    }

                    this.url.Track(undoAction, redoAction);

                    this.plugin.Dirty = true;
                }
            }
        }

        private void RemoveOverlay(Point location) {
            if((this.map.Overlay[location] is Overlay overlay) && overlay.Type.IsPlaceable) {
                this.map.Overlay[location] = null;
                this.mapPanel.Invalidate(this.map, location);

                void undoAction(UndoRedoEventArgs e) {
                    e.Map.Overlay[location] = overlay;
                    e.MapPanel.Invalidate(e.Map, location);
                }

                void redoAction(UndoRedoEventArgs e) {
                    e.MapPanel.Invalidate(e.Map, location);
                    e.Map.Overlay[location] = null;
                }

                this.url.Track(undoAction, redoAction);

                this.plugin.Dirty = true;
            }
        }

        private void EnterPlacementMode() {
            if(this.placementMode) {
                return;
            }

            this.placementMode = true;

            this.navigationWidget.MouseoverSize = Size.Empty;

            if(this.SelectedOverlayType != null) {
                this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
            }

            this.UpdateStatus();
        }

        private void ExitPlacementMode() {
            if(!this.placementMode) {
                return;
            }

            this.placementMode = false;

            this.navigationWidget.MouseoverSize = new Size(1, 1);

            if(this.SelectedOverlayType != null) {
                this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
            }

            this.UpdateStatus();
        }

        private void PickOverlay(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var overlay = this.map.Overlay[cell];
                if((overlay != null) && !overlay.Type.IsWall) {
                    this.SelectedOverlayType = overlay.Type;
                }
            }
        }

        private void RefreshMapPanel() => this.overlayTypeMapPanel.MapImage = this.SelectedOverlayType?.Thumbnail;

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to place overlay, Right-Click to remove overlay";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click or Right-Click to pick overlay";
            }
        }

        protected override void PreRenderMap() {
            base.PreRenderMap();

            this.previewMap = this.map.Clone();
            if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedOverlayType != null) {
                    if(this.previewMap.Metrics.GetCell(location, out var cell)) {
                        if(this.previewMap.Overlay[cell] == null) {
                            this.previewMap.Overlay[cell] = new Overlay {
                                Type = SelectedOverlayType,
                                Icon = 0,
                                Tint = Color.FromArgb(128, Color.White)
                            };
                        }
                    }
                }
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            var overlayPen = new Pen(Color.Green, 4.0f);
            foreach(var (cell, overlay) in this.previewMap.Overlay.Where(x => x.Value.Type.IsPlaceable)) {
                this.previewMap.Metrics.GetLocation(cell, out var topLeft);
                var bounds = new Rectangle(new Point(topLeft.X * Globals.TileWidth, topLeft.Y * Globals.TileHeight), Globals.TileSize);
                graphics.DrawRectangle(overlayPen, bounds);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected override void Dispose(bool disposing) {
            if(!this.disposedValue) {
                if(disposing) {
                    this.overlayTypeComboBox.SelectedIndexChanged -= this.OverlayTypeComboBox_SelectedIndexChanged;

                    this.mapPanel.MouseDown -= this.MapPanel_MouseDown;
                    this.mapPanel.MouseMove -= this.MapPanel_MouseMove;
                    (this.mapPanel as Control).KeyDown -= this.OverlaysTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.OverlaysTool_KeyUp;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion IDisposable Support
    }
}
