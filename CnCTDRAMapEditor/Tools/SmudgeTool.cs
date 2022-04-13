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
    public class SmudgeTool : ViewTool {
        private readonly TypeComboBox smudgeTypeComboBox;
        private readonly MapPanel smudgeTypeMapPanel;

        private Map previewMap;
        protected override Map RenderMap => this.previewMap;

        private bool placementMode;

        private SmudgeType selectedSmudgeType;
        private SmudgeType SelectedSmudgeType {
            get => this.selectedSmudgeType;
            set {
                if(this.selectedSmudgeType != value) {
                    if(this.placementMode && (this.selectedSmudgeType != null)) {
                        this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
                    }

                    this.selectedSmudgeType = value;
                    this.smudgeTypeComboBox.SelectedValue = this.selectedSmudgeType;

                    if(this.placementMode && (this.selectedSmudgeType != null)) {
                        this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
                    }

                    this.RefreshMapPanel();
                }
            }
        }

        public SmudgeTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, TypeComboBox smudgeTypeComboBox, MapPanel smudgeTypeMapPanel, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.previewMap = this.map;

            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.SmudgeTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.SmudgeTool_KeyUp;

            this.smudgeTypeComboBox = smudgeTypeComboBox;
            this.smudgeTypeComboBox.SelectedIndexChanged += this.SmudgeTypeComboBox_SelectedIndexChanged;

            this.smudgeTypeMapPanel = smudgeTypeMapPanel;
            this.smudgeTypeMapPanel.BackColor = Color.White;
            this.smudgeTypeMapPanel.MaxZoom = 1;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            this.SelectedSmudgeType = smudgeTypeComboBox.Types.First() as SmudgeType;

            this.UpdateStatus();
        }

        private void SmudgeTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) => this.SelectedSmudgeType = this.smudgeTypeComboBox.SelectedValue as SmudgeType;

        private void SmudgeTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.EnterPlacementMode();
            }
        }

        private void SmudgeTool_KeyUp(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.ExitPlacementMode();
            }
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.AddSmudge(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveSmudge(this.navigationWidget.MouseCell);
                }
            } else if((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right)) {
                this.PickSmudge(this.navigationWidget.MouseCell);
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
                if(this.SelectedSmudgeType != null) {
                    this.mapPanel.Invalidate(this.map, e.OldCell);
                    this.mapPanel.Invalidate(this.map, e.NewCell);
                }
            }
        }

        private void AddSmudge(Point location) {
            if(this.map.Smudge[location] == null) {
                if(this.SelectedSmudgeType != null) {
                    var smudge = new Smudge {
                        Type = SelectedSmudgeType,
                        Icon = 0,
                        Data = 0
                    };
                    this.map.Smudge[location] = smudge;
                    this.mapPanel.Invalidate(this.map, location);

                    void undoAction(UndoRedoEventArgs e) {
                        e.MapPanel.Invalidate(e.Map, location);
                        e.Map.Smudge[location] = null;
                    }

                    void redoAction(UndoRedoEventArgs e) {
                        e.Map.Smudge[location] = smudge;
                        e.MapPanel.Invalidate(e.Map, location);
                    }

                    this.url.Track(undoAction, redoAction);

                    this.plugin.Dirty = true;
                }
            }
        }

        private void RemoveSmudge(Point location) {
            if((this.map.Smudge[location] is Smudge smudge) && ((smudge.Type.Flag & SmudgeTypeFlag.Bib) == SmudgeTypeFlag.None)) {
                this.map.Smudge[location] = null;
                this.mapPanel.Invalidate(this.map, location);

                void undoAction(UndoRedoEventArgs e) {
                    e.Map.Smudge[location] = smudge;
                    e.MapPanel.Invalidate(e.Map, location);
                }

                void redoAction(UndoRedoEventArgs e) {
                    e.MapPanel.Invalidate(e.Map, location);
                    e.Map.Smudge[location] = null;
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

            if(this.SelectedSmudgeType != null) {
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

            if(this.SelectedSmudgeType != null) {
                this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
            }

            this.UpdateStatus();
        }

        private void PickSmudge(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var smudge = this.map.Smudge[cell];
                if(smudge != null) {
                    this.SelectedSmudgeType = smudge.Type;
                }
            }
        }

        private void RefreshMapPanel() => this.smudgeTypeMapPanel.MapImage = this.SelectedSmudgeType?.Thumbnail;

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to place smudge, Right-Click to remove smudge";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click or Right-Click to pick smudge";
            }
        }

        protected override void PreRenderMap() {
            base.PreRenderMap();

            this.previewMap = this.map.Clone();
            if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedSmudgeType != null) {
                    if(this.previewMap.Metrics.GetCell(location, out var cell)) {
                        if(this.previewMap.Smudge[cell] == null) {
                            this.previewMap.Smudge[cell] = new Smudge { Type = SelectedSmudgeType, Data = 0, Tint = Color.FromArgb(128, Color.White) };
                        }
                    }
                }
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            var smudgePen = new Pen(Color.Green, 4.0f);
            foreach(var (cell, smudge) in this.previewMap.Smudge.Where(x => (x.Value.Type.Flag & SmudgeTypeFlag.Bib) == SmudgeTypeFlag.None)) {
                this.previewMap.Metrics.GetLocation(cell, out var topLeft);
                var bounds = new Rectangle(new Point(topLeft.X * Globals.TileWidth, topLeft.Y * Globals.TileHeight), Globals.TileSize);
                graphics.DrawRectangle(smudgePen, bounds);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected override void Dispose(bool disposing) {
            if(!this.disposedValue) {
                if(disposing) {
                    this.smudgeTypeComboBox.SelectedIndexChanged -= this.SmudgeTypeComboBox_SelectedIndexChanged;

                    this.mapPanel.MouseDown -= this.MapPanel_MouseDown;
                    this.mapPanel.MouseMove -= this.MapPanel_MouseMove;
                    (this.mapPanel as Control).KeyDown -= this.SmudgeTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.SmudgeTool_KeyUp;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
