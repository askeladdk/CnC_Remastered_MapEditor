using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class GenericToolControl : UserControl {
        public ListView GenericTypeListView => this.genericTypeListView;

        public MapPanel GenericTypeMapPanel => this.genericTypeMapPanel;

        public GenericToolControl() {
            this.InitializeComponent();
        }
    }
}
