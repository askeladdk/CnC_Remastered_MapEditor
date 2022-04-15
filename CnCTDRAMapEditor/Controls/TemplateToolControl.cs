using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class TemplateToolControl : UserControl {
        public ListView TemplateTypeListView => this.templateTypeListView;

        public MapPanel TemplateTypeMapPanel => this.templateTypeMapPanel;

        public TemplateToolControl() => this.InitializeComponent();
    }
}
