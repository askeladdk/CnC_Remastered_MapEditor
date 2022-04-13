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
using MobiusEditor.Event;
using MobiusEditor.Interface;
using MobiusEditor.Model;
using MobiusEditor.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class MapPanel : Panel {
        private bool updatingCamera;
        private Rectangle cameraBounds;
        private Point lastScrollPosition;

        private (Point map, SizeF client)? referencePositions;

        private readonly Matrix mapToViewTransform = new Matrix();
        private readonly Matrix viewToPageTransform = new Matrix();

        private readonly Matrix compositeTransform = new Matrix();
        private readonly Matrix invCompositeTransform = new Matrix();

        private readonly HashSet<Point> invalidateCells = new HashSet<Point>();
        private bool fullInvalidation;

        private Image mapImage;
        public Image MapImage {
            get => this.mapImage;
            set {
                if(this.mapImage != value) {
                    this.mapImage = value;
                    this.UpdateCamera();
                }
            }
        }

        private int minZoom = 1;
        public int MinZoom {
            get => this.minZoom;
            set {
                if(this.minZoom != value) {
                    this.minZoom = value;
                    this.Zoom = this.zoom;
                }
            }
        }

        private int maxZoom = 8;
        public int MaxZoom {
            get => this.maxZoom;
            set {
                if(this.maxZoom != value) {
                    this.maxZoom = value;
                    this.Zoom = this.zoom;
                }
            }
        }

        private int zoomStep = 1;
        public int ZoomStep {
            get => this.zoomStep;
            set {
                if(this.zoomStep != value) {
                    this.zoomStep = value;
                    this.Zoom = (this.Zoom / this.zoomStep) * this.zoomStep;
                }
            }
        }

        private int zoom = 1;
        public int Zoom {
            get => this.zoom;
            set {
                var newZoom = Math.Max(this.MinZoom, Math.Min(this.MaxZoom, value));
                if(this.zoom != newZoom) {
                    this.zoom = newZoom;

                    var clientPosition = this.PointToClient(MousePosition);
                    this.referencePositions = (this.ClientToMap(clientPosition), new SizeF(clientPosition.X / (float)this.ClientSize.Width, clientPosition.Y / (float)this.ClientSize.Height));

                    this.UpdateCamera();
                }
            }
        }

        private int quality = Properties.Settings.Default.Quality;
        public int Quality {
            get => this.quality;
            set {
                if(this.quality != value) {
                    this.quality = value;
                    this.Invalidate();
                }
            }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool FocusOnMouseEnter {
            get; set;
        }

        public event EventHandler<RenderEventArgs> PreRender;
        public event EventHandler<RenderEventArgs> PostRender;

        public MapPanel() {
            this.InitializeComponent();
            this.DoubleBuffered = true;
        }

        public Point MapToClient(Point point) {
            var points = new Point[] { point };
            this.compositeTransform.TransformPoints(points);
            return points[0];
        }

        public Size MapToClient(Size size) {
            var points = new Point[] { (Point)size };
            this.compositeTransform.VectorTransformPoints(points);
            return (Size)points[0];
        }

        public Rectangle MapToClient(Rectangle rectangle) {
            var points = new Point[] { rectangle.Location, new Point(rectangle.Right, rectangle.Bottom) };
            this.compositeTransform.TransformPoints(points);
            return new Rectangle(points[0], new Size(points[1].X - points[0].X, points[1].Y - points[0].Y));
        }

        public Point ClientToMap(Point point) {
            var points = new Point[] { point };
            this.invCompositeTransform.TransformPoints(points);
            return points[0];
        }

        public Size ClientToMap(Size size) {
            var points = new Point[] { (Point)size };
            this.invCompositeTransform.VectorTransformPoints(points);
            return (Size)points[0];
        }

        public Rectangle ClientToMap(Rectangle rectangle) {
            var points = new Point[] { rectangle.Location, new Point(rectangle.Right, rectangle.Bottom) };
            this.invCompositeTransform.TransformPoints(points);
            return new Rectangle(points[0], new Size(points[1].X - points[0].X, points[1].Y - points[0].Y));
        }

        public void Invalidate(Map invalidateMap) {
            if(!this.fullInvalidation) {
                this.invalidateCells.Clear();
                this.fullInvalidation = true;
                this.Invalidate();
            }
        }

        public void Invalidate(Map invalidateMap, Rectangle cellBounds) {
            if(this.fullInvalidation) {
                return;
            }

            var count = this.invalidateCells.Count;
            this.invalidateCells.UnionWith(cellBounds.Points());
            if(this.invalidateCells.Count > count) {
                var overlapCells = invalidateMap.Overlappers.Overlaps(this.invalidateCells).ToHashSet();
                this.invalidateCells.UnionWith(overlapCells);
                this.Invalidate();
            }
        }

        public void Invalidate(Map invalidateMap, IEnumerable<Rectangle> cellBounds) {
            if(this.fullInvalidation) {
                return;
            }

            var count = this.invalidateCells.Count;
            this.invalidateCells.UnionWith(cellBounds.SelectMany(c => c.Points()));
            if(this.invalidateCells.Count > count) {
                var overlapCells = invalidateMap.Overlappers.Overlaps(this.invalidateCells).ToHashSet();
                this.invalidateCells.UnionWith(overlapCells);
                this.Invalidate();
            }
        }

        public void Invalidate(Map invalidateMap, Point location) {
            if(this.fullInvalidation) {
                return;
            }

            this.Invalidate(invalidateMap, new Rectangle(location, new Size(1, 1)));
        }

        public void Invalidate(Map invalidateMap, IEnumerable<Point> locations) {
            if(this.fullInvalidation) {
                return;
            }

            this.Invalidate(invalidateMap, locations.Select(l => new Rectangle(l, new Size(1, 1))));
        }

        public void Invalidate(Map invalidateMap, int cell) {
            if(this.fullInvalidation) {
                return;
            }

            if(invalidateMap.Metrics.GetLocation(cell, out var location)) {
                this.Invalidate(invalidateMap, location);
            }
        }

        public void Invalidate(Map invalidateMap, IEnumerable<int> cells) {
            if(this.fullInvalidation) {
                return;
            }

            this.Invalidate(invalidateMap, cells
                .Where(c => invalidateMap.Metrics.GetLocation(c, out var location))
                .Select(c => {
                    invalidateMap.Metrics.GetLocation(c, out var location);
                    return location;
                })
            );
        }

        public void Invalidate(Map invalidateMap, ICellOverlapper overlapper) {
            if(this.fullInvalidation) {
                return;
            }

            var rectangle = invalidateMap.Overlappers[overlapper];
            if(rectangle.HasValue) {
                this.Invalidate(invalidateMap, rectangle.Value);
            }
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);

            if(this.FocusOnMouseEnter) {
                this.Focus();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e) => this.Zoom += this.ZoomStep * Math.Sign(e.Delta);

        protected override void OnClientSizeChanged(EventArgs e) {
            base.OnClientSizeChanged(e);

            this.UpdateCamera();
        }

        protected override void OnScroll(ScrollEventArgs se) {
            base.OnScroll(se);

            this.InvalidateScroll();
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);

            e.Graphics.Clear(this.BackColor);
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);

            this.InvalidateScroll();

            PreRender?.Invoke(this, new RenderEventArgs(pe.Graphics, this.fullInvalidation ? null : this.invalidateCells));

            if(this.mapImage != null) {
                pe.Graphics.Transform = this.compositeTransform;

                var oldCompositingMode = pe.Graphics.CompositingMode;
                var oldCompositingQuality = pe.Graphics.CompositingQuality;
                var oldInterpolationMode = pe.Graphics.InterpolationMode;
                if(this.Quality > 1) {
                    pe.Graphics.CompositingMode = CompositingMode.SourceCopy;
                    pe.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
                }

                pe.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                pe.Graphics.DrawImage(this.mapImage, 0, 0);

                pe.Graphics.CompositingMode = oldCompositingMode;
                pe.Graphics.CompositingQuality = oldCompositingQuality;
                pe.Graphics.InterpolationMode = oldInterpolationMode;
            }

            PostRender?.Invoke(this, new RenderEventArgs(pe.Graphics, this.fullInvalidation ? null : this.invalidateCells));

#if DEVELOPER
            if(Globals.Developer.ShowOverlapCells) {
                var invalidPen = new Pen(Color.DarkRed);
                foreach(var cell in this.invalidateCells) {
                    pe.Graphics.DrawRectangle(invalidPen, new Rectangle(cell.X * Globals.TileWidth, cell.Y * Globals.TileHeight, Globals.TileWidth, Globals.TileHeight));
                }
            }
#endif

            this.invalidateCells.Clear();
            this.fullInvalidation = false;
        }

        private void UpdateCamera() {
            if(this.mapImage == null) {
                return;
            }

            if(this.ClientSize.IsEmpty) {
                return;
            }

            this.updatingCamera = true;

            var mapAspect = (double)this.mapImage.Width / this.mapImage.Height;
            var panelAspect = (double)this.ClientSize.Width / this.ClientSize.Height;
            var cameraLocation = this.cameraBounds.Location;

            var size = Size.Empty;
            if(panelAspect > mapAspect) {
                size.Height = this.mapImage.Height / this.zoom;
                size.Width = (int)(size.Height * panelAspect);
            } else {
                size.Width = this.mapImage.Width / this.zoom;
                size.Height = (int)(size.Width / panelAspect);
            }

            var location = Point.Empty;
            var scrollSize = Size.Empty;
            if(size.Width < this.mapImage.Width) {
                location.X = Math.Max(0, Math.Min(this.mapImage.Width - size.Width, this.cameraBounds.Left));
                scrollSize.Width = this.mapImage.Width * this.ClientSize.Width / size.Width;
            } else {
                location.X = (this.mapImage.Width - size.Width) / 2;
            }

            if(size.Height < this.mapImage.Height) {
                location.Y = Math.Max(0, Math.Min(this.mapImage.Height - size.Height, this.cameraBounds.Top));
                scrollSize.Height = this.mapImage.Height * this.ClientSize.Height / size.Height;
            } else {
                location.Y = (this.mapImage.Height - size.Height) / 2;
            }

            this.cameraBounds = new Rectangle(location, size);
            this.RecalculateTransforms();

            if(this.referencePositions.HasValue) {
                var mapPoint = this.referencePositions.Value.map;
                var clientSize = this.referencePositions.Value.client;

                cameraLocation = this.cameraBounds.Location;
                if(scrollSize.Width != 0) {
                    cameraLocation.X = Math.Max(0, Math.Min(this.mapImage.Width - this.cameraBounds.Width, (int)(mapPoint.X - (this.cameraBounds.Width * clientSize.Width))));
                }
                if(scrollSize.Height != 0) {
                    cameraLocation.Y = Math.Max(0, Math.Min(this.mapImage.Height - this.cameraBounds.Height, (int)(mapPoint.Y - (this.cameraBounds.Height * clientSize.Height))));
                }
                if(!scrollSize.IsEmpty) {
                    this.cameraBounds.Location = cameraLocation;
                    this.RecalculateTransforms();
                }

                this.referencePositions = null;
            }

            this.SuspendDrawing();
            this.AutoScrollMinSize = scrollSize;
            this.AutoScrollPosition = (Point)this.MapToClient((Size)this.cameraBounds.Location);
            this.lastScrollPosition = this.AutoScrollPosition;
            this.ResumeDrawing();

            this.updatingCamera = false;

            this.Invalidate();
        }

        private void RecalculateTransforms() {
            this.mapToViewTransform.Reset();
            this.mapToViewTransform.Translate(this.cameraBounds.Left, this.cameraBounds.Top);
            this.mapToViewTransform.Scale(this.cameraBounds.Width, this.cameraBounds.Height);
            this.mapToViewTransform.Invert();

            this.viewToPageTransform.Reset();
            this.viewToPageTransform.Scale(this.ClientSize.Width, this.ClientSize.Height);

            this.compositeTransform.Reset();
            this.compositeTransform.Multiply(this.viewToPageTransform);
            this.compositeTransform.Multiply(this.mapToViewTransform);

            this.invCompositeTransform.Reset();
            this.invCompositeTransform.Multiply(this.compositeTransform);
            this.invCompositeTransform.Invert();
        }

        private void InvalidateScroll() {
            if(this.updatingCamera) {
                return;
            }

            if((this.lastScrollPosition.X != this.AutoScrollPosition.X) || (this.lastScrollPosition.Y != this.AutoScrollPosition.Y)) {
                var delta = this.ClientToMap((Size)(this.lastScrollPosition - (Size)this.AutoScrollPosition));
                this.lastScrollPosition = this.AutoScrollPosition;

                var cameraLocation = this.cameraBounds.Location;
                if(this.AutoScrollMinSize.Width != 0) {
                    cameraLocation.X = Math.Max(0, Math.Min(this.mapImage.Width - this.cameraBounds.Width, this.cameraBounds.Left + delta.Width));
                }
                if(this.AutoScrollMinSize.Height != 0) {
                    cameraLocation.Y = Math.Max(0, Math.Min(this.mapImage.Height - this.cameraBounds.Height, this.cameraBounds.Top + delta.Height));
                }
                if(!this.AutoScrollMinSize.IsEmpty) {
                    this.cameraBounds.Location = cameraLocation;
                    this.RecalculateTransforms();
                }

                this.Invalidate();
            }
        }

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

        private const int WM_SETREDRAW = 11;

        private void SuspendDrawing() => SendMessage(this.Handle, WM_SETREDRAW, false, 0);

        private void ResumeDrawing() => SendMessage(this.Handle, WM_SETREDRAW, true, 0);
    }
}
