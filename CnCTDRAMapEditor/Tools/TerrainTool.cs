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
    public class TerrainTool : ViewTool {
        private readonly ListView terrainTypeListView;
        private readonly TerrainProperties terrainProperties;

        private Map previewMap;
        protected override Map RenderMap => this.previewMap;

        private bool placementMode;

        private readonly Terrain mockTerrain;

        private Terrain selectedTerrain;
        private Point selectedTerrainPivot;

        private TerrainType selectedTerrainType;
        private TerrainPropertiesPopup selectedTerrainProperties;
        private TerrainType SelectedTerrainType {
            get => this.selectedTerrainType;
            set {
                if(this.selectedTerrainType != value) {
                    if(this.placementMode && (this.selectedTerrainType != null)) {
                        this.mapPanel.Invalidate(this.map, new Rectangle(this.navigationWidget.MouseCell, this.selectedTerrainType.OverlapBounds.Size));
                    }

                    this.selectedTerrainType = value;

                    this.terrainTypeListView.BeginUpdate();
                    this.terrainTypeListView.SelectedIndexChanged -= this.TerrainTypeCombo_SelectedIndexChanged;
                    foreach(ListViewItem item in this.terrainTypeListView.Items) {
                        item.Selected = item.Tag == this.selectedTerrainType;
                    }
                    if(this.terrainTypeListView.SelectedIndices.Count > 0) {
                        this.terrainTypeListView.EnsureVisible(this.terrainTypeListView.SelectedIndices[0]);
                    }
                    this.terrainTypeListView.SelectedIndexChanged += this.TerrainTypeCombo_SelectedIndexChanged;
                    this.terrainTypeListView.EndUpdate();

                    if(this.placementMode && (this.selectedTerrainType != null)) {
                        this.mapPanel.Invalidate(this.map, new Rectangle(this.navigationWidget.MouseCell, this.selectedTerrainType.OverlapBounds.Size));
                    }

                    this.mockTerrain.Type = this.selectedTerrainType;
                    if(this.selectedTerrainType != null) {
                        this.mockTerrain.Icon = this.selectedTerrainType.IsTransformable ? 22 : 0;
                    }
                }
            }
        }

        public TerrainTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, ListView terrainTypeListView, TerrainProperties terrainProperties, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.previewMap = this.map;

            this.mockTerrain = new Terrain();

            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            this.mapPanel.MouseUp += this.MapPanel_MouseUp;
            this.mapPanel.MouseDoubleClick += this.MapPanel_MouseDoubleClick;
            (this.mapPanel as Control).KeyDown += this.TerrainTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.TerrainTool_KeyUp;

            this.terrainTypeListView = terrainTypeListView;
            this.terrainTypeListView.SelectedIndexChanged += this.TerrainTypeCombo_SelectedIndexChanged;

            var terrainTypes = plugin.Map.TerrainTypes.Where(t => t.Theaters.Contains(plugin.Map.Theater)).OrderBy(t => t.Name);
            var terrainTypeImages = terrainTypes.Select(t => t.Thumbnail);

            var maxWidth = terrainTypeImages.Max(t => t.Width);
            var maxHeight = terrainTypeImages.Max(t => t.Height);

            var imageList = new ImageList();
            imageList.Images.AddRange(terrainTypeImages.ToArray());
            imageList.ImageSize = new Size(maxWidth / 2, maxHeight / 2);
            imageList.ColorDepth = ColorDepth.Depth24Bit;

            this.terrainTypeListView.BeginUpdate();
            this.terrainTypeListView.Items.Clear();
            this.terrainTypeListView.SmallImageList = imageList;

            var imageIndex = 0;
            foreach(var templateType in terrainTypes) {
                var item = new ListViewItem(templateType.DisplayName, imageIndex++) {
                    Tag = templateType
                };
                this.terrainTypeListView.Items.Add(item);
            }
            this.terrainTypeListView.EndUpdate();

            this.terrainProperties = terrainProperties;
            this.terrainProperties.Terrain = this.mockTerrain;
            this.terrainProperties.Enabled = plugin.GameType == GameType.TiberianDawn;
            this.terrainProperties.Initialize(plugin, true);

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            this.SelectedTerrainType = terrainTypes.First();

            this.UpdateStatus();
        }

        private void MapPanel_MouseDoubleClick(object sender, MouseEventArgs e) {
            if(Control.ModifierKeys != Keys.None) {
                return;
            }

            if(this.map.Metrics.GetCell(this.navigationWidget.MouseCell, out var cell)) {
                if(this.map.Technos[cell] is Terrain terrain) {
                    this.selectedTerrain = null;

                    this.selectedTerrainProperties?.Close();
                    this.selectedTerrainProperties = new TerrainPropertiesPopup(this.terrainProperties.Plugin, terrain);
                    this.selectedTerrainProperties.Closed += (cs, ce) => {
                        this.navigationWidget.Refresh();
                    };

                    this.selectedTerrainProperties.Show(this.mapPanel, this.mapPanel.PointToClient(Control.MousePosition));

                    this.UpdateStatus();
                }
            }
        }

        private void TerrainTypeCombo_SelectedIndexChanged(object sender, EventArgs e) {
            this.SelectedTerrainType = (this.terrainTypeListView.SelectedItems.Count > 0) ? (this.terrainTypeListView.SelectedItems[0].Tag as TerrainType) : null;
        }

        private void TerrainTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.EnterPlacementMode();
            }
        }

        private void TerrainTool_KeyUp(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.ExitPlacementMode();
            }
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.AddTerrain(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveTerrain(this.navigationWidget.MouseCell);
                }
            } else if(e.Button == MouseButtons.Left) {
                this.SelectTerrain(this.navigationWidget.MouseCell);
            } else if(e.Button == MouseButtons.Right) {
                this.PickTerrain(this.navigationWidget.MouseCell);
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e) {
            if(this.selectedTerrain != null) {
                this.selectedTerrain = null;
                this.selectedTerrainPivot = Point.Empty;

                this.UpdateStatus();
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
                if(this.SelectedTerrainType != null) {
                    this.mapPanel.Invalidate(this.map, new Rectangle(e.OldCell, this.SelectedTerrainType.OverlapBounds.Size));
                    this.mapPanel.Invalidate(this.map, new Rectangle(e.NewCell, this.SelectedTerrainType.OverlapBounds.Size));
                }
            } else if(this.selectedTerrain != null) {
                var oldLocation = this.map.Technos[this.selectedTerrain].Value;
                var newLocation = new Point(Math.Max(0, e.NewCell.X - this.selectedTerrainPivot.X), Math.Max(0, e.NewCell.Y - this.selectedTerrainPivot.Y));
                this.mapPanel.Invalidate(this.map, this.selectedTerrain);
                this.map.Technos.Remove(this.selectedTerrain);
                if(this.map.Technos.Add(newLocation, this.selectedTerrain)) {
                    this.mapPanel.Invalidate(this.map, this.selectedTerrain);
                } else {
                    this.map.Technos.Add(oldLocation, this.selectedTerrain);
                }
            }
        }

        private void AddTerrain(Point location) {
            if(!this.map.Metrics.Contains(location)) {
                return;
            }

            if(this.SelectedTerrainType != null) {
                var terrain = this.mockTerrain.Clone();
                if(this.map.Technos.Add(location, terrain)) {
                    this.mapPanel.Invalidate(this.map, terrain);

                    void undoAction(UndoRedoEventArgs e) {
                        e.MapPanel.Invalidate(e.Map, location);
                        e.Map.Technos.Remove(terrain);
                    }

                    void redoAction(UndoRedoEventArgs e) {
                        e.Map.Technos.Add(location, terrain);
                        e.MapPanel.Invalidate(e.Map, location);
                    }

                    this.url.Track(undoAction, redoAction);

                    this.plugin.Dirty = true;
                }
            }
        }

        private void RemoveTerrain(Point location) {
            if(this.map.Technos[location] is Terrain terrain) {
                this.mapPanel.Invalidate(this.map, terrain);
                this.map.Technos.Remove(location);

                void undoAction(UndoRedoEventArgs e) {
                    e.Map.Technos.Add(location, terrain);
                    e.MapPanel.Invalidate(e.Map, location);
                }

                void redoAction(UndoRedoEventArgs e) {
                    e.MapPanel.Invalidate(e.Map, location);
                    e.Map.Technos.Remove(terrain);
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

            if(this.SelectedTerrainType != null) {
                this.mapPanel.Invalidate(this.map, new Rectangle(this.navigationWidget.MouseCell, this.selectedTerrainType.OverlapBounds.Size));
            }

            this.UpdateStatus();
        }

        private void ExitPlacementMode() {
            if(!this.placementMode) {
                return;
            }

            this.placementMode = false;

            this.navigationWidget.MouseoverSize = new Size(1, 1);

            if(this.SelectedTerrainType != null) {
                this.mapPanel.Invalidate(this.map, new Rectangle(this.navigationWidget.MouseCell, this.selectedTerrainType.OverlapBounds.Size));
            }

            this.UpdateStatus();
        }

        private void PickTerrain(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                if(this.map.Technos[cell] is Terrain terrain) {
                    this.SelectedTerrainType = terrain.Type;
                    this.mockTerrain.Trigger = terrain.Trigger;
                }
            }
        }

        private void SelectTerrain(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                this.selectedTerrain = this.map.Technos[cell] as Terrain;
                this.selectedTerrainPivot = (this.selectedTerrain != null) ? (location - (Size)this.map.Technos[this.selectedTerrain].Value) : Point.Empty;
            }

            this.UpdateStatus();
        }

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to place terrain, Right-Click to remove terrain";
            } else if(this.selectedTerrain != null) {
                this.statusLbl.Text = "Drag mouse to move terrain";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click drag to move terrain, Double-Click update terrain properties, Right-Click to pick terrain";
            }
        }

        protected override void PreRenderMap() {
            base.PreRenderMap();

            this.previewMap = this.map.Clone();
            if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedTerrainType != null) {
                    if(this.previewMap.Metrics.Contains(location)) {
                        var terrain = new Terrain {
                            Type = SelectedTerrainType,
                            Icon = this.SelectedTerrainType.IsTransformable ? 22 : 0,
                            Tint = Color.FromArgb(128, Color.White)
                        };
                        this.previewMap.Technos.Add(location, terrain);
                    }
                }
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            var terrainPen = new Pen(Color.Green, 4.0f);
            var occupyPen = new Pen(Color.Red, 2.0f);
            foreach(var (topLeft, terrain) in this.previewMap.Technos.OfType<Terrain>()) {
                var bounds = new Rectangle(new Point(topLeft.X * Globals.TileWidth, topLeft.Y * Globals.TileHeight), terrain.Type.RenderSize);
                graphics.DrawRectangle(terrainPen, bounds);

                for(var y = 0; y < terrain.Type.OccupyMask.GetLength(0); ++y) {
                    for(var x = 0; x < terrain.Type.OccupyMask.GetLength(1); ++x) {
                        if(terrain.Type.OccupyMask[y, x]) {
                            var occupyBounds = new Rectangle(
                                new Point((topLeft.X + x) * Globals.TileWidth, (topLeft.Y + y) * Globals.TileHeight),
                                Globals.TileSize
                            );
                            graphics.DrawRectangle(occupyPen, occupyBounds);
                        }
                    }
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected override void Dispose(bool disposing) {
            if(!this.disposedValue) {
                if(disposing) {
                    this.selectedTerrainProperties?.Close();

                    this.mapPanel.MouseDown -= this.MapPanel_MouseDown;
                    this.mapPanel.MouseMove -= this.MapPanel_MouseMove;
                    this.mapPanel.MouseUp -= this.MapPanel_MouseUp;
                    this.mapPanel.MouseDoubleClick -= this.MapPanel_MouseDoubleClick;
                    (this.mapPanel as Control).KeyDown -= this.TerrainTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.TerrainTool_KeyUp;

                    this.terrainTypeListView.SelectedIndexChanged -= this.TerrainTypeCombo_SelectedIndexChanged;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion IDisposable Support
    }
}
