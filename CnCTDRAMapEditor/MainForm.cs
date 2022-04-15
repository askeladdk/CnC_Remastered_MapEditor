//
// Copyright 2020 Electronic Arts Inc.
//
// The Command & Conquer Map Editor and corresponding source code is free
// software: you can redistribute it and/or modify it under the terms of
// the GNU General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.

// The Command & Conquer Map Editor and corresponding source code is distributed
// in the hope that it will be useful, but with permitted additional restrictions
// under Section 7 of the GPL. See the GNU General Public License in LICENSE.TXT
// distributed with this program. You should have received a copy of the
// GNU General Public License along with permitted additional restrictions
// with this program. If not, see https://github.com/electronicarts/CnC_Remastered_Collection
using MobiusEditor.Dialogs;
using MobiusEditor.Event;
using MobiusEditor.Interface;
using MobiusEditor.Model;
using MobiusEditor.Tools;
using MobiusEditor.Tools.Dialogs;
using MobiusEditor.Utility;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiusEditor {
    public partial class MainForm : Form {
        [Flags]
        private enum ToolType {
            None = 0,
            Map = 1 << 0,
            Smudge = 1 << 1,
            Overlay = 1 << 2,
            Terrain = 1 << 3,
            Infantry = 1 << 4,
            Unit = 1 << 5,
            Building = 1 << 6,
            Resources = 1 << 7,
            Wall = 1 << 8,
            Waypoint = 1 << 9,
            CellTrigger = 1 << 10
        }

        private static readonly ToolType[] toolTypes;

        private ToolType availableToolTypes = ToolType.None;

        private ToolType activeToolType = ToolType.None;
        private ToolType ActiveToolType {
            get => this.activeToolType;
            set {
                var firstAvailableTool = value;
                if((this.availableToolTypes & firstAvailableTool) == ToolType.None) {
                    var otherAvailableToolTypes = toolTypes.Where(t => (this.availableToolTypes & t) != ToolType.None);
                    firstAvailableTool = otherAvailableToolTypes.Any() ? otherAvailableToolTypes.First() : ToolType.None;
                }

                if(this.activeToolType != firstAvailableTool) {
                    this.activeToolType = firstAvailableTool;
                    this.RefreshActiveTool();
                }
            }
        }

        private MapLayerFlag activeLayers;
        public MapLayerFlag ActiveLayers {
            get => this.activeLayers;
            set {
                if(this.activeLayers != value) {
                    this.activeLayers = value;
                    if(this.activeTool != null) {
                        this.activeTool.Layers = this.ActiveLayers;
                    }
                }
            }
        }

        private ITool activeTool;
        private Form activeToolForm;

        private IGamePlugin plugin;
        private string filename;

        private readonly MRU mru;

        private readonly UndoRedoList<UndoRedoEventArgs> url = new UndoRedoList<UndoRedoEventArgs>();

        private readonly Timer steamUpdateTimer = new Timer();

        static MainForm() {
            toolTypes = ((IEnumerable<ToolType>)Enum.GetValues(typeof(ToolType))).Where(t => t != ToolType.None).ToArray();
        }

        public MainForm() {
            this.InitializeComponent();

            this.mru = new MRU("Software\\Petroglyph\\CnCRemasteredEditor", 10, this.fileRecentFilesMenuItem);
            this.mru.FileSelected += this.Mru_FileSelected;

            foreach(ToolStripButton toolStripButton in this.mainToolStrip.Items) {
                toolStripButton.MouseMove += this.mainToolStrip_MouseMove;
            }

#if !DEVELOPER
            fileExportMenuItem.Visible = false;
            developerToolStripMenuItem.Visible = false;
#endif

            this.url.Tracked += this.UndoRedo_Updated;
            this.url.Undone += this.UndoRedo_Updated;
            this.url.Redone += this.UndoRedo_Updated;
            this.UpdateUndoRedo();

            this.steamUpdateTimer.Interval = 500;
            this.steamUpdateTimer.Tick += this.SteamUpdateTimer_Tick;
        }

        private void SteamUpdateTimer_Tick(object sender, EventArgs e) {
            if(SteamworksUGC.IsInit) {
                SteamworksUGC.Service();
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            this.RefreshAvailableTools();
            this.UpdateVisibleLayers();

            this.filePublishMenuItem.Visible = SteamworksUGC.IsInit;

            this.steamUpdateTimer.Start();
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);

            this.steamUpdateTimer.Stop();
            this.steamUpdateTimer.Dispose();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if(keyData == Keys.Q) {
                this.mapToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.W) {
                this.smudgeToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.E) {
                this.overlayToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.R) {
                this.terrainToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.T) {
                this.infantryToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.Y) {
                this.unitToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.A) {
                this.buildingToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.S) {
                this.resourcesToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.D) {
                this.wallsToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.F) {
                this.waypointsToolStripButton.PerformClick();
                return true;
            } else if(keyData == Keys.G) {
                this.cellTriggersToolStripButton.PerformClick();
                return true;
            } else if(keyData == (Keys.Control | Keys.Z)) {
                if(this.editUndoMenuItem.Enabled) {
                    this.editUndoMenuItem_Click(this, new EventArgs());
                }
                return true;
            } else if(keyData == (Keys.Control | Keys.Y)) {
                if(this.editRedoMenuItem.Enabled) {
                    this.editRedoMenuItem_Click(this, new EventArgs());
                }
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void UpdateUndoRedo() {
            this.editUndoMenuItem.Enabled = this.url.CanUndo;
            this.editRedoMenuItem.Enabled = this.url.CanRedo;
        }

        private void UndoRedo_Updated(object sender, EventArgs e) {
            this.UpdateUndoRedo();
        }

        private void fileNewMenuItem_Click(object sender, EventArgs e) {
            if(!this.PromptSaveMap()) {
                return;
            }

            var nmd = new NewMapDialog();
            if(nmd.ShowDialog() == DialogResult.OK) {
                if(this.plugin != null) {
                    this.plugin.Map.Triggers.CollectionChanged -= this.Triggers_CollectionChanged;
                    this.plugin.Dispose();
                }
                this.plugin = null;

                Globals.TheTilesetManager.Reset();
                Globals.TheTextureManager.Reset();

                if(nmd.GameType == GameType.TiberianDawn) {
                    Globals.TheTeamColorManager.Reset();
                    Globals.TheTeamColorManager.Load(@"DATA\XML\CNCTDTEAMCOLORS.XML");

                    this.plugin = new TiberianDawn.GamePlugin();
                    this.plugin.New(nmd.TheaterName);
                } else if(nmd.GameType == GameType.RedAlert) {
                    Globals.TheTeamColorManager.Reset();
                    Globals.TheTeamColorManager.Load(@"DATA\XML\CNCRATEAMCOLORS.XML");

                    this.plugin = new RedAlert.GamePlugin();
                    this.plugin.New(nmd.TheaterName);
                }

                if(SteamworksUGC.IsInit) {
                    this.plugin.Map.BasicSection.Author = SteamFriends.GetPersonaName();
                }

                this.plugin.Map.Triggers.CollectionChanged += this.Triggers_CollectionChanged;
                this.mapPanel.MapImage = this.plugin.MapImage;

                this.filename = null;
                this.Text = "CnC TDRA Map Editor";
                this.url.Clear();

                this.ClearActiveTool();
                this.RefreshAvailableTools();
                this.RefreshActiveTool();
            }
        }

        private void fileOpenMenuItem_Click(object sender, EventArgs e) {
            if(!this.PromptSaveMap()) {
                return;
            }

            var pgmFilter =
#if DEVELOPER
                "|PGM files (*.pgm)|*.pgm"
#else
                string.Empty
#endif
            ;

            var ofd = new OpenFileDialog {
                AutoUpgradeEnabled = false,
                RestoreDirectory = true
            };
            ofd.Filter = "Tiberian Dawn files (*.ini;*.bin)|*.ini;*.bin|Red Alert files (*.mpr)|*.mpr" + pgmFilter + "|All files (*.*)|*.*";
            if(this.plugin != null) {
                switch(this.plugin.GameType) {
                case GameType.TiberianDawn:
                    ofd.InitialDirectory = TiberianDawn.Constants.SaveDirectory;
                    ofd.FilterIndex = 1;
                    break;
                case GameType.RedAlert:
                    ofd.InitialDirectory = RedAlert.Constants.SaveDirectory;
                    ofd.FilterIndex = 2;
                    break;
                }
            } else {
                ofd.InitialDirectory = Globals.RootSaveDirectory;
            }
            if(ofd.ShowDialog() == DialogResult.OK) {
                var fileInfo = new FileInfo(ofd.FileName);
                if(this.LoadFile(fileInfo.FullName)) {
                    this.mru.Add(fileInfo);
                } else {
                    this.mru.Remove(fileInfo);
                    MessageBox.Show(string.Format("Error loading {0}.", ofd.FileName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void fileSaveMenuItem_Click(object sender, EventArgs e) {
            if(this.plugin == null) {
                return;
            }

            if(string.IsNullOrEmpty(this.filename)) {
                this.fileSaveAsMenuItem.PerformClick();
            } else {
                var fileInfo = new FileInfo(this.filename);
                if(this.SaveFile(fileInfo.FullName)) {
                    this.mru.Add(fileInfo);
                } else {
                    this.mru.Remove(fileInfo);
                }
            }
        }

        private void fileSaveAsMenuItem_Click(object sender, EventArgs e) {
            if(this.plugin == null) {
                return;
            }

            var sfd = new SaveFileDialog {
                AutoUpgradeEnabled = false,
                RestoreDirectory = true
            };
            var filters = new List<string>();
            switch(this.plugin.GameType) {
            case GameType.TiberianDawn:
                filters.Add("Tiberian Dawn files (*.ini;*.bin)|*.ini;*.bin");
                sfd.InitialDirectory = TiberianDawn.Constants.SaveDirectory;
                break;
            case GameType.RedAlert:
                filters.Add("Red Alert files (*.mpr)|*.mpr");
                sfd.InitialDirectory = RedAlert.Constants.SaveDirectory;
                break;
            }
            filters.Add("All files (*.*)|*.*");

            sfd.Filter = string.Join("|", filters);
            if(!string.IsNullOrEmpty(this.filename)) {
                sfd.InitialDirectory = Path.GetDirectoryName(this.filename);
                sfd.FileName = Path.GetFileName(this.filename);
            }
            if(sfd.ShowDialog() == DialogResult.OK) {
                var fileInfo = new FileInfo(sfd.FileName);
                if(this.SaveFile(fileInfo.FullName)) {
                    this.mru.Add(fileInfo);
                } else {
                    this.mru.Remove(fileInfo);
                }
            }
        }

        private void fileExportMenuItem_Click(object sender, EventArgs e) {
            if(this.plugin == null) {
                return;
            }

            var sfd = new SaveFileDialog {
                AutoUpgradeEnabled = false,
                RestoreDirectory = true
            };
            sfd.Filter = "MEG files (*.meg)|*.meg";
            if(sfd.ShowDialog() == DialogResult.OK) {
                this.plugin.Save(sfd.FileName, FileType.MEG);
            }
        }

        private void fileExitMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void editUndoMenuItem_Click(object sender, EventArgs e) {
            if(this.url.CanUndo) {
                this.url.Undo(new UndoRedoEventArgs(this.mapPanel, this.plugin.Map));
            }
        }

        private void editRedoMenuItem_Click(object sender, EventArgs e) {
            if(this.url.CanRedo) {
                this.url.Redo(new UndoRedoEventArgs(this.mapPanel, this.plugin.Map));
            }
        }

        private void settingsMapSettingsMenuItem_Click(object sender, EventArgs e) {
            if(this.plugin == null) {
                return;
            }

            var basicSettings = new PropertyTracker<BasicSection>(this.plugin.Map.BasicSection);
            var briefingSettings = new PropertyTracker<BriefingSection>(this.plugin.Map.BriefingSection);
            var houseSettingsTrackers = this.plugin.Map.Houses.ToDictionary(h => h, h => new PropertyTracker<House>(h));

            var msd = new MapSettingsDialog(this.plugin, basicSettings, briefingSettings, houseSettingsTrackers);
            if(msd.ShowDialog() == DialogResult.OK) {
                basicSettings.Commit();
                briefingSettings.Commit();
                foreach(var houseSettingsTracker in houseSettingsTrackers.Values) {
                    houseSettingsTracker.Commit();
                }
                this.plugin.Dirty = true;
            }
        }

        private void settingsTeamTypesMenuItem_Click(object sender, EventArgs e) {
            if(this.plugin == null) {
                return;
            }

            var maxTeams = 0;
            switch(this.plugin.GameType) {
            case GameType.TiberianDawn: {
                maxTeams = TiberianDawn.Constants.MaxTeams;
            }
            break;
            case GameType.RedAlert: {
                maxTeams = RedAlert.Constants.MaxTeams;
            }
            break;
            }

            var ttd = new TeamTypesDialog(this.plugin, maxTeams);
            if(ttd.ShowDialog() == DialogResult.OK) {
                this.plugin.Map.TeamTypes.Clear();
                this.plugin.Map.TeamTypes.AddRange(ttd.TeamTypes.Select(t => t.Clone()));
                this.plugin.Dirty = true;
            }
        }

        private void settingsTriggersMenuItem_Click(object sender, EventArgs e) {
            if(this.plugin == null) {
                return;
            }

            var maxTriggers = 0;
            switch(this.plugin.GameType) {
            case GameType.TiberianDawn: {
                maxTriggers = TiberianDawn.Constants.MaxTriggers;
            }
            break;
            case GameType.RedAlert: {
                maxTriggers = RedAlert.Constants.MaxTriggers;
            }
            break;
            }

            var td = new TriggersDialog(this.plugin, maxTriggers);
            if(td.ShowDialog() == DialogResult.OK) {
                var oldTriggers =
                    from leftTrigger in this.plugin.Map.Triggers
                    join rightTrigger in td.Triggers
                    on leftTrigger.Name equals rightTrigger.Name into result
                    where result.Count() == 0
                    select leftTrigger;
                var newTriggers =
                    from leftTrigger in td.Triggers
                    join rightTrigger in this.plugin.Map.Triggers
                    on leftTrigger.Name equals rightTrigger.Name into result
                    where result.Count() == 0
                    select leftTrigger;
                var sameTriggers =
                    from leftTrigger in this.plugin.Map.Triggers
                    join rightTrigger in td.Triggers
                    on leftTrigger.Name equals rightTrigger.Name
                    select new {
                        OldTrigger = leftTrigger,
                        NewTrigger = rightTrigger
                    };

                foreach(var oldTrigger in oldTriggers.ToArray()) {
                    this.plugin.Map.Triggers.Remove(oldTrigger);
                }

                foreach(var newTrigger in newTriggers.ToArray()) {
                    this.plugin.Map.Triggers.Add(newTrigger.Clone());
                }

                foreach(var item in sameTriggers.ToArray()) {
                    this.plugin.Map.Triggers.Add(item.NewTrigger.Clone());
                    this.plugin.Map.Triggers.Remove(item.OldTrigger);
                }

                this.plugin.Dirty = true;
            }
        }

        private void Mru_FileSelected(object sender, FileInfo e) {
            if(!this.PromptSaveMap()) {
                return;
            }

            if(this.LoadFile(e.FullName)) {
                this.mru.Add(e);
            } else {
                this.mru.Remove(e);
                MessageBox.Show(string.Format("Error loading {0}.", e.FullName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mapPanel_MouseMove(object sender, MouseEventArgs e) {
            if(this.plugin != null) {
                var mapPoint = this.mapPanel.ClientToMap(e.Location);
                var location = new Point((int)Math.Floor((double)mapPoint.X / Globals.TileWidth), (int)Math.Floor((double)mapPoint.Y / Globals.TileHeight));
                if(this.plugin.Map.Metrics.GetCell(location, out var cell)) {
                    var sb = new StringBuilder();
                    sb.AppendFormat("X = {0}, Y = {1}, Cell = {2}", location.X, location.Y, cell);

                    var template = this.plugin.Map.Templates[cell];
                    var templateType = template?.Type;
                    if(templateType != null) {
                        sb.AppendFormat(", Template = {0} ({1})", templateType.DisplayName, template.Icon);
                    }

                    var smudge = this.plugin.Map.Smudge[cell];
                    var smudgeType = smudge?.Type;
                    if(smudgeType != null) {
                        sb.AppendFormat(", Smudge = {0}", smudgeType.DisplayName);
                    }

                    var overlay = this.plugin.Map.Overlay[cell];
                    var overlayType = overlay?.Type;
                    if(overlayType != null) {
                        sb.AppendFormat(", Overlay = {0}", overlayType.DisplayName);
                    }

                    var terrain = this.plugin.Map.Technos[location] as Terrain;
                    var terrainType = terrain?.Type;
                    if(terrainType != null) {
                        sb.AppendFormat(", Terrain = {0}", terrainType.DisplayName);
                    }

                    if(this.plugin.Map.Technos[location] is InfantryGroup infantryGroup) {
                        var subPixel = new Point(
                            (mapPoint.X * Globals.PixelWidth / Globals.TileWidth) % Globals.PixelWidth,
                            (mapPoint.Y * Globals.PixelHeight / Globals.TileHeight) % Globals.PixelHeight
                        );

                        var i = InfantryGroup.ClosestStoppingTypes(subPixel).Cast<int>().First();
                        if(infantryGroup.Infantry[i] != null) {
                            sb.AppendFormat(", Infantry = {0}", infantryGroup.Infantry[i].Type.DisplayName);
                        }
                    }

                    var unit = this.plugin.Map.Technos[location] as Unit;
                    var unitType = unit?.Type;
                    if(unitType != null) {
                        sb.AppendFormat(", Unit = {0}", unitType.DisplayName);
                    }

                    var building = this.plugin.Map.Technos[location] as Building;
                    var buildingType = building?.Type;
                    if(buildingType != null) {
                        sb.AppendFormat(", Building = {0}", buildingType.DisplayName);
                    }

                    this.cellStatusLabel.Text = sb.ToString();
                } else {
                    this.cellStatusLabel.Text = string.Empty;
                }
            }
        }

        private bool LoadFile(string loadFilename) {
            var fileType = FileType.None;
            switch(Path.GetExtension(loadFilename).ToLower()) {
            case ".ini":
            case ".mpr":
                fileType = FileType.INI;
                break;
            case ".bin":
                fileType = FileType.BIN;
                break;
#if DEVELOPER
            case ".pgm":
                fileType = FileType.PGM;
                break;
#endif
            }

            if(fileType == FileType.None) {
                return false;
            }

            var gameType = GameType.None;
            switch(fileType) {
            case FileType.INI: {
                var ini = new INI();
                try {
                    using(var reader = new StreamReader(loadFilename)) {
                        ini.Parse(reader);
                    }
                } catch(FileNotFoundException) {
                    return false;
                }
                gameType = File.Exists(Path.ChangeExtension(loadFilename, ".bin")) ? GameType.TiberianDawn : GameType.RedAlert;
            }
            break;
            case FileType.BIN:
                gameType = GameType.TiberianDawn;
                break;
#if DEVELOPER
            case FileType.PGM: {
                try {
                    using(var megafile = new Megafile(loadFilename)) {
                        if(megafile.Any(f => Path.GetExtension(f).ToLower() == ".mpr")) {
                            gameType = GameType.RedAlert;
                        } else {
                            gameType = GameType.TiberianDawn;
                        }
                    }
                } catch(FileNotFoundException) {
                    return false;
                }
            }
            break;
#endif
            }

            if(gameType == GameType.None) {
                return false;
            }

            if(this.plugin != null) {
                this.plugin.Map.Triggers.CollectionChanged -= this.Triggers_CollectionChanged;
                this.plugin.Dispose();
            }
            this.plugin = null;

            Globals.TheTilesetManager.Reset();
            Globals.TheTextureManager.Reset();

            switch(gameType) {
            case GameType.TiberianDawn: {
                Globals.TheTeamColorManager.Reset();
                Globals.TheTeamColorManager.Load(@"DATA\XML\CNCTDTEAMCOLORS.XML");
                this.plugin = new TiberianDawn.GamePlugin();
            }
            break;
            case GameType.RedAlert: {
                Globals.TheTeamColorManager.Reset();
                Globals.TheTeamColorManager.Load(@"DATA\XML\CNCRATEAMCOLORS.XML");
                this.plugin = new RedAlert.GamePlugin();
            }
            break;
            }

            try {
                var errors = this.plugin.Load(loadFilename, fileType).ToArray();
                if(errors.Length > 0) {
                    var errorMessageBox = new ErrorMessageBox { Errors = errors };
                    errorMessageBox.ShowDialog();
                }
            } catch(Exception) {
#if DEVELOPER
                throw;
#else
                return false;
#endif
            }

            this.plugin.Map.Triggers.CollectionChanged += this.Triggers_CollectionChanged;
            this.mapPanel.MapImage = this.plugin.MapImage;

            this.plugin.Dirty = false;
            this.filename = loadFilename;
            this.Text = string.Format("CnC TDRA Map Editor - {0}", this.filename);

            this.url.Clear();

            this.ClearActiveTool();
            this.RefreshAvailableTools();
            this.RefreshActiveTool();

            return true;
        }

        private bool SaveFile(string saveFilename) {
            var fileType = FileType.None;
            switch(Path.GetExtension(saveFilename).ToLower()) {
            case ".ini":
            case ".mpr":
                fileType = FileType.INI;
                break;
            case ".bin":
                fileType = FileType.BIN;
                break;
            }

            if(fileType == FileType.None) {
                return false;
            }

            if(string.IsNullOrEmpty(this.plugin.Map.SteamSection.Title)) {
                this.plugin.Map.SteamSection.Title = this.plugin.Map.BasicSection.Name;
            }

            if(!this.plugin.Save(saveFilename, fileType)) {
                return false;
            }

            if(new FileInfo(saveFilename).Length > Globals.MaxMapSize) {
                MessageBox.Show(string.Format("Map file exceeds the maximum size of {0} bytes.", Globals.MaxMapSize), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.plugin.Dirty = false;
            this.filename = saveFilename;
            this.Text = string.Format("CnC TDRA Map Editor - {0}", this.filename);

            return true;
        }

        private void RefreshAvailableTools() {
            this.availableToolTypes = ToolType.None;
            if(this.plugin != null) {
                this.availableToolTypes |= ToolType.Waypoint;

                if(this.plugin.Map.TemplateTypes.Any())
                    this.availableToolTypes |= ToolType.Map;
                if(this.plugin.Map.SmudgeTypes.Any())
                    this.availableToolTypes |= ToolType.Smudge;
                if(this.plugin.Map.OverlayTypes.Any(t => t.IsPlaceable && ((t.Theaters == null) || t.Theaters.Contains(this.plugin.Map.Theater))))
                    this.availableToolTypes |= ToolType.Overlay;
                if(this.plugin.Map.TerrainTypes.Any(t => t.Theaters.Contains(this.plugin.Map.Theater)))
                    this.availableToolTypes |= ToolType.Terrain;
                if(this.plugin.Map.InfantryTypes.Any())
                    this.availableToolTypes |= ToolType.Infantry;
                if(this.plugin.Map.UnitTypes.Any())
                    this.availableToolTypes |= ToolType.Unit;
                if(this.plugin.Map.BuildingTypes.Any())
                    this.availableToolTypes |= ToolType.Building;
                if(this.plugin.Map.OverlayTypes.Any(t => t.IsResource))
                    this.availableToolTypes |= ToolType.Resources;
                if(this.plugin.Map.OverlayTypes.Any(t => t.IsWall))
                    this.availableToolTypes |= ToolType.Wall;
                if(this.plugin.Map.Triggers.Any())
                    this.availableToolTypes |= ToolType.CellTrigger;
            }

            this.mapToolStripButton.Enabled = (this.availableToolTypes & ToolType.Map) != ToolType.None;
            this.smudgeToolStripButton.Enabled = (this.availableToolTypes & ToolType.Smudge) != ToolType.None;
            this.overlayToolStripButton.Enabled = (this.availableToolTypes & ToolType.Overlay) != ToolType.None;
            this.terrainToolStripButton.Enabled = (this.availableToolTypes & ToolType.Terrain) != ToolType.None;
            this.infantryToolStripButton.Enabled = (this.availableToolTypes & ToolType.Infantry) != ToolType.None;
            this.unitToolStripButton.Enabled = (this.availableToolTypes & ToolType.Unit) != ToolType.None;
            this.buildingToolStripButton.Enabled = (this.availableToolTypes & ToolType.Building) != ToolType.None;
            this.resourcesToolStripButton.Enabled = (this.availableToolTypes & ToolType.Resources) != ToolType.None;
            this.wallsToolStripButton.Enabled = (this.availableToolTypes & ToolType.Wall) != ToolType.None;
            this.waypointsToolStripButton.Enabled = (this.availableToolTypes & ToolType.Waypoint) != ToolType.None;
            this.cellTriggersToolStripButton.Enabled = (this.availableToolTypes & ToolType.CellTrigger) != ToolType.None;

            this.ActiveToolType = this.activeToolType;
        }

        private void ClearActiveTool() {
            this.activeTool?.Dispose();
            this.activeTool = null;

            if(this.activeToolForm != null) {
                this.activeToolForm.ResizeEnd -= this.ActiveToolForm_ResizeEnd;
                this.activeToolForm.Close();
                this.activeToolForm = null;
            }

            this.toolStatusLabel.Text = string.Empty;
        }

        private void RefreshActiveTool() {
            if(this.plugin == null) {
                return;
            }

            if(this.activeTool == null) {
                this.activeLayers = MapLayerFlag.None;
            }

            this.ClearActiveTool();

            switch(this.ActiveToolType) {
            case ToolType.Map: {
                var toolDialog = this.templateToolControl;

                this.activeTool = new TemplateTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.TemplateTypeListView, toolDialog.TemplateTypeMapPanel, this.mouseToolTip, this.plugin, this.url);
                this.toolTabControl.SelectedTab = this.mapToolTabPage;
            }
            break;
            case ToolType.Smudge: {
                var toolDialog = this.smudgeToolControl;

                this.activeTool = new SmudgeTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.GenericTypeListView, toolDialog.GenericTypeMapPanel, this.plugin, this.url);
                this.toolTabControl.SelectedTab = this.smudgeToolTabPage;
            }
            break;
            case ToolType.Overlay: {
                var toolDialog = this.overlayToolControl;

                this.activeTool = new OverlaysTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.GenericTypeListView, toolDialog.GenericTypeMapPanel, this.plugin, this.url);
                this.toolTabControl.SelectedTab = this.overlayToolTabPage;
            }
            break;
            case ToolType.Resources: {
                var toolDialog = new ResourcesToolDialog();

                this.activeTool = new ResourcesTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.TotalResourcesLbl, toolDialog.ResourceBrushSizeNud, toolDialog.GemsCheckBox, this.plugin, this.url);
                this.activeToolForm = toolDialog;
                this.activeToolForm.Show(this);
            }
            break;
            case ToolType.Terrain: {
                var toolDialog = this.terrainToolControl;
                toolDialog.Initialize(this.plugin);
                this.activeTool = new TerrainTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.TerrainTypeListView, toolDialog.TerrainTypeMapPanel, toolDialog.TerrainProperties, this.plugin, this.url);
                this.toolTabControl.SelectedTab = this.terrainToolTabPage;
            }
            break;
            case ToolType.Infantry: {
                var toolDialog = new ObjectToolDialog(this.plugin) {
                    Text = "Infantry"
                };

                toolDialog.ObjectTypeComboBox.Types = this.plugin.Map.InfantryTypes.OrderBy(t => t.Name);

                this.activeTool = new InfantryTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.ObjectTypeComboBox, toolDialog.ObjectTypeMapPanel, toolDialog.ObjectProperties, this.plugin, this.url);
                this.activeToolForm = toolDialog;
                this.activeToolForm.Show(this);
            }
            break;
            case ToolType.Unit: {
                var toolDialog = new ObjectToolDialog(this.plugin) {
                    Text = "Units"
                };

                toolDialog.ObjectTypeComboBox.Types = this.plugin.Map.UnitTypes
                    .Where(t => !t.IsFixedWing)
                    .OrderBy(t => t.Name);

                this.activeTool = new UnitTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.ObjectTypeComboBox, toolDialog.ObjectTypeMapPanel, toolDialog.ObjectProperties, this.plugin, this.url);
                this.activeToolForm = toolDialog;
                this.activeToolForm.Show(this);
            }
            break;
            case ToolType.Building: {
                var toolDialog = new ObjectToolDialog(this.plugin) {
                    Text = "Structures"
                };

                toolDialog.ObjectTypeComboBox.Types = this.plugin.Map.BuildingTypes
                    .Where(t => (t.Theaters == null) || t.Theaters.Contains(this.plugin.Map.Theater))
                    .OrderBy(t => t.IsFake)
                    .ThenBy(t => t.Name);

                this.activeTool = new BuildingTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.ObjectTypeComboBox, toolDialog.ObjectTypeMapPanel, toolDialog.ObjectProperties, this.plugin, this.url);
                this.activeToolForm = toolDialog;
                this.activeToolForm.Show(this);
            }
            break;
            case ToolType.Wall: {
                var toolDialog = new GenericToolDialog {
                    Text = "Walls"
                };

                toolDialog.GenericTypeComboBox.Types = this.plugin.Map.OverlayTypes.Where(t => t.IsWall).OrderBy(t => t.Name);

                this.activeTool = new WallsTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.GenericTypeComboBox, toolDialog.GenericTypeMapPanel, this.plugin, this.url);
                this.activeToolForm = toolDialog;
                this.activeToolForm.Show(this);
            }
            break;
            case ToolType.Waypoint: {
                var toolDialog = new WaypointsToolDialog();

                toolDialog.WaypointCombo.DataSource = this.plugin.Map.Waypoints.Select(w => w.Name).ToArray();

                this.activeTool = new WaypointsTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.WaypointCombo, this.plugin, this.url);
                this.activeToolForm = toolDialog;
                this.activeToolForm.Show(this);
            }
            break;
            case ToolType.CellTrigger: {
                var toolDialog = new CellTriggersToolDialog();

                toolDialog.TriggerCombo.DataSource = this.plugin.Map.Triggers.Select(t => t.Name).ToArray();

                this.activeTool = new CellTriggersTool(this.mapPanel, this.ActiveLayers, this.toolStatusLabel, toolDialog.TriggerCombo, this.plugin, this.url);
                this.activeToolForm = toolDialog;
                this.activeToolForm.Show(this);
            }
            break;
            }

            if(this.activeToolForm != null) {
                this.activeToolForm.ResizeEnd += this.ActiveToolForm_ResizeEnd;
                this.clampActiveToolForm();
            }

            switch(this.plugin.GameType) {
            case GameType.TiberianDawn:
                this.mapPanel.MaxZoom = 8;
                this.mapPanel.ZoomStep = 1;
                break;
            case GameType.RedAlert:
                this.mapPanel.MaxZoom = 16;
                this.mapPanel.ZoomStep = 2;
                break;
            }

            this.mapToolStripButton.Checked = this.ActiveToolType == ToolType.Map;
            this.smudgeToolStripButton.Checked = this.ActiveToolType == ToolType.Smudge;
            this.overlayToolStripButton.Checked = this.ActiveToolType == ToolType.Overlay;
            this.terrainToolStripButton.Checked = this.ActiveToolType == ToolType.Terrain;
            this.infantryToolStripButton.Checked = this.ActiveToolType == ToolType.Infantry;
            this.unitToolStripButton.Checked = this.ActiveToolType == ToolType.Unit;
            this.buildingToolStripButton.Checked = this.ActiveToolType == ToolType.Building;
            this.resourcesToolStripButton.Checked = this.ActiveToolType == ToolType.Resources;
            this.wallsToolStripButton.Checked = this.ActiveToolType == ToolType.Wall;
            this.waypointsToolStripButton.Checked = this.ActiveToolType == ToolType.Waypoint;
            this.cellTriggersToolStripButton.Checked = this.ActiveToolType == ToolType.CellTrigger;

            this.Focus();

            this.UpdateVisibleLayers();
            this.mapPanel.Invalidate();
        }

        private void clampActiveToolForm() {
            if(this.activeToolForm == null) {
                return;
            }

            var bounds = this.activeToolForm.DesktopBounds;
            var workingArea = Screen.FromControl(this).WorkingArea;
            if(bounds.Right > workingArea.Right) {
                bounds.X = workingArea.Right - bounds.Width;
            }
            if(bounds.X < workingArea.Left) {
                bounds.X = workingArea.Left;
            }
            if(bounds.Bottom > workingArea.Bottom) {
                bounds.Y = workingArea.Bottom - bounds.Height;
            }
            if(bounds.Y < workingArea.Top) {
                bounds.Y = workingArea.Top;
            }
            this.activeToolForm.DesktopBounds = bounds;
        }

        private void ActiveToolForm_ResizeEnd(object sender, EventArgs e) {
            this.clampActiveToolForm();
        }

        private void Triggers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            this.RefreshAvailableTools();
        }

        private void mainToolStripButton_Click(object sender, EventArgs e) {
            if(this.plugin == null) {
                return;
            }

            if(sender == this.mapToolStripButton) {
                this.ActiveToolType = ToolType.Map;
            } else if(sender == this.smudgeToolStripButton) {
                this.ActiveToolType = ToolType.Smudge;
            } else if(sender == this.overlayToolStripButton) {
                this.ActiveToolType = ToolType.Overlay;
            } else if(sender == this.terrainToolStripButton) {
                this.ActiveToolType = ToolType.Terrain;
            } else if(sender == this.infantryToolStripButton) {
                this.ActiveToolType = ToolType.Infantry;
            } else if(sender == this.unitToolStripButton) {
                this.ActiveToolType = ToolType.Unit;
            } else if(sender == this.buildingToolStripButton) {
                this.ActiveToolType = ToolType.Building;
            } else if(sender == this.resourcesToolStripButton) {
                this.ActiveToolType = ToolType.Resources;
            } else if(sender == this.wallsToolStripButton) {
                this.ActiveToolType = ToolType.Wall;
            } else if(sender == this.waypointsToolStripButton) {
                this.ActiveToolType = ToolType.Waypoint;
            } else if(sender == this.cellTriggersToolStripButton) {
                this.ActiveToolType = ToolType.CellTrigger;
            }
        }

        private void UpdateVisibleLayers() {
            var layers = MapLayerFlag.All;
            if(!this.viewLayersBoundariesMenuItem.Checked) {
                layers &= ~MapLayerFlag.Boundaries;
            }
            if(!this.viewLayersOverlayMenuItem.Checked) {
                layers &= ~MapLayerFlag.OverlayAll;
            }
            if(!this.viewLayersTerrainMenuItem.Checked) {
                layers &= ~MapLayerFlag.Terrain;
            }
            if(!this.viewLayersWaypointsMenuItem.Checked) {
                layers &= ~MapLayerFlag.Waypoints;
            }
            if(!this.viewLayersCellTriggersMenuItem.Checked) {
                layers &= ~MapLayerFlag.CellTriggers;
            }
            if(!this.viewLayersObjectTriggersMenuItem.Checked) {
                layers &= ~MapLayerFlag.TechnoTriggers;
            }
            this.ActiveLayers = layers;
        }

        private void viewLayersMenuItem_CheckedChanged(object sender, EventArgs e) {
            this.UpdateVisibleLayers();
        }

        private void toolTabControl_Selected(object sender, TabControlEventArgs e) {
            if(this.plugin == null) {
                return;
            }
        }

        private void developerGenerateMapPreviewMenuItem_Click(object sender, EventArgs e) {
#if DEVELOPER
            if((this.plugin == null) || string.IsNullOrEmpty(this.filename)) {
                return;
            }

            this.plugin.Map.GenerateMapPreview().Save(Path.ChangeExtension(this.filename, ".tga"));
#endif
        }

        private void developerGoToINIMenuItem_Click(object sender, EventArgs e) {
#if DEVELOPER
            if((this.plugin == null) || string.IsNullOrEmpty(this.filename)) {
                return;
            }

            var path = Path.ChangeExtension(this.filename, ".mpr");
            if(!File.Exists(path)) {
                path = Path.ChangeExtension(this.filename, ".ini");
            }

            try {
                Process.Start(path);
            } catch(Win32Exception) {
                Process.Start("notepad.exe", path);
            } catch(Exception) { }
#endif
        }

        private void developerGenerateMapPreviewDirectoryMenuItem_Click(object sender, EventArgs e) {
#if DEVELOPER
            var fbd = new FolderBrowserDialog {
                ShowNewFolderButton = false
            };
            if(fbd.ShowDialog() == DialogResult.OK) {
                var extensions = new string[] { ".ini", ".mpr" };
                foreach(var file in Directory.EnumerateFiles(fbd.SelectedPath).Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))) {
                    var gameType = GameType.None;

                    var ini = new INI();
                    using(var reader = new StreamReader(file)) {
                        ini.Parse(reader);
                    }
                    gameType = ini.Sections.Contains("MapPack") ? GameType.RedAlert : GameType.TiberianDawn;

                    if(gameType == GameType.None) {
                        continue;
                    }

                    IGamePlugin plugin = null;
                    switch(gameType) {
                    case GameType.TiberianDawn: {
                        plugin = new TiberianDawn.GamePlugin(false);
                    }
                    break;
                    case GameType.RedAlert: {
                        plugin = new RedAlert.GamePlugin(false);
                    }
                    break;
                    }

                    plugin.Load(file, FileType.INI);
                    plugin.Map.GenerateMapPreview().Save(Path.ChangeExtension(file, ".tga"));
                    plugin.Dispose();
                }
            }
#endif
        }

        private void developerDebugShowOverlapCellsMenuItem_CheckedChanged(object sender, EventArgs e) =>
#if DEVELOPER
            Globals.Developer.ShowOverlapCells = this.developerDebugShowOverlapCellsMenuItem.Checked;
#endif

        private void filePublishMenuItem_Click(object sender, EventArgs e) {
            if(this.plugin == null) {
                return;
            }

            if(!this.PromptSaveMap()) {
                return;
            }

            if(this.plugin.Dirty) {
                MessageBox.Show("Map must be saved before publishing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(new FileInfo(this.filename).Length > Globals.MaxMapSize) {
                return;
            }

            using(var sd = new SteamDialog(this.plugin)) {
                sd.ShowDialog();
            }

            this.fileSaveMenuItem.PerformClick();
        }

        private void mainToolStrip_MouseMove(object sender, MouseEventArgs e) {
            this.mainToolStrip.Focus();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = !this.PromptSaveMap();
        }

        private bool PromptSaveMap() {
            var cancel = false;
            if(this.plugin?.Dirty ?? false) {
                var message = string.IsNullOrEmpty(this.filename) ? "Save new map?" : string.Format("Save map '{0}'?", this.filename);
                var result = MessageBox.Show(message, "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch(result) {
                case DialogResult.Yes: {
                    if(string.IsNullOrEmpty(this.filename)) {
                        this.fileSaveAsMenuItem.PerformClick();
                    } else {
                        this.fileSaveMenuItem.PerformClick();
                    }
                }
                break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    cancel = true;
                    break;
                }
            }
            return !cancel;
        }
    }
}
