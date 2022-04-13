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
using MobiusEditor.Render;
using MobiusEditor.Utility;
using MobiusEditor.Widgets;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MobiusEditor.Tools {
    public class BuildingTool : ViewTool {
        private readonly TypeComboBox buildingTypeComboBox;
        private readonly MapPanel buildingTypeMapPanel;
        private readonly ObjectProperties objectProperties;

        private Map previewMap;
        protected override Map RenderMap => this.previewMap;

        private bool placementMode;

        private readonly Building mockBuilding;

        private Building selectedBuilding;
        private ObjectPropertiesPopup selectedObjectProperties;
        private Point selectedBuildingPivot;

        private BuildingType selectedBuildingType;
        private BuildingType SelectedBuildingType {
            get => this.selectedBuildingType;
            set {
                if(this.selectedBuildingType != value) {
                    if(this.placementMode && (this.selectedBuildingType != null)) {
                        this.mapPanel.Invalidate(this.map, new Rectangle(this.navigationWidget.MouseCell, this.selectedBuildingType.OverlapBounds.Size));
                    }

                    this.selectedBuildingType = value;
                    this.buildingTypeComboBox.SelectedValue = this.selectedBuildingType;

                    if(this.placementMode && (this.selectedBuildingType != null)) {
                        this.mapPanel.Invalidate(this.map, new Rectangle(this.navigationWidget.MouseCell, this.selectedBuildingType.OverlapBounds.Size));
                    }

                    this.mockBuilding.Type = this.selectedBuildingType;

                    this.RefreshMapPanel();
                }
            }
        }

        public BuildingTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, TypeComboBox buildingTypeComboBox, MapPanel buildingTypeMapPanel, ObjectProperties objectProperties, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.previewMap = this.map;

            this.mockBuilding = new Building() {
                Type = buildingTypeComboBox.Types.First() as BuildingType,
                House = this.map.Houses.First().Type,
                Strength = 256,
                Direction = this.map.DirectionTypes.Where(d => d.Equals(FacingType.North)).First()
            };
            this.mockBuilding.PropertyChanged += this.MockBuilding_PropertyChanged;

            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseUp += this.MapPanel_MouseUp;
            this.mapPanel.MouseDoubleClick += this.MapPanel_MouseDoubleClick;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.UnitTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.UnitTool_KeyUp;

            this.buildingTypeComboBox = buildingTypeComboBox;
            this.buildingTypeComboBox.SelectedIndexChanged += this.UnitTypeComboBox_SelectedIndexChanged;

            this.buildingTypeMapPanel = buildingTypeMapPanel;
            this.buildingTypeMapPanel.BackColor = Color.White;
            this.buildingTypeMapPanel.MaxZoom = 1;

            this.objectProperties = objectProperties;
            this.objectProperties.Object = this.mockBuilding;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            this.SelectedBuildingType = this.mockBuilding.Type;

            this.UpdateStatus();
        }

        private void MapPanel_MouseDoubleClick(object sender, MouseEventArgs e) {
            if(Control.ModifierKeys != Keys.None) {
                return;
            }

            if(this.map.Metrics.GetCell(this.navigationWidget.MouseCell, out var cell)) {
                if(this.map.Technos[cell] is Building building) {
                    this.selectedBuilding = null;
                    this.selectedBuildingPivot = Point.Empty;

                    this.selectedObjectProperties?.Close();
                    this.selectedObjectProperties = new ObjectPropertiesPopup(this.objectProperties.Plugin, building);
                    this.selectedObjectProperties.Closed += (cs, ce) => {
                        this.navigationWidget.Refresh();
                    };

                    building.PropertyChanged += this.SelectedBuilding_PropertyChanged;

                    this.selectedObjectProperties.Show(this.mapPanel, this.mapPanel.PointToClient(Control.MousePosition));

                    this.UpdateStatus();
                }
            }
        }

        private void MockBuilding_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if((this.mockBuilding.Type == null) || !this.mockBuilding.Type.HasTurret) {
                this.mockBuilding.Direction = this.map.DirectionTypes.Where(d => d.Equals(FacingType.North)).First();
            }

            this.RefreshMapPanel();
        }

        private void SelectedBuilding_PropertyChanged(object sender, PropertyChangedEventArgs e) => this.mapPanel.Invalidate(this.map, sender as Building);

        private void UnitTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) => this.SelectedBuildingType = this.buildingTypeComboBox.SelectedValue as BuildingType;

        private void UnitTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.EnterPlacementMode();
            }
        }

        private void UnitTool_KeyUp(object sender, KeyEventArgs e) {
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

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.AddBuilding(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveBuilding(this.navigationWidget.MouseCell);
                }
            } else if(e.Button == MouseButtons.Left) {
                this.SelectBuilding(this.navigationWidget.MouseCell);
            } else if(e.Button == MouseButtons.Right) {
                this.PickBuilding(this.navigationWidget.MouseCell);
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e) {
            if(this.selectedBuilding != null) {
                this.selectedBuilding = null;
                this.selectedBuildingPivot = Point.Empty;

                this.UpdateStatus();
            }
        }

        private void MouseoverWidget_MouseCellChanged(object sender, MouseCellChangedEventArgs e) {
            if(this.placementMode) {
                if(this.SelectedBuildingType != null) {
                    this.mapPanel.Invalidate(this.map, new Rectangle(e.OldCell, this.SelectedBuildingType.OverlapBounds.Size));
                    this.mapPanel.Invalidate(this.map, new Rectangle(e.NewCell, this.SelectedBuildingType.OverlapBounds.Size));
                }
            } else if(this.selectedBuilding != null) {
                var oldLocation = this.map.Technos[this.selectedBuilding].Value;
                var newLocation = new Point(Math.Max(0, e.NewCell.X - this.selectedBuildingPivot.X), Math.Max(0, e.NewCell.Y - this.selectedBuildingPivot.Y));
                this.mapPanel.Invalidate(this.map, this.selectedBuilding);
                this.map.Buildings.Remove(this.selectedBuilding);
                if(this.map.Technos.CanAdd(newLocation, this.selectedBuilding, this.selectedBuilding.Type.BaseOccupyMask) &&
                    this.map.Buildings.Add(newLocation, this.selectedBuilding)) {
                    this.mapPanel.Invalidate(this.map, this.selectedBuilding);
                    this.plugin.Dirty = true;
                } else {
                    this.map.Buildings.Add(oldLocation, this.selectedBuilding);
                }
            }
        }

        private void AddBuilding(Point location) {
            if(this.SelectedBuildingType != null) {
                var building = this.mockBuilding.Clone();
                if(this.map.Technos.CanAdd(location, building, building.Type.BaseOccupyMask) && this.map.Buildings.Add(location, building)) {
                    if(building.BasePriority >= 0) {
                        foreach(var baseBuilding in this.map.Buildings.OfType<Building>().Select(x => x.Occupier).Where(x => x.BasePriority >= 0)) {
                            if((building != baseBuilding) && (baseBuilding.BasePriority >= building.BasePriority)) {
                                baseBuilding.BasePriority++;
                            }
                        }

                        var baseBuildings = this.map.Buildings.OfType<Building>().Select(x => x.Occupier).Where(x => x.BasePriority >= 0).OrderBy(x => x.BasePriority).ToArray();
                        for(var i = 0; i < baseBuildings.Length; ++i) {
                            baseBuildings[i].BasePriority = i;
                        }

                        foreach(var baseBuilding in this.map.Buildings.OfType<Building>().Select(x => x.Occupier).Where(x => x.BasePriority >= 0)) {
                            this.mapPanel.Invalidate(this.map, baseBuilding);
                        }
                    }

                    this.mapPanel.Invalidate(this.map, building);

                    this.plugin.Dirty = true;
                }
            }
        }

        private void RemoveBuilding(Point location) {
            if(this.map.Technos[location] is Building building) {
                this.mapPanel.Invalidate(this.map, building);
                this.map.Buildings.Remove(building);

                if(building.BasePriority >= 0) {
                    var baseBuildings = this.map.Buildings.OfType<Building>().Select(x => x.Occupier).Where(x => x.BasePriority >= 0).OrderBy(x => x.BasePriority).ToArray();
                    for(var i = 0; i < baseBuildings.Length; ++i) {
                        baseBuildings[i].BasePriority = i;
                    }

                    foreach(var baseBuilding in this.map.Buildings.OfType<Building>().Select(x => x.Occupier).Where(x => x.BasePriority >= 0)) {
                        this.mapPanel.Invalidate(this.map, baseBuilding);
                    }
                }

                this.plugin.Dirty = true;
            }
        }

        private void EnterPlacementMode() {
            if(this.placementMode) {
                return;
            }

            this.placementMode = true;

            this.navigationWidget.MouseoverSize = Size.Empty;

            if(this.SelectedBuildingType != null) {
                this.mapPanel.Invalidate(this.map, new Rectangle(this.navigationWidget.MouseCell, this.SelectedBuildingType.OverlapBounds.Size));
            }

            this.UpdateStatus();
        }

        private void ExitPlacementMode() {
            if(!this.placementMode) {
                return;
            }

            this.placementMode = false;

            this.navigationWidget.MouseoverSize = new Size(1, 1);

            if(this.SelectedBuildingType != null) {
                this.mapPanel.Invalidate(this.map, new Rectangle(this.navigationWidget.MouseCell, this.SelectedBuildingType.OverlapBounds.Size));
            }

            this.UpdateStatus();
        }

        private void PickBuilding(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                if(this.map.Technos[cell] is Building building) {
                    this.SelectedBuildingType = building.Type;
                    this.mockBuilding.House = building.House;
                    this.mockBuilding.Strength = building.Strength;
                    this.mockBuilding.Direction = building.Direction;
                    this.mockBuilding.Trigger = building.Trigger;
                    this.mockBuilding.BasePriority = building.BasePriority;
                    this.mockBuilding.IsPrebuilt = building.IsPrebuilt;
                    this.mockBuilding.Sellable = building.Sellable;
                    this.mockBuilding.Rebuild = building.Rebuild;
                }
            }
        }

        private void SelectBuilding(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                this.selectedBuilding = this.map.Technos[cell] as Building;
                this.selectedBuildingPivot = (this.selectedBuilding != null) ? (location - (Size)this.map.Technos[this.selectedBuilding].Value) : Point.Empty;
            }

            this.UpdateStatus();
        }

        private void RefreshMapPanel() {
            if(this.mockBuilding.Type != null) {
                var render = MapRenderer.Render(this.plugin.GameType, this.map.Theater, new Point(0, 0), Globals.TileSize, Globals.TileScale, this.mockBuilding);
                if(!render.Item1.IsEmpty) {
                    var buildingPreview = new Bitmap(render.Item1.Width, render.Item1.Height);
                    using(var g = Graphics.FromImage(buildingPreview)) {
                        render.Item2(g);
                    }
                    this.buildingTypeMapPanel.MapImage = buildingPreview;
                } else {
                    this.buildingTypeMapPanel.MapImage = null;
                }
            } else {
                this.buildingTypeMapPanel.MapImage = null;
            }
        }

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to place building, Right-Click to remove building";
            } else if(this.selectedBuilding != null) {
                this.statusLbl.Text = "Drag mouse to move building";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click drag to move building, Double-Click update building properties, Right-Click to pick building";
            }
        }

        protected override void PreRenderMap() {
            base.PreRenderMap();

            this.previewMap = this.map.Clone();
            if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedBuildingType != null) {
                    var building = this.mockBuilding.Clone();
                    building.Tint = Color.FromArgb(128, Color.White);
                    if(this.previewMap.Technos.CanAdd(location, building, building.Type.BaseOccupyMask) && this.previewMap.Buildings.Add(location, building)) {
                        this.mapPanel.Invalidate(this.previewMap, building);
                    }
                }
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            var buildingPen = new Pen(Color.Green, 4.0f);
            var occupyPen = new Pen(Color.Red, 2.0f);
            foreach(var (topLeft, building) in this.map.Buildings.OfType<Building>()) {
                var bounds = new Rectangle(
                    new Point(topLeft.X * Globals.TileWidth, topLeft.Y * Globals.TileHeight),
                    new Size(building.Type.Size.Width * Globals.TileWidth, building.Type.Size.Height * Globals.TileHeight)
                );
                graphics.DrawRectangle(buildingPen, bounds);

                for(var y = 0; y < building.Type.BaseOccupyMask.GetLength(0); ++y) {
                    for(var x = 0; x < building.Type.BaseOccupyMask.GetLength(1); ++x) {
                        if(building.Type.BaseOccupyMask[y, x]) {
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
                    this.mapPanel.MouseDown -= this.MapPanel_MouseDown;
                    this.mapPanel.MouseUp -= this.MapPanel_MouseUp;
                    this.mapPanel.MouseDoubleClick -= this.MapPanel_MouseDoubleClick;
                    this.mapPanel.MouseMove -= this.MapPanel_MouseMove;
                    (this.mapPanel as Control).KeyDown -= this.UnitTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.UnitTool_KeyUp;

                    this.buildingTypeComboBox.SelectedIndexChanged -= this.UnitTypeComboBox_SelectedIndexChanged;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion IDisposable Support
    }
}
