
namespace MobiusEditor.Tools.Controls {
    partial class ResourceToolControl {
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.totalResourcesLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.resourceBrushSizeNud = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.gemsCheckBox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resourceBrushSizeNud)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.totalResourcesLbl, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.resourceBrushSizeNud, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.gemsCheckBox, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(192, 118);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // totalResourcesLbl
            // 
            this.totalResourcesLbl.AutoSize = true;
            this.totalResourcesLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalResourcesLbl.Location = new System.Drawing.Point(99, 0);
            this.totalResourcesLbl.Name = "totalResourcesLbl";
            this.totalResourcesLbl.Size = new System.Drawing.Size(90, 39);
            this.totalResourcesLbl.TabIndex = 0;
            this.totalResourcesLbl.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "Total Resources";
            // 
            // resourceBrushSizeNud
            // 
            this.resourceBrushSizeNud.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourceBrushSizeNud.Location = new System.Drawing.Point(99, 42);
            this.resourceBrushSizeNud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.resourceBrushSizeNud.Name = "resourceBrushSizeNud";
            this.resourceBrushSizeNud.Size = new System.Drawing.Size(90, 20);
            this.resourceBrushSizeNud.TabIndex = 2;
            this.resourceBrushSizeNud.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 39);
            this.label2.TabIndex = 3;
            this.label2.Text = "Brush Size";
            // 
            // gemsCheckBox
            // 
            this.gemsCheckBox.AutoSize = true;
            this.gemsCheckBox.Location = new System.Drawing.Point(3, 81);
            this.gemsCheckBox.Name = "gemsCheckBox";
            this.gemsCheckBox.Size = new System.Drawing.Size(53, 17);
            this.gemsCheckBox.TabIndex = 4;
            this.gemsCheckBox.Text = "Gems";
            this.gemsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ResourceToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ResourceToolControl";
            this.Size = new System.Drawing.Size(192, 118);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resourceBrushSizeNud)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label totalResourcesLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown resourceBrushSizeNud;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox gemsCheckBox;
    }
}
