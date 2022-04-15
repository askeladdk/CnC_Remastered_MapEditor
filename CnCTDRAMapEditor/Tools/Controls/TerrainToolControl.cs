using MobiusEditor.Interface;
using System.Windows.Forms;

namespace MobiusEditor.Controls {
    public partial class TerrainToolControl : UserControl {
        public ListView TerrainTypeListView => this.terrainTypeListView;

        public MapPanel TerrainTypeMapPanel => this.terrainTypeMapPanel;

        public TerrainProperties TerrainProperties => this.terrainProperties;

        public TerrainToolControl() {
            this.InitializeComponent();
        }

        public void Initialize(IGamePlugin plugin) {
            this.terrainProperties.Initialize(plugin, true);
        }
    }
}
