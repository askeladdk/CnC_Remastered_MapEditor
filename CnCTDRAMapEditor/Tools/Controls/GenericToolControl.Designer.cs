
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
            this.genericTypeListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // genericTypeListView
            // 
            this.genericTypeListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genericTypeListView.HideSelection = false;
            this.genericTypeListView.Location = new System.Drawing.Point(0, 0);
            this.genericTypeListView.MultiSelect = false;
            this.genericTypeListView.Name = "genericTypeListView";
            this.genericTypeListView.Size = new System.Drawing.Size(210, 254);
            this.genericTypeListView.TabIndex = 1;
            this.genericTypeListView.UseCompatibleStateImageBehavior = false;
            this.genericTypeListView.View = System.Windows.Forms.View.List;
            // 
            // GenericToolControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.genericTypeListView);
            this.Name = "GenericToolControl";
            this.Size = new System.Drawing.Size(210, 254);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView genericTypeListView;
    }
}
