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
    public class InfantryTool : ViewTool {
        private readonly TypeComboBox infantryTypeComboBox;
        private readonly MapPanel infantryTypeMapPanel;
        private readonly ObjectProperties objectProperties;

        private Map previewMap;
        protected override Map RenderMap => this.previewMap;

        private bool placementMode;

        private readonly Infantry mockInfantry;

        private Infantry selectedInfantry;
        private ObjectPropertiesPopup selectedObjectProperties;

        private InfantryType selectedInfantryType;
        private InfantryType SelectedInfantryType {
            get => this.selectedInfantryType;
            set {
                if(this.selectedInfantryType != value) {
                    if(this.placementMode && (this.selectedInfantryType != null)) {
                        this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
                    }

                    this.selectedInfantryType = value;
                    this.infantryTypeComboBox.SelectedValue = this.selectedInfantryType;

                    if(this.placementMode && (this.selectedInfantryType != null)) {
                        this.mapPanel.Invalidate(this.map, this.navigationWidget.MouseCell);
                    }

                    this.mockInfantry.Type = this.selectedInfantryType;

                    this.RefreshMapPanel();
                }
            }
        }

        public InfantryTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, TypeComboBox infantryTypeComboBox, MapPanel infantryTypeMapPanel, ObjectProperties objectProperties, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.previewMap = this.map;

            this.mockInfantry = new Infantry(null) {
                Type = infantryTypeComboBox.Types.First() as InfantryType,
                House = this.map.Houses.First().Type,
                Strength = 256,
                Direction = this.map.DirectionTypes.Where(d => d.Equals(FacingType.South)).First(),
                Mission = this.map.MissionTypes.Where(m => m.Equals("Guard")).FirstOrDefault() ?? this.map.MissionTypes.First()
            };
            this.mockInfantry.PropertyChanged += this.MockInfantry_PropertyChanged;

            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseUp += this.MapPanel_MouseUp;
            this.mapPanel.MouseDoubleClick += this.MapPanel_MouseDoubleClick;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.InfantryTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.InfantryTool_KeyUp;

            this.infantryTypeComboBox = infantryTypeComboBox;
            this.infantryTypeComboBox.SelectedIndexChanged += this.InfantryTypeComboBox_SelectedIndexChanged;

            this.infantryTypeMapPanel = infantryTypeMapPanel;
            this.infantryTypeMapPanel.BackColor = Color.White;
            this.infantryTypeMapPanel.MaxZoom = 1;

            this.objectProperties = objectProperties;
            this.objectProperties.Object = this.mockInfantry;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            this.SelectedInfantryType = this.infantryTypeComboBox.Types.First() as InfantryType;

            this.UpdateStatus();
        }

        private void MapPanel_MouseDoubleClick(object sender, MouseEventArgs e) {
            if(Control.ModifierKeys != Keys.None) {
                return;
            }

            if(this.map.Metrics.GetCell(this.navigationWidget.MouseCell, out var cell)) {
                if(this.map.Technos[cell] is InfantryGroup infantryGroup) {
                    var i = InfantryGroup.ClosestStoppingTypes(this.navigationWidget.MouseSubPixel).Cast<int>().First();
                    if(infantryGroup.Infantry[i] is Infantry infantry) {
                        this.selectedInfantry = null;

                        this.selectedObjectProperties?.Close();
                        this.selectedObjectProperties = new ObjectPropertiesPopup(this.objectProperties.Plugin, infantry);
                        this.selectedObjectProperties.Closed += (cs, ce) => {
                            this.navigationWidget.Refresh();
                        };

                        infantry.PropertyChanged += this.SelectedInfantry_PropertyChanged;

                        this.selectedObjectProperties.Show(this.mapPanel, this.mapPanel.PointToClient(Control.MousePosition));

                        this.UpdateStatus();
                    }
                }
            }
        }

        private void MockInfantry_PropertyChanged(object sender, PropertyChangedEventArgs e) => this.RefreshMapPanel();

        private void SelectedInfantry_PropertyChanged(object sender, PropertyChangedEventArgs e) => this.mapPanel.Invalidate(this.map, (sender as Infantry).InfantryGroup);

        private void InfantryTypeComboBox_SelectedIndexChanged(object sender, EventArgs e) => this.SelectedInfantryType = this.infantryTypeComboBox.SelectedValue as InfantryType;

        private void InfantryTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.EnterPlacementMode();
            }
        }

        private void InfantryTool_KeyUp(object sender, KeyEventArgs e) {
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

            if(this.placementMode) {
                this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(this.navigationWidget.MouseCell, new Size(1, 1)), 1, 1));
            } else if(this.selectedInfantry != null) {
                var oldLocation = this.map.Technos[this.selectedInfantry.InfantryGroup].Value;
                var oldStop = Array.IndexOf(this.selectedInfantry.InfantryGroup.Infantry, this.selectedInfantry);

                InfantryGroup infantryGroup = null;
                var techno = this.map.Technos[this.navigationWidget.MouseCell];
                if(techno == null) {
                    infantryGroup = new InfantryGroup();
                    this.map.Technos.Add(this.navigationWidget.MouseCell, infantryGroup);
                } else if(techno is InfantryGroup) {
                    infantryGroup = techno as InfantryGroup;
                }

                if(infantryGroup != null) {
                    foreach(var i in InfantryGroup.ClosestStoppingTypes(this.navigationWidget.MouseSubPixel).Cast<int>()) {
                        if(infantryGroup.Infantry[i] == null) {
                            this.selectedInfantry.InfantryGroup.Infantry[oldStop] = null;
                            infantryGroup.Infantry[i] = this.selectedInfantry;

                            if(infantryGroup != this.selectedInfantry.InfantryGroup) {
                                this.mapPanel.Invalidate(this.map, this.selectedInfantry.InfantryGroup);
                                if(this.selectedInfantry.InfantryGroup.Infantry.All(x => x == null)) {
                                    this.map.Technos.Remove(this.selectedInfantry.InfantryGroup);
                                }
                            }
                            this.selectedInfantry.InfantryGroup = infantryGroup;

                            this.mapPanel.Invalidate(this.map, infantryGroup);

                            this.plugin.Dirty = true;
                        }

                        if(infantryGroup == this.selectedInfantry.InfantryGroup) {
                            break;
                        }
                    }
                }
            }
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.AddInfantry(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveInfantry(this.navigationWidget.MouseCell);
                }
            } else if(e.Button == MouseButtons.Left) {
                this.SelectInfantry(this.navigationWidget.MouseCell);
            } else if(e.Button == MouseButtons.Right) {
                this.PickInfantry(this.navigationWidget.MouseCell);
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e) {
            if(this.selectedInfantry != null) {
                this.selectedInfantry = null;
                this.UpdateStatus();
            }
        }

        private void MouseoverWidget_MouseCellChanged(object sender, MouseCellChangedEventArgs e) {
            if(this.placementMode) {
                if(this.SelectedInfantryType != null) {
                    this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(e.OldCell, new Size(1, 1)), 1, 1));
                    this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(e.NewCell, new Size(1, 1)), 1, 1));
                }
            }
        }

        private void AddInfantry(Point location) {
            if(this.SelectedInfantryType != null) {
                if(this.map.Metrics.GetCell(location, out var cell)) {
                    InfantryGroup infantryGroup = null;

                    var techno = this.map.Technos[cell];
                    if(techno == null) {
                        infantryGroup = new InfantryGroup();
                        this.map.Technos.Add(cell, infantryGroup);
                    } else if(techno is InfantryGroup) {
                        infantryGroup = techno as InfantryGroup;
                    }

                    if(infantryGroup != null) {
                        foreach(var i in InfantryGroup.ClosestStoppingTypes(this.navigationWidget.MouseSubPixel).Cast<int>()) {
                            if(infantryGroup.Infantry[i] == null) {
                                var infantry = this.mockInfantry.Clone();
                                infantryGroup.Infantry[i] = infantry;
                                infantry.InfantryGroup = infantryGroup;
                                this.mapPanel.Invalidate(this.map, infantryGroup);
                                this.plugin.Dirty = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void RemoveInfantry(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                if(this.map.Technos[cell] is InfantryGroup infantryGroup) {
                    foreach(var i in InfantryGroup.ClosestStoppingTypes(this.navigationWidget.MouseSubPixel).Cast<int>()) {
                        if(infantryGroup.Infantry[i] != null) {
                            infantryGroup.Infantry[i] = null;
                            this.mapPanel.Invalidate(this.map, infantryGroup);
                            this.plugin.Dirty = true;
                            break;
                        }
                    }
                    if(infantryGroup.Infantry.All(i => i == null)) {
                        this.map.Technos.Remove(infantryGroup);
                    }
                }
            }
        }

        private void EnterPlacementMode() {
            if(this.placementMode) {
                return;
            }

            this.placementMode = true;

            this.navigationWidget.MouseoverSize = Size.Empty;

            if(this.SelectedInfantryType != null) {
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

            if(this.SelectedInfantryType != null) {
                this.mapPanel.Invalidate(this.map, Rectangle.Inflate(new Rectangle(this.navigationWidget.MouseCell, new Size(1, 1)), 1, 1));
            }

            this.UpdateStatus();
        }

        private void PickInfantry(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                if(this.map.Technos[cell] is InfantryGroup infantryGroup) {
                    var i = InfantryGroup.ClosestStoppingTypes(this.navigationWidget.MouseSubPixel).Cast<int>().First();
                    if(infantryGroup.Infantry[i] is Infantry infantry) {
                        this.SelectedInfantryType = infantry.Type;
                        this.mockInfantry.House = infantry.House;
                        this.mockInfantry.Strength = infantry.Strength;
                        this.mockInfantry.Direction = infantry.Direction;
                        this.mockInfantry.Mission = infantry.Mission;
                        this.mockInfantry.Trigger = infantry.Trigger;
                    }
                }
            }
        }

        private void SelectInfantry(Point location) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                this.selectedInfantry = null;
                if(this.map.Technos[cell] is InfantryGroup infantryGroup) {
                    var i = InfantryGroup.ClosestStoppingTypes(this.navigationWidget.MouseSubPixel).Cast<int>().First();
                    if(infantryGroup.Infantry[i] is Infantry infantry) {
                        this.selectedInfantry = infantry;
                    }
                }
            }

            this.UpdateStatus();
        }

        private void RefreshMapPanel() {
            if(this.mockInfantry.Type != null) {
                var infantryPreview = new Bitmap(Globals.TileWidth, Globals.TileHeight);
                using(var g = Graphics.FromImage(infantryPreview)) {
                    MapRenderer.Render(this.map.Theater, Point.Empty, Globals.TileSize, this.mockInfantry, InfantryStoppingType.Center).Item2(g);
                }
                this.infantryTypeMapPanel.MapImage = infantryPreview;
            } else {
                this.infantryTypeMapPanel.MapImage = null;
            }
        }

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to place infantry, Right-Click to remove infantry";
            } else if(this.selectedInfantry != null) {
                this.statusLbl.Text = "Drag mouse to move infantry";
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Left-Click drag to move infantry, Double-Click update infantry properties, Right-Click to pick infantry";
            }
        }

        protected override void PreRenderMap() {
            base.PreRenderMap();

            this.previewMap = this.map.Clone();
            if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedInfantryType != null) {
                    if(this.previewMap.Metrics.GetCell(location, out var cell)) {
                        InfantryGroup infantryGroup = null;

                        var techno = this.previewMap.Technos[cell];
                        if(techno == null) {
                            infantryGroup = new InfantryGroup();
                            this.previewMap.Technos.Add(cell, infantryGroup);
                        } else if(techno is InfantryGroup) {
                            infantryGroup = techno as InfantryGroup;
                        }

                        if(infantryGroup != null) {
                            foreach(var i in InfantryGroup.ClosestStoppingTypes(this.navigationWidget.MouseSubPixel).Cast<int>()) {
                                if(infantryGroup.Infantry[i] == null) {
                                    var infantry = this.mockInfantry.Clone();
                                    infantry.Tint = Color.FromArgb(128, Color.White);
                                    infantryGroup.Infantry[i] = infantry;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            var infantryPen = new Pen(Color.Green, 4.0f);
            foreach(var (topLeft, _) in this.map.Technos.OfType<InfantryGroup>()) {
                var bounds = new Rectangle(new Point(topLeft.X * Globals.TileWidth, topLeft.Y * Globals.TileHeight), Globals.TileSize);
                graphics.DrawRectangle(infantryPen, bounds);
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
                    (this.mapPanel as Control).KeyDown -= this.InfantryTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.InfantryTool_KeyUp;

                    this.infantryTypeComboBox.SelectedIndexChanged -= this.InfantryTypeComboBox_SelectedIndexChanged;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion IDisposable Support
    }
}
