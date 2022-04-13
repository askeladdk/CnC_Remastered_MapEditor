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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class MenuButton : Button {
        public const int DefaultSplitWidth = 20;

        [DefaultValue(null), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContextMenuStrip Menu {
            get; set;
        }

        [DefaultValue(DefaultSplitWidth), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int SplitWidth { get; set; } = DefaultSplitWidth;

        public MenuButton() => this.InitializeComponent();

        protected override void OnMouseDown(MouseEventArgs mevent) {
            var splitRect = new Rectangle(this.Width - this.SplitWidth, 0, this.SplitWidth, this.Height);

            if((this.Menu != null) && (mevent.Button == MouseButtons.Left) && splitRect.Contains(mevent.Location)) {
                this.Menu.Show(this, 0, this.Height);
            } else {
                base.OnMouseDown(mevent);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent) {
            base.OnPaint(pevent);

            if((this.Menu != null) && (this.SplitWidth > 0)) {
                var arrowX = this.ClientRectangle.Width - 14;
                var arrowY = this.ClientRectangle.Height / 2 - 1;

                var arrowBrush = this.Enabled ? SystemBrushes.ControlText : SystemBrushes.ButtonShadow;
                var arrows = new[] { new Point(arrowX, arrowY), new Point(arrowX + 7, arrowY), new Point(arrowX + 3, arrowY + 4) };
                pevent.Graphics.FillPolygon(arrowBrush, arrows);

                var lineX = this.ClientRectangle.Width - this.SplitWidth;
                var lineYFrom = arrowY - 4;
                var lineYTo = arrowY + 8;
                using(var separatorPen = new Pen(Brushes.DarkGray) { DashStyle = DashStyle.Dot }) {
                    pevent.Graphics.DrawLine(separatorPen, lineX, lineYFrom, lineX, lineYTo);
                }
            }
        }
    }
}
