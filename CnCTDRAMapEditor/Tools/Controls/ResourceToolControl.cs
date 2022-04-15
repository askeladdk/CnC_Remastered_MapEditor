using System.Windows.Forms;

namespace MobiusEditor.Tools.Controls {
    public partial class ResourceToolControl : UserControl {
        public Label TotalResourcesLbl => this.totalResourcesLbl;

        public NumericUpDown ResourceBrushSizeNud => this.resourceBrushSizeNud;

        public CheckBox GemsCheckBox => this.gemsCheckBox;

        public ResourceToolControl() {
            this.InitializeComponent();
        }
    }
}
