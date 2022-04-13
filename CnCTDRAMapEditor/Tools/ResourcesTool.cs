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
    public class ResourcesTool : ViewTool {
        private readonly Label totalResourcesLbl;
        private readonly NumericUpDown brushSizeNud;
        private readonly CheckBox gemsCheckBox;

        private bool placementMode;
        private bool additivePlacement;

        private readonly Dictionary<int, Overlay> undoOverlays = new Dictionary<int, Overlay>();
        private readonly Dictionary<int, Overlay> redoOverlays = new Dictionary<int, Overlay>();

        public ResourcesTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, Label totalResourcesLbl, NumericUpDown brushSizeNud, CheckBox gemsCheckBox, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseUp += this.MapPanel_MouseUp;
            (this.mapPanel as Control).KeyDown += this.ResourceTool_KeyDown;

            this.totalResourcesLbl = totalResourcesLbl;
            this.brushSizeNud = brushSizeNud;
            this.gemsCheckBox = gemsCheckBox;

            this.brushSizeNud.ValueChanged += this.BrushSizeNud_ValueChanged;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;
            this.navigationWidget.MouseoverSize = new Size((int)brushSizeNud.Value, (int)brushSizeNud.Value);

            url.Undone += this.Url_UndoRedo;
            url.Redone += this.Url_UndoRedo;

            this.Update();

            this.UpdateStatus();
        }

        private void Url_UndoRedo(object sender, EventArgs e) => this.Update();

        private void BrushSizeNud_ValueChanged(object sender, EventArgs e) => this.navigationWidget.MouseoverSize = new Size((int)this.brushSizeNud.Value, (int)this.brushSizeNud.Value);

        private void ResourceTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.OemOpenBrackets) {
                this.brushSizeNud.DownButton();
                this.mapPanel.Invalidate();
            } else if(e.KeyCode == Keys.OemCloseBrackets) {
                this.brushSizeNud.UpButton();
                this.mapPanel.Invalidate();
            }
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                if(!this.placementMode) {
                    this.EnterPlacementMode(true);
                    this.AddResource(this.navigationWidget.MouseCell);
                }
            } else if(e.Button == MouseButtons.Right) {
                if(!this.placementMode) {
                    this.EnterPlacementMode(false);
                    this.RemoveResource(this.navigationWidget.MouseCell);
                }
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(((e.Button == MouseButtons.Left) && this.additivePlacement) ||
                    ((e.Button == MouseButtons.Right) && !this.additivePlacement)) {
                    this.ExitPlacementMode();
                }
            }

            if((this.undoOverlays.Count > 0) || (this.redoOverlays.Count > 0)) {
                this.CommitChange();
            }
        }

        private void MouseoverWidget_MouseCellChanged(object sender, MouseCellChangedEventArgs e) {
            if(this.placementMode) {
                if(this.additivePlacement) {
                    this.AddResource(e.NewCell);
                } else {
                    this.RemoveResource(e.NewCell);
                }
            }

            if(this.brushSizeNud.Value > 1) {
                foreach(var cell in new Point[] { e.OldCell, e.NewCell }) {
                    this.mapPanel.Invalidate(this.mapPanel.MapToClient(new Rectangle(
                        new Point(cell.X - ((int)this.brushSizeNud.Value / 2), cell.Y - ((int)this.brushSizeNud.Value / 2)),
                        new Size((int)this.brushSizeNud.Value, (int)this.brushSizeNud.Value)
                    )));
                }
            }
        }

        private void AddResource(Point location) {
            var rectangle = new Rectangle(location, new Size(1, 1));
            rectangle.Inflate(this.navigationWidget.MouseoverSize.Width / 2, this.navigationWidget.MouseoverSize.Height / 2);
            foreach(var subLocation in rectangle.Points()) {
                if((subLocation.Y == 0) || (subLocation.Y == (this.map.Metrics.Height - 1))) {
                    continue;
                }

                if(this.map.Metrics.GetCell(subLocation, out var cell)) {
                    if(this.map.Overlay[cell] == null) {
                        var resourceType = this.gemsCheckBox.Checked ?
                            this.map.OverlayTypes.Where(t => t.IsGem).FirstOrDefault() :
                            this.map.OverlayTypes.Where(t => t.IsTiberiumOrGold).FirstOrDefault();
                        if(resourceType != null) {
                            if(!this.undoOverlays.ContainsKey(cell)) {
                                this.undoOverlays[cell] = this.map.Overlay[cell];
                            }

                            var overlay = new Overlay { Type = resourceType, Icon = 0 };
                            this.map.Overlay[cell] = overlay;
                            this.redoOverlays[cell] = overlay;

                            this.plugin.Dirty = true;
                        }
                    }
                }
            }

            rectangle.Inflate(1, 1);
            this.mapPanel.Invalidate(this.map, rectangle);

            this.Update();
        }

        private void RemoveResource(Point location) {
            var rectangle = new Rectangle(location, new Size(1, 1));
            rectangle.Inflate(this.navigationWidget.MouseoverSize.Width / 2, this.navigationWidget.MouseoverSize.Height / 2);
            foreach(var subLocation in rectangle.Points()) {
                if(this.map.Metrics.GetCell(subLocation, out var cell)) {
                    if(this.map.Overlay[cell]?.Type.IsResource ?? false) {
                        if(!this.undoOverlays.ContainsKey(cell)) {
                            this.undoOverlays[cell] = this.map.Overlay[cell];
                        }

                        this.map.Overlay[cell] = null;
                        this.redoOverlays[cell] = null;

                        this.plugin.Dirty = true;
                    }
                }
            }

            rectangle.Inflate(1, 1);
            this.mapPanel.Invalidate(this.map, rectangle);

            this.Update();
        }

        private void EnterPlacementMode(bool additive) {
            if(this.placementMode) {
                return;
            }

            this.placementMode = true;
            this.additivePlacement = additive;

            this.UpdateStatus();
        }

        private void ExitPlacementMode() {
            if(!this.placementMode) {
                return;
            }

            this.placementMode = false;

            this.UpdateStatus();
        }

        private void CommitChange() {
            var undoOverlays2 = new Dictionary<int, Overlay>(this.undoOverlays);
            void undoAction(UndoRedoEventArgs e) {
                foreach(var kv in undoOverlays2) {
                    e.Map.Overlay[kv.Key] = kv.Value;
                }
                e.MapPanel.Invalidate(e.Map, undoOverlays2.Keys.Select(k => {
                    e.Map.Metrics.GetLocation(k, out var location);
                    var rectangle = new Rectangle(location, new Size(1, 1));
                    rectangle.Inflate(1, 1);
                    return rectangle;
                }));
            }

            var redoOverlays2 = new Dictionary<int, Overlay>(this.redoOverlays);
            void redoAction(UndoRedoEventArgs e) {
                foreach(var kv in redoOverlays2) {
                    e.Map.Overlay[kv.Key] = kv.Value;
                }
                e.MapPanel.Invalidate(e.Map, redoOverlays2.Keys.Select(k => {
                    e.Map.Metrics.GetLocation(k, out var location);
                    var rectangle = new Rectangle(location, new Size(1, 1));
                    rectangle.Inflate(1, 1);
                    return rectangle;
                }));
            }

            this.undoOverlays.Clear();
            this.redoOverlays.Clear();

            this.url.Track(undoAction, redoAction);
        }

        private void Update() {
            this.totalResourcesLbl.Text = this.map.TotalResources.ToString();

            if(this.map.OverlayTypes.Any(t => t.IsGem)) {
                this.gemsCheckBox.Visible = true;
            } else {
                this.gemsCheckBox.Visible = false;
                this.gemsCheckBox.Checked = false;
            }
        }

        private void UpdateStatus() {
            if(this.placementMode) {
                if(this.additivePlacement) {
                    this.statusLbl.Text = "Drag mouse to add resources";
                } else {
                    this.statusLbl.Text = "Drag mouse to remove resources";
                }
            } else {
                this.statusLbl.Text = "Left-Click drag to add resources, Right-Click drag to remove resources";
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            var resourcePen = new Pen(Color.Green, 4.0f);
            foreach(var (cell, overlay) in this.map.Overlay) {
                if(overlay.Type.IsResource) {
                    this.map.Metrics.GetLocation(cell, out var topLeft);
                    var bounds = new Rectangle(new Point(topLeft.X * Globals.TileWidth, topLeft.Y * Globals.TileHeight), Globals.TileSize);
                    graphics.DrawRectangle(resourcePen, bounds);
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
                    (this.mapPanel as Control).KeyDown -= this.ResourceTool_KeyDown;

                    this.brushSizeNud.ValueChanged -= this.BrushSizeNud_ValueChanged;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;

                    this.url.Undone -= this.Url_UndoRedo;
                    this.url.Redone -= this.Url_UndoRedo;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion IDisposable Support
    }
}
