
namespace MobiusEditor.Controls {
    partial class TerrainToolControl {
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
            this.terrainTypeListView = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.terrainProperties = new MobiusEditor.Controls.TerrainProperties();
            this.terrainTypeMapPanel = new MobiusEditor.Controls.MapPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.terrainTypeListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(230, 294);
            this.splitContainer1.SplitterDistance = 141;
            this.splitContainer1.TabIndex = 0;
            // 
            // terrainTypeListView
            // 
            this.terrainTypeListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terrainTypeListView.HideSelection = false;
            this.terrainTypeListView.Location = new System.Drawing.Point(0, 0);
            this.terrainTypeListView.Name = "terrainTypeListView";
            this.terrainTypeListView.Size = new System.Drawing.Size(230, 141);
            this.terrainTypeListView.TabIndex = 0;
            this.terrainTypeListView.UseCompatibleStateImageBehavior = false;
            this.terrainTypeListView.View = System.Windows.Forms.View.SmallIcon;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.terrainProperties, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.terrainTypeMapPanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 78.65169F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 21.34831F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(230, 149);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // terrainProperties
            // 
            this.terrainProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.terrainProperties.Location = new System.Drawing.Point(3, 120);
            this.terrainProperties.Name = "terrainProperties";
            this.terrainProperties.Size = new System.Drawing.Size(224, 26);
            this.terrainProperties.TabIndex = 0;
            this.terrainProperties.Terrain = null;
            // 
            // terrainTypeMapPanel
            // 
            this.terrainTypeMapPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.terrainTypeMapPanel.Location = new System.Drawing.Point(3, 3);
            this.terrainTypeMapPanel.MapImage = null;
            this.terrainTypeMapPanel.MaxZoom = 8;
            this.terrainTypeMapPanel.MinZoom = 1;
            this.terrainTypeMapPanel.Name = "terrainTypeMapPanel";
            this.terrainTypeMapPanel.Quality = 2;
            this.terrainTypeMapPanel.Size = new System.Drawing.Size(224, 111);
            this.terrainTypeMapPanel.TabIndex = 1;
            this.terrainTypeMapPanel.Zoom = 1;
            this.terrainTypeMapPanel.ZoomStep = 1;
            // 
            // TerrainToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "TerrainToolControl";
            this.Size = new System.Drawing.Size(230, 294);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView terrainTypeListView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private TerrainProperties terrainProperties;
        private MapPanel terrainTypeMapPanel;
    }
}
