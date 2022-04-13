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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MobiusEditor.Tools {
    public class TemplateTool : ViewTool {
        private static readonly Regex CategoryRegex = new Regex(@"^([a-z]*)", RegexOptions.Compiled);

        private readonly ListView templateTypeListView;
        private readonly MapPanel templateTypeMapPanel;
        private readonly ToolTip mouseTooltip;

        private readonly Dictionary<int, Template> undoTemplates = new Dictionary<int, Template>();
        private readonly Dictionary<int, Template> redoTemplates = new Dictionary<int, Template>();

        private Map previewMap;
        protected override Map RenderMap => this.previewMap;

        private bool placementMode;

        private bool boundsMode;
        private Rectangle dragBounds;
        private int dragEdge = -1;

        private TemplateType selectedTemplateType;
        private TemplateType SelectedTemplateType {
            get => this.selectedTemplateType;
            set {
                if(this.selectedTemplateType != value) {
                    if(this.placementMode && (this.selectedTemplateType != null)) {
                        for(var y = 0; y < this.selectedTemplateType.IconHeight; ++y) {
                            for(var x = 0; x < this.selectedTemplateType.IconWidth; ++x) {
                                this.mapPanel.Invalidate(this.map, new Point(this.navigationWidget.MouseCell.X + x, this.navigationWidget.MouseCell.Y + y));
                            }
                        }
                    }

                    this.selectedTemplateType = value;

                    this.templateTypeListView.BeginUpdate();
                    this.templateTypeListView.SelectedIndexChanged -= this.TemplateTypeListView_SelectedIndexChanged;
                    foreach(ListViewItem item in this.templateTypeListView.Items) {
                        item.Selected = item.Tag == this.selectedTemplateType;
                    }
                    if(this.templateTypeListView.SelectedIndices.Count > 0) {
                        this.templateTypeListView.EnsureVisible(this.templateTypeListView.SelectedIndices[0]);
                    }
                    this.templateTypeListView.SelectedIndexChanged += this.TemplateTypeListView_SelectedIndexChanged;
                    this.templateTypeListView.EndUpdate();

                    if(this.placementMode && (this.selectedTemplateType != null)) {
                        for(var y = 0; y < this.selectedTemplateType.IconHeight; ++y) {
                            for(var x = 0; x < this.selectedTemplateType.IconWidth; ++x) {
                                this.mapPanel.Invalidate(this.map, new Point(this.navigationWidget.MouseCell.X + x, this.navigationWidget.MouseCell.Y + y));
                            }
                        }
                    }

                    this.RefreshMapPanel();
                }
            }
        }

        private Point? selectedIcon;
        private Point? SelectedIcon {
            get => this.selectedIcon;
            set {
                if(this.selectedIcon != value) {
                    this.selectedIcon = value;
                    this.templateTypeMapPanel.Invalidate();

                    if(this.placementMode && (this.SelectedTemplateType != null)) {
                        for(var y = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                            for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x) {
                                this.mapPanel.Invalidate(this.map, new Point(this.navigationWidget.MouseCell.X + x, this.navigationWidget.MouseCell.Y + y));
                            }
                        }
                    }
                }
            }
        }

        private NavigationWidget templateTypeNavigationWidget;

        public TemplateTool(MapPanel mapPanel, MapLayerFlag layers, ToolStripStatusLabel statusLbl, ListView templateTypeListView, MapPanel templateTypeMapPanel, ToolTip mouseTooltip, IGamePlugin plugin, UndoRedoList<UndoRedoEventArgs> url)
            : base(mapPanel, layers, statusLbl, plugin, url) {
            this.previewMap = this.map;

            this.mapPanel.MouseDown += this.MapPanel_MouseDown;
            this.mapPanel.MouseUp += this.MapPanel_MouseUp;
            this.mapPanel.MouseMove += this.MapPanel_MouseMove;
            (this.mapPanel as Control).KeyDown += this.TemplateTool_KeyDown;
            (this.mapPanel as Control).KeyUp += this.TemplateTool_KeyUp;

            this.templateTypeListView = templateTypeListView;
            this.templateTypeListView.SelectedIndexChanged += this.TemplateTypeListView_SelectedIndexChanged;

            string templateCategory(TemplateType template) {
                var m = CategoryRegex.Match(template.Name);
                return m.Success ? m.Groups[1].Value : string.Empty;
            }

            var templateTypes = plugin.Map.TemplateTypes
                .Where(t =>
                    (t.Thumbnail != null) &&
                    t.Theaters.Contains(plugin.Map.Theater) &&
                    ((t.Flag & TemplateTypeFlag.Clear) == TemplateTypeFlag.None))
                .GroupBy(t => templateCategory(t)).OrderBy(g => g.Key);
            var templateTypeImages = templateTypes.SelectMany(g => g).Select(t => t.Thumbnail);

            var maxWidth = templateTypeImages.Max(t => t.Width);
            var maxHeight = templateTypeImages.Max(t => t.Height);

            var imageList = new ImageList();
            imageList.Images.AddRange(templateTypeImages.ToArray());
            imageList.ImageSize = new Size(maxWidth, maxHeight);
            imageList.ColorDepth = ColorDepth.Depth24Bit;

            this.templateTypeListView.BeginUpdate();
            this.templateTypeListView.LargeImageList = imageList;

            var imageIndex = 0;
            foreach(var templateTypeGroup in templateTypes) {
                var group = new ListViewGroup(templateTypeGroup.Key);
                this.templateTypeListView.Groups.Add(group);
                foreach(var templateType in templateTypeGroup) {
                    var item = new ListViewItem(templateType.DisplayName, imageIndex++) {
                        Group = group,
                        Tag = templateType
                    };
                    this.templateTypeListView.Items.Add(item);
                }
            }
            this.templateTypeListView.EndUpdate();

            this.templateTypeMapPanel = templateTypeMapPanel;
            this.templateTypeMapPanel.MouseDown += this.TemplateTypeMapPanel_MouseDown;
            this.templateTypeMapPanel.PostRender += this.TemplateTypeMapPanel_PostRender;
            this.templateTypeMapPanel.BackColor = Color.Black;
            this.templateTypeMapPanel.MaxZoom = 1;

            this.mouseTooltip = mouseTooltip;

            this.navigationWidget.MouseCellChanged += this.MouseoverWidget_MouseCellChanged;

            url.Undone += this.Url_Undone;
            url.Redone += this.Url_Redone;

            this.SelectedTemplateType = templateTypes.First().First();

            this.UpdateStatus();
        }

        private void Url_Redone(object sender, EventArgs e) {
            if(this.boundsMode && (this.map.Bounds != this.dragBounds)) {
                this.dragBounds = this.map.Bounds;
                this.dragEdge = -1;

                this.UpdateTooltip();
                this.mapPanel.Invalidate();
            }
        }

        private void Url_Undone(object sender, EventArgs e) {
            if(this.boundsMode && (this.map.Bounds != this.dragBounds)) {
                this.dragBounds = this.map.Bounds;
                this.dragEdge = -1;

                this.UpdateTooltip();
                this.mapPanel.Invalidate();
            }
        }

        private void TemplateTypeMapPanel_MouseDown(object sender, MouseEventArgs e) {
            if((this.SelectedTemplateType == null) || ((this.SelectedTemplateType.IconWidth * this.SelectedTemplateType.IconHeight) == 1)) {
                this.SelectedIcon = null;
            } else {
                if(e.Button == MouseButtons.Left) {
                    var templateTypeMouseCell = this.templateTypeNavigationWidget.MouseCell;
                    if((templateTypeMouseCell.X >= 0) && (templateTypeMouseCell.X < this.SelectedTemplateType.IconWidth)) {
                        if((templateTypeMouseCell.Y >= 0) && (templateTypeMouseCell.Y < this.SelectedTemplateType.IconHeight)) {
                            if(this.SelectedTemplateType.IconMask[templateTypeMouseCell.X, templateTypeMouseCell.Y]) {
                                this.SelectedIcon = templateTypeMouseCell;
                            }
                        }
                    }
                } else if(e.Button == MouseButtons.Right) {
                    this.SelectedIcon = null;
                }
            }
        }

        private void TemplateTypeMapPanel_PostRender(object sender, RenderEventArgs e) {
            if(this.SelectedIcon.HasValue) {
                var selectedIconPen = new Pen(Color.Yellow, 2);
                var cellSize = new Size(Globals.OriginalTileWidth / 4, Globals.OriginalTileHeight / 4);
                var rect = new Rectangle(new Point(this.SelectedIcon.Value.X * cellSize.Width, this.SelectedIcon.Value.Y * cellSize.Height), cellSize);
                e.Graphics.DrawRectangle(selectedIconPen, rect);
            }

            if(this.SelectedTemplateType != null) {
                var sizeStringFormat = new StringFormat {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                var sizeBackgroundBrush = new SolidBrush(Color.FromArgb(128, Color.Black));
                var sizeTextBrush = new SolidBrush(Color.White);

                var text = string.Format("{0} ({1}x{2})", this.SelectedTemplateType.DisplayName, this.SelectedTemplateType.IconWidth, this.SelectedTemplateType.IconHeight);
                var textSize = e.Graphics.MeasureString(text, SystemFonts.CaptionFont) + new SizeF(6.0f, 6.0f);
                var textBounds = new RectangleF(new PointF(0, 0), textSize);
                e.Graphics.Transform = new Matrix();
                e.Graphics.FillRectangle(sizeBackgroundBrush, textBounds);
                e.Graphics.DrawString(text, SystemFonts.CaptionFont, sizeTextBrush, textBounds, sizeStringFormat);
            }
        }

        private void TemplateTool_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.ShiftKey) {
                this.EnterPlacementMode();
            } else if(e.KeyCode == Keys.ControlKey) {
                this.EnterBoundsMode();
            }
        }

        private void TemplateTool_KeyUp(object sender, KeyEventArgs e) {
            if((e.KeyCode == Keys.ShiftKey) || (e.KeyCode == Keys.ControlKey)) {
                this.ExitAllModes();
            }
        }

        private void TemplateTypeListView_SelectedIndexChanged(object sender, EventArgs e) {
            this.SelectedTemplateType = (this.templateTypeListView.SelectedItems.Count > 0) ? (this.templateTypeListView.SelectedItems[0].Tag as TemplateType) : null;
            this.SelectedIcon = null;
        }

        private void MapPanel_MouseDown(object sender, MouseEventArgs e) {
            if(this.boundsMode) {
                this.dragEdge = this.DetectDragEdge();

                this.UpdateStatus();
            } else if(this.placementMode) {
                if(e.Button == MouseButtons.Left) {
                    this.SetTemplate(this.navigationWidget.MouseCell);
                } else if(e.Button == MouseButtons.Right) {
                    this.RemoveTemplate(this.navigationWidget.MouseCell);
                }
            } else if((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right)) {
                this.PickTemplate(this.navigationWidget.MouseCell, e.Button == MouseButtons.Left);
            }
        }

        private void MapPanel_MouseUp(object sender, MouseEventArgs e) {
            if(this.boundsMode) {
                if(this.dragBounds != this.map.Bounds) {
                    var oldBounds = this.map.Bounds;
                    void undoAction(UndoRedoEventArgs ure) {
                        ure.Map.Bounds = oldBounds;
                        ure.MapPanel.Invalidate();
                    }

                    void redoAction(UndoRedoEventArgs ure) {
                        ure.Map.Bounds = this.dragBounds;
                        ure.MapPanel.Invalidate();
                    }

                    this.map.Bounds = this.dragBounds;

                    this.url.Track(undoAction, redoAction);
                    this.mapPanel.Invalidate();
                }

                this.dragEdge = -1;

                this.UpdateStatus();
            } else {
                if((this.undoTemplates.Count > 0) || (this.redoTemplates.Count > 0)) {
                    this.CommitChange();
                }
            }
        }

        private void MapPanel_MouseMove(object sender, MouseEventArgs e) {
            if(!this.placementMode && (Control.ModifierKeys == Keys.Shift)) {
                this.EnterPlacementMode();
            } else if(!this.boundsMode && (Control.ModifierKeys == Keys.Control)) {
                this.EnterBoundsMode();
            } else if((this.placementMode || this.boundsMode) && (Control.ModifierKeys == Keys.None)) {
                this.ExitAllModes();
            }

            var cursor = Cursors.Default;
            if(this.boundsMode) {
                switch((this.dragEdge >= 0) ? this.dragEdge : this.DetectDragEdge()) {
                case 0:
                case 4:
                    cursor = Cursors.SizeNS;
                    break;
                case 2:
                case 6:
                    cursor = Cursors.SizeWE;
                    break;
                case 1:
                case 5:
                    cursor = Cursors.SizeNESW;
                    break;
                case 3:
                case 7:
                    cursor = Cursors.SizeNWSE;
                    break;
                }
            }
            Cursor.Current = cursor;

            this.UpdateTooltip();
        }

        private void MouseoverWidget_MouseCellChanged(object sender, MouseCellChangedEventArgs e) {
            if(this.dragEdge >= 0) {
                var endDrag = this.navigationWidget.MouseCell;
                this.map.Metrics.Clip(ref endDrag, new Size(1, 1), Size.Empty);

                switch(this.dragEdge) {
                case 0:
                case 1:
                case 7:
                    if(endDrag.Y < this.dragBounds.Bottom) {
                        this.dragBounds.Height = this.dragBounds.Bottom - endDrag.Y;
                        this.dragBounds.Y = endDrag.Y;
                    }
                    break;
                }

                switch(this.dragEdge) {
                case 5:
                case 6:
                case 7:
                    if(endDrag.X < this.dragBounds.Right) {
                        this.dragBounds.Width = this.dragBounds.Right - endDrag.X;
                        this.dragBounds.X = endDrag.X;
                    }
                    break;
                }

                switch(this.dragEdge) {
                case 3:
                case 4:
                case 5:
                    if(endDrag.Y > this.dragBounds.Top) {
                        this.dragBounds.Height = endDrag.Y - this.dragBounds.Top;
                    }
                    break;
                }

                switch(this.dragEdge) {
                case 1:
                case 2:
                case 3:
                    if(endDrag.X > this.dragBounds.Left) {
                        this.dragBounds.Width = endDrag.X - this.dragBounds.Left;
                    }
                    break;
                }

                this.mapPanel.Invalidate();
            } else if(this.placementMode) {
                if(Control.MouseButtons == MouseButtons.Right) {
                    this.RemoveTemplate(this.navigationWidget.MouseCell);
                }

                if(this.SelectedTemplateType != null) {
                    foreach(var location in new Point[] { e.OldCell, e.NewCell }) {
                        for(var y = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                            for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x) {
                                this.mapPanel.Invalidate(this.map, new Point(location.X + x, location.Y + y));
                            }
                        }
                    }
                }
            } else if((Control.MouseButtons == MouseButtons.Left) || (Control.MouseButtons == MouseButtons.Right)) {
                this.PickTemplate(this.navigationWidget.MouseCell, Control.MouseButtons == MouseButtons.Left);
            }
        }

        private void RefreshMapPanel() {
            if(this.templateTypeNavigationWidget != null) {
                this.templateTypeNavigationWidget.Dispose();
                this.templateTypeNavigationWidget = null;
            }

            if(this.SelectedTemplateType != null) {
                this.templateTypeMapPanel.MapImage = this.SelectedTemplateType.Thumbnail;

                var templateTypeMetrics = new CellMetrics(this.SelectedTemplateType.IconWidth, this.SelectedTemplateType.IconHeight);
                this.templateTypeNavigationWidget = new NavigationWidget(this.templateTypeMapPanel, templateTypeMetrics,
                    new Size(Globals.OriginalTileWidth / 4, Globals.OriginalTileHeight / 4)) {
                    MouseoverSize = Size.Empty
                };
            } else {
                this.templateTypeMapPanel.MapImage = null;
            }
        }

        private void SetTemplate(Point location) {
            if(this.SelectedTemplateType != null) {
                if(this.SelectedIcon.HasValue) {
                    if(this.map.Metrics.GetCell(location, out var cell)) {
                        if(!this.undoTemplates.ContainsKey(cell)) {
                            this.undoTemplates[cell] = this.map.Templates[location];
                        }

                        var icon = (this.SelectedIcon.Value.Y * this.SelectedTemplateType.IconWidth) + this.SelectedIcon.Value.X;
                        var template = new Template { Type = SelectedTemplateType, Icon = icon };
                        this.map.Templates[cell] = template;
                        this.redoTemplates[cell] = template;
                        this.mapPanel.Invalidate(this.map, cell);
                        this.plugin.Dirty = true;
                    }
                } else {
                    for(int y = 0, icon = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                        for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x, ++icon) {
                            var subLocation = new Point(location.X + x, location.Y + y);
                            if(this.map.Metrics.GetCell(subLocation, out var cell)) {
                                if(!this.undoTemplates.ContainsKey(cell)) {
                                    this.undoTemplates[cell] = this.map.Templates[subLocation];
                                }
                            }
                        }
                    }

                    for(int y = 0, icon = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                        for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x, ++icon) {
                            if(!this.SelectedTemplateType.IconMask[x, y]) {
                                continue;
                            }

                            var subLocation = new Point(location.X + x, location.Y + y);
                            if(this.map.Metrics.GetCell(subLocation, out var cell)) {
                                var template = new Template { Type = SelectedTemplateType, Icon = icon };
                                this.map.Templates[cell] = template;
                                this.redoTemplates[cell] = template;
                                this.mapPanel.Invalidate(this.map, cell);
                                this.plugin.Dirty = true;
                            }
                        }
                    }
                }
            }
        }

        private void RemoveTemplate(Point location) {
            if(this.SelectedTemplateType != null) {
                if(this.SelectedIcon.HasValue) {
                    if(this.map.Metrics.GetCell(location, out var cell)) {
                        if(!this.undoTemplates.ContainsKey(cell)) {
                            this.undoTemplates[cell] = this.map.Templates[location];
                        }

                        this.map.Templates[cell] = null;
                        this.redoTemplates[cell] = null;
                        this.mapPanel.Invalidate(this.map, cell);
                        this.plugin.Dirty = true;
                    }
                } else {
                    for(int y = 0, icon = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                        for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x, ++icon) {
                            var subLocation = new Point(location.X + x, location.Y + y);
                            if(this.map.Metrics.GetCell(subLocation, out var cell)) {
                                if(!this.undoTemplates.ContainsKey(cell)) {
                                    this.undoTemplates[cell] = this.map.Templates[subLocation];
                                }
                            }
                        }
                    }

                    for(int y = 0, icon = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                        for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x, ++icon) {
                            var subLocation = new Point(location.X + x, location.Y + y);
                            if(this.map.Metrics.GetCell(subLocation, out var cell)) {
                                this.map.Templates[cell] = null;
                                this.redoTemplates[cell] = null;
                                this.mapPanel.Invalidate(this.map, cell);
                                this.plugin.Dirty = true;
                            }
                        }
                    }
                }
            }
        }

        private void EnterPlacementMode() {
            if(this.placementMode || this.boundsMode) {
                return;
            }

            this.placementMode = true;

            this.navigationWidget.MouseoverSize = Size.Empty;

            if(this.SelectedTemplateType != null) {
                for(var y = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                    for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x) {
                        this.mapPanel.Invalidate(this.map, new Point(this.navigationWidget.MouseCell.X + x, this.navigationWidget.MouseCell.Y + y));
                    }
                }
            }

            this.UpdateStatus();
        }

        private void EnterBoundsMode() {
            if(this.boundsMode || this.placementMode) {
                return;
            }

            this.boundsMode = true;
            this.dragBounds = this.map.Bounds;

            this.navigationWidget.MouseoverSize = Size.Empty;

            if(this.SelectedTemplateType != null) {
                for(var y = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                    for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x) {
                        this.mapPanel.Invalidate(this.map, new Point(this.navigationWidget.MouseCell.X + x, this.navigationWidget.MouseCell.Y + y));
                    }
                }
            }

            this.UpdateTooltip();
            this.UpdateStatus();
        }

        private void ExitAllModes() {
            if(!this.placementMode && !this.boundsMode) {
                return;
            }

            this.boundsMode = false;
            this.dragEdge = -1;
            this.dragBounds = Rectangle.Empty;
            this.placementMode = false;

            this.navigationWidget.MouseoverSize = new Size(1, 1);

            if(this.SelectedTemplateType != null) {
                for(var y = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                    for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x) {
                        this.mapPanel.Invalidate(this.map, new Point(this.navigationWidget.MouseCell.X + x, this.navigationWidget.MouseCell.Y + y));
                    }
                }
            }

            this.UpdateTooltip();
            this.UpdateStatus();
        }

        private void UpdateTooltip() {
            if(this.boundsMode) {
                var tooltip = string.Format("X = {0}\nY = {1}\nWidth = {2}\nHeight = {3}", this.dragBounds.Left, this.dragBounds.Top, this.dragBounds.Width, this.dragBounds.Height);
                var textSize = TextRenderer.MeasureText(tooltip, SystemFonts.CaptionFont);
                var tooltipSize = new Size(textSize.Width + 6, textSize.Height + 6);

                var tooltipPosition = this.mapPanel.PointToClient(Control.MousePosition);
                switch(this.dragEdge) {
                case -1:
                case 0:
                case 1:
                case 7:
                    tooltipPosition.Y -= tooltipSize.Height;
                    break;
                }
                switch(this.dragEdge) {
                case -1:
                case 5:
                case 6:
                case 7:
                    tooltipPosition.X -= tooltipSize.Width;
                    break;
                }

                var screenPosition = this.mapPanel.PointToScreen(tooltipPosition);
                var screen = Screen.FromControl(this.mapPanel);
                screenPosition.X = Math.Max(0, Math.Min(screen.WorkingArea.Width - tooltipSize.Width, screenPosition.X));
                screenPosition.Y = Math.Max(0, Math.Min(screen.WorkingArea.Height - tooltipSize.Height, screenPosition.Y));
                tooltipPosition = this.mapPanel.PointToClient(screenPosition);

                this.mouseTooltip.Show(tooltip, this.mapPanel, tooltipPosition.X, tooltipPosition.Y);
            } else {
                this.mouseTooltip.Hide(this.mapPanel);
            }
        }

        private void PickTemplate(Point location, bool wholeTemplate) {
            if(this.map.Metrics.GetCell(location, out var cell)) {
                var template = this.map.Templates[cell];
                if(template != null) {
                    this.SelectedTemplateType = template.Type;
                } else {
                    this.SelectedTemplateType = this.map.TemplateTypes.Where(t => t.Equals("clear1")).FirstOrDefault();
                }

                if(!wholeTemplate && ((this.SelectedTemplateType.IconWidth * this.SelectedTemplateType.IconHeight) > 1)) {
                    var icon = template?.Icon ?? 0;
                    this.SelectedIcon = new Point(icon % this.SelectedTemplateType.IconWidth, icon / this.SelectedTemplateType.IconWidth);
                } else {
                    this.SelectedIcon = null;
                }
            }
        }

        private int DetectDragEdge() {
            var mouseCell = this.navigationWidget.MouseCell;
            var mousePixel = this.navigationWidget.MouseSubPixel;
            var topEdge =
                ((mouseCell.Y == this.dragBounds.Top) && (mousePixel.Y <= (Globals.PixelHeight / 4))) ||
                ((mouseCell.Y == this.dragBounds.Top - 1) && (mousePixel.Y >= (3 * Globals.PixelHeight / 4)));
            var bottomEdge =
                ((mouseCell.Y == this.dragBounds.Bottom) && (mousePixel.Y <= (Globals.PixelHeight / 4))) ||
                ((mouseCell.Y == this.dragBounds.Bottom - 1) && (mousePixel.Y >= (3 * Globals.PixelHeight / 4)));
            var leftEdge =
                 ((mouseCell.X == this.dragBounds.Left) && (mousePixel.X <= (Globals.PixelWidth / 4))) ||
                 ((mouseCell.X == this.dragBounds.Left - 1) && (mousePixel.X >= (3 * Globals.PixelWidth / 4)));
            var rightEdge =
                ((mouseCell.X == this.dragBounds.Right) && (mousePixel.X <= (Globals.PixelHeight / 4))) ||
                ((mouseCell.X == this.dragBounds.Right - 1) && (mousePixel.X >= (3 * Globals.PixelHeight / 4)));
            if(topEdge) {
                if(rightEdge) {
                    return 1;
                } else if(leftEdge) {
                    return 7;
                } else {
                    return 0;
                }
            } else if(bottomEdge) {
                if(rightEdge) {
                    return 3;
                } else if(leftEdge) {
                    return 5;
                } else {
                    return 4;
                }
            } else if(rightEdge) {
                return 2;
            } else if(leftEdge) {
                return 6;
            } else {
                return -1;
            }
        }

        private void CommitChange() {
            var undoTemplates2 = new Dictionary<int, Template>(this.undoTemplates);
            void undoAction(UndoRedoEventArgs e) {
                foreach(var kv in undoTemplates2) {
                    e.Map.Templates[kv.Key] = kv.Value;
                }
                e.MapPanel.Invalidate(e.Map, undoTemplates2.Keys);
            }

            var redoTemplates2 = new Dictionary<int, Template>(this.redoTemplates);
            void redoAction(UndoRedoEventArgs e) {
                foreach(var kv in redoTemplates2) {
                    e.Map.Templates[kv.Key] = kv.Value;
                }
                e.MapPanel.Invalidate(e.Map, redoTemplates2.Keys);
            }

            this.undoTemplates.Clear();
            this.redoTemplates.Clear();

            this.url.Track(undoAction, redoAction);
        }

        private void UpdateStatus() {
            if(this.placementMode) {
                this.statusLbl.Text = "Left-Click to place template, Right-Click to clear template";
            } else if(this.boundsMode) {
                if(this.dragEdge >= 0) {
                    this.statusLbl.Text = "Release left button to end dragging map bounds edge";
                } else {
                    this.statusLbl.Text = "Left-Click a map bounds edge to start dragging";
                }
            } else {
                this.statusLbl.Text = "Shift to enter placement mode, Ctrl to enter map bounds mode, Left-Click to pick whole template, Right-Click to pick individual template tile";
            }
        }

        protected override void PreRenderMap() {
            base.PreRenderMap();

            this.previewMap = this.map.Clone();
            if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedTemplateType != null) {
                    if(this.SelectedIcon.HasValue) {
                        if(this.previewMap.Metrics.GetCell(location, out var cell)) {
                            var icon = (this.SelectedIcon.Value.Y * this.SelectedTemplateType.IconWidth) + this.SelectedIcon.Value.X;
                            this.previewMap.Templates[cell] = new Template { Type = SelectedTemplateType, Icon = icon };
                        }
                    } else {
                        var icon = 0;
                        for(var y = 0; y < this.SelectedTemplateType.IconHeight; ++y) {
                            for(var x = 0; x < this.SelectedTemplateType.IconWidth; ++x, ++icon) {
                                if(!this.SelectedTemplateType.IconMask[x, y]) {
                                    continue;
                                }

                                var subLocation = new Point(location.X + x, location.Y + y);
                                if(this.previewMap.Metrics.GetCell(subLocation, out var cell)) {
                                    this.previewMap.Templates[cell] = new Template { Type = SelectedTemplateType, Icon = icon };
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void PostRenderMap(Graphics graphics) {
            base.PostRenderMap(graphics);

            if(this.boundsMode) {
                var bounds = Rectangle.FromLTRB(
                    this.dragBounds.Left * Globals.TileWidth,
                    this.dragBounds.Top * Globals.TileHeight,
                    this.dragBounds.Right * Globals.TileWidth,
                    this.dragBounds.Bottom * Globals.TileHeight
                );

                var boundsPen = new Pen(Color.Red, 8.0f);
                graphics.DrawRectangle(boundsPen, bounds);
            } else if(this.placementMode) {
                var location = this.navigationWidget.MouseCell;
                if(this.SelectedTemplateType != null) {
                    var previewPen = new Pen(Color.Green, 4.0f);
                    var previewBounds = new Rectangle(
                        location.X * Globals.TileWidth,
                        location.Y * Globals.TileHeight,
                        (this.SelectedIcon.HasValue ? 1 : this.SelectedTemplateType.IconWidth) * Globals.TileWidth,
                        (this.SelectedIcon.HasValue ? 1 : this.SelectedTemplateType.IconHeight) * Globals.TileHeight
                    );
                    graphics.DrawRectangle(previewPen, previewBounds);
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
                    (this.mapPanel as Control).KeyDown -= this.TemplateTool_KeyDown;
                    (this.mapPanel as Control).KeyUp -= this.TemplateTool_KeyUp;

                    this.templateTypeListView.SelectedIndexChanged -= this.TemplateTypeListView_SelectedIndexChanged;

                    this.templateTypeMapPanel.MouseDown -= this.TemplateTypeMapPanel_MouseDown;
                    this.templateTypeMapPanel.PostRender -= this.TemplateTypeMapPanel_PostRender;

                    this.navigationWidget.MouseCellChanged -= this.MouseoverWidget_MouseCellChanged;

                    this.url.Undone -= this.Url_Undone;
                    this.url.Redone -= this.Url_Redone;
                }
                this.disposedValue = true;
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
