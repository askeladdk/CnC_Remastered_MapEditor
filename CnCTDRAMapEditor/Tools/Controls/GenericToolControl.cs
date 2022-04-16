using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class GenericToolControl : UserControl {
        public ListView GenericTypeListView => this.genericTypeListView;

        public GenericToolControl() {
            this.InitializeComponent();
        }
    }
}
