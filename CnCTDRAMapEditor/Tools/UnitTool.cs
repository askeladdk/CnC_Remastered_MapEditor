﻿//
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
    public class UnitTool : ViewTool {
        private readonly TypeComboBox unitTypeComboBox;
        private readonly MapPanel unitTypeMapPanel;
        private readonly ObjectProperties objectProperties;

        private Map previewMap;
        protected override Map RenderMap => this.previewMap;

        private bool placementMode;

        private readonly Unit mockUnit;

        private Unit selectedUnit;
        private ObjectPropertiesPopup selectedObjectProperties;

        private UnitType selectedUnitType;
        private UnitType SelectedUnitType {
            get => this.selectedUnitType;
            set {
                if(this.selectedUnitType != value) {
                    if(this.placementMode && (this.selectedUnitType != null)) {
                        this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(this.navigationWidget.MouseCell, new Size(1, 1)), 1, 1));
                    }

                    this.selectedUnitType = value;
                    this.unitTypeComboBox.SelectedValue = this.selectedUnitType;

                    if(this.placementMode && (this.selectedUnitType != null)) {
                        this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(this.navigationWidget.MouseCell, new Size(1, 1)), 1, 1));
                    }

                    this.mockUnit.Type = this.selectedUnitType;

                    this.RefreshMapPanel();
                }
            }
        }

        public UnitTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, TypeComboBox unitTypeComboBox, MapPanel unitTypeMapPanel, ObjectProperties objectProperties, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.previewMap = this.map;

            this.mockUnit = new Unit() {
                Type = unitTypeComboBox.Types.First() as UnitType,
                House = this.map.Houses.First().Type,
                Strength = 256,
                Direction = this.map.DirectionTypes.Where(d => d.Equals(FacingType.North)).First(),
                Mission = this.map.MissionTypes.Where(m => m.Equals("Guard")).FirstOrDefault() ?? this.map.MissionTypes.First()
            };
            this.mockUnit.PropertyChanged += this.MockUnit_PropertyChanged;

            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseUp += this.MapPanel_MouseUp;
            this.mapPanel.MouseDoubleClick += this.MapPanel_MouseDoubleClick;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.UnitTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.UnitTool_KeyUp;

            this.unitTypeComboBox = unitTypeComboBox;
            this.unitTypeComboBox.SelectedIndexChanged += this.UnitTypeComboBox_SelectedIndexChanged;

            this.unitTypeMapPanel = unitTypeMapPanel;
            this.unitTypeMapPanel.BackColor = Color.White;
            this.unitTypeMapPanel.MaxZoom = 1;

            this.objectProperties = objectProperties;
            this.objectProperties.Object = this.mockUnit;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            this.SelectedUnitType = this.mockUnit.Type;

            this.UpdateStatus();
        }

        private void MapPanel_MouseDoubleClick(object sender, MouseEventArgs e) {
            if(Control.ModifierKeys != Keys.None) {
                return;
            }

            if(this.map.Metrics.GetCell(this.navigationWidget.MouseCell, out var cell)) {
                if(this.map.Technos[cell] is Unit unit) {
                    this.selectedUnit = null;

                    this.selectedObjectProperties?.Close();
                    this.selectedObjectProperties = new ObjectPropertiesPopup(this.objectProperties.Plugin, unit);
                    this.selectedObjectProperties.Closed += (cs, ce) => {
                        this.navigationWidget.Refresh();
                    };

                    unit.PropertyChanged += this.SelectedUnit_PropertyChanged;

                    this.selectedObjectProperties.Show(this.mapPanel, this.mapPanel.PointToClient(Control.MousePosition));

                    this.UpdateStatus();
                }
            }
        }

        private void MockUnit_PropertyChanged(object sender, PropertyChangedEventArgs e) => this.RefreshMapPanel();

        private void SelectedUnit_PropertyChanged(object sender, PropertyChangedEventArgs e) => this.mapPanel.Invalidate(this.map, sender as Unit);

        private void UnitTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) => this.SelectedUnitType = this.unitTypeComboBox.SelectedValue as UnitType;

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
                    this.AddUnit(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveUnit(this.navigationWidget.MouseCell);
                }
            } else if(e.Button == MouseButtons.Left) {
                this.SelectUnit(this.navigationWidget.MouseCell);
            } else if(e.Button == MouseButtons.Right) {
                this.PickUnit(this.navigationWidget.MouseCell);
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e) {
            if(this.selectedUnit != null) {
                this.selectedUnit = null;

                this.UpdateStatus();
            }
        }

        private void MouseoverWidget_MouseCellChanged(object sender, MouseCellChangedEventArgs e) {
            if(this.placementMode) {
                if(this.SelectedUnitType != null) {
                    this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(e.OldCell, new Size(1, 1)), 1, 1));
                    this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(e.NewCell, new Size(1, 1)), 1, 1));
                }
            } else if(this.selectedUnit != null) {
                var oldLocation = this.map.Technos[this.selectedUnit].Value;
                this.mapPanel.Invalidate(this.map, this.selectedUnit);
                this.map.Technos.Remove(this.selectedUnit);
                if(this.map.Technos.Add(e.NewCell, this.selectedUnit)) {
                    this.mapPanel.Invalidate(this.map, this.selectedUnit);
                    this.plugin.Dirty = true;
                } else {
                    this.map.Technos.Add(oldLocation, this.selectedUnit);
                }
            }
        }

        private void AddUnit(Point location) {
            if(this.SelectedUnitType != null) {
                var unit = this.mockUnit.Clone();
                if(this.map.Technos.Add(location, unit)) {
                    this.mapPanel.Invalidate(this.map, unit);
                    this.plugin.Dirty = true;
                }
            }
        }

        private void RemoveUnit(Point location) {
            if(this.map.Technos[location] is Unit unit) {
                this.mapPanel.Invalidate(this.map, unit);
                this.map.Technos.Remove(unit);
                this.plugin.Dirty = true;
            }
        }

        private void EnterPlacementMode() {
            if(this.placementMode) {
                return;
            }

            this.placementMode = true;

            this.navigationWidget.MouseoverSize = Size.Empty;

            if(this.SelectedUnitType != null) {
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

            if(this.SelectedUnitType != null) {
                this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(this.navigationWidget.MouseCell, new Size(1, 1)), 1, 1));
            }

            this.UpdateStatus();
        }

        private void PickUnit(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                if(this.map.Technos[cell] is Unit unit) {
                    this.SelectedUnitType = unit.Type;
                    this.mockUnit.House = unit.House;
                    this.mockUnit.Strength = unit.Strength;
                    this.mockUnit.Direction = unit.Direction;
                    this.mockUnit.Mission = unit.Mission;
                    this.mockUnit.Trigger = unit.Trigger;
                }
            }
        }

        private void SelectUnit(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                this.selectedUnit = this.map.Technos[cell] as Unit;
            }

            this.UpdateStatus();
        }

        private void RefreshMapPanel() {
            if(this.mockUnit.Type != null) {
                var unitPreview = new Bitmap(Globals.TileWidth * 3, Globals.TileHeight * 3);
                using(var g = Graphics.FromImage(unitPreview)) {
                    MapRenderer.Render(this.plugin.GameType, this.map.Theater, new Point(1, 1), Globals.TileSize, this.mockUnit).Item2(g);
                }
                this.unitTypeMapPanel.MapImage = unitPreview;
            } else {
                this.unitTypeMapPanel.MapImage = null;
            }
        }

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to place unit, Right-Click to remove unit";
            } else if(this.selectedUnit != null) {
                this.statusLbl.Text = "Drag mouse to move unit";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click drag to move unit, Double-Click update unit properties, Right-Click to pick unit";
            }
        }

        protected override void PreRenderMap() {
            base.PreRenderMap();

            this.previewMap = this.map.Clone();
            if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedUnitType != null) {
                    var unit = this.mockUnit.Clone();
                    unit.Tint = Color.FromArgb(128, Color.White);
                    if(this.previewMap.Technos.Add(location, unit)) {
                        this.mapPanel.Invalidate(this.previewMap, unit);
                    }
                }
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            var unitPen = new Pen(Color.Green, 4.0f);
            foreach(var (topLeft, _) in this.map.Technos.OfType<Unit>()) {
                var bounds = new Rectangle(new Point(topLeft.X * Globals.TileWidth, topLeft.Y * Globals.TileHeight), Globals.TileSize);
                graphics.DrawRectangle(unitPen, bounds);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected override void Dispose(bool disposing) {
            if(!this.disposedValue) {
                if(disposing) {
                    this.selectedObjectProperties?.Close();

                    this.mapPanel.MouseDown -= this.MapPanel_MouseDown;
                    this.mapPanel.MouseUp -= this.MapPanel_MouseUp;
                    this.mapPanel.MouseDoubleClick -= this.MapPanel_MouseDoubleClick;
                    this.mapPanel.MouseMove -= this.MapPanel_MouseMove;
                    (this.mapPanel as Control).KeyDown -= this.UnitTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.UnitTool_KeyUp;

                    this.unitTypeComboBox.SelectedIndexChanged -= this.UnitTypeComboBox_SelectedIndexChanged;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion IDisposable Support
    }
}
