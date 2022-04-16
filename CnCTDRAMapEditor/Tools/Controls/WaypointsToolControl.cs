using System.Windows.Forms;

namespace MobiusEditor.Tools.Controls {
    public partial class WaypointsToolControl : UserControl {
        public ListView WaypointListView => this.waypointsListView;

        public WaypointsToolControl() {
            this.InitializeComponent();
        }
    }
}
