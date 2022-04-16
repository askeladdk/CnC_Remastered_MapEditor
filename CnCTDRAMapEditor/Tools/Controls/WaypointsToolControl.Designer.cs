
namespace MobiusEditor.Tools.Controls {
    partial class WaypointsToolControl {
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
            this.waypointsListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // waypointsListView
            // 
            this.waypointsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.waypointsListView.HideSelection = false;
            this.waypointsListView.Location = new System.Drawing.Point(0, 0);
            this.waypointsListView.MultiSelect = false;
            this.waypointsListView.Name = "waypointsListView";
            this.waypointsListView.Size = new System.Drawing.Size(150, 150);
            this.waypointsListView.TabIndex = 0;
            this.waypointsListView.UseCompatibleStateImageBehavior = false;
            this.waypointsListView.View = System.Windows.Forms.View.List;
            // 
            // WaypointsToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.waypointsListView);
            this.Name = "WaypointsToolControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView waypointsListView;
    }
}
