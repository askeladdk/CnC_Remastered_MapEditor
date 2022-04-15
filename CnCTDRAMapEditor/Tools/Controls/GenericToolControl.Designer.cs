
namespace MobiusEditor.Controls {
    partial class GenericToolControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.genericTypeListView = new System.Windows.Forms.ListView();
            this.genericTypeMapPanel = new MobiusEditor.Controls.MapPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.genericTypeListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.genericTypeMapPanel);
            this.splitContainer1.Size = new System.Drawing.Size(210, 254);
            this.splitContainer1.SplitterDistance = 123;
            this.splitContainer1.TabIndex = 0;
            // 
            // genericTypeListView
            // 
            this.genericTypeListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genericTypeListView.HideSelection = false;
            this.genericTypeListView.Location = new System.Drawing.Point(0, 0);
            this.genericTypeListView.Name = "genericTypeListView";
            this.genericTypeListView.Size = new System.Drawing.Size(210, 123);
            this.genericTypeListView.TabIndex = 0;
            this.genericTypeListView.UseCompatibleStateImageBehavior = false;
            this.genericTypeListView.View = System.Windows.Forms.View.SmallIcon;
            // 
            // genericTypeMapPanel
            // 
            this.genericTypeMapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genericTypeMapPanel.Location = new System.Drawing.Point(0, 0);
            this.genericTypeMapPanel.MapImage = null;
            this.genericTypeMapPanel.MaxZoom = 8;
            this.genericTypeMapPanel.MinZoom = 1;
            this.genericTypeMapPanel.Name = "genericTypeMapPanel";
            this.genericTypeMapPanel.Quality = 2;
            this.genericTypeMapPanel.Size = new System.Drawing.Size(210, 127);
            this.genericTypeMapPanel.TabIndex = 0;
            this.genericTypeMapPanel.Zoom = 1;
            this.genericTypeMapPanel.ZoomStep = 1;
            // 
            // GenericToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "GenericToolControl";
            this.Size = new System.Drawing.Size(210, 254);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView genericTypeListView;
        private MapPanel genericTypeMapPanel;
    }
}
