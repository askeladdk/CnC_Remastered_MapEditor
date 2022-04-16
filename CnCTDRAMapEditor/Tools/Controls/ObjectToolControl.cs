using MobiusEditor.Controls;
using System.Windows.Forms;

namespace MobiusEditor.Tools.Controls {
    public partial class ObjectToolControl : UserControl {

        public ListView ObjectTypeListView => this.objectTypeListView;

        public ObjectProperties ObjectProperties => this.objectProperties;

        public ObjectToolControl() {
            this.InitializeComponent();
        }
    }
}
