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
using MobiusEditor.Interface;
using MobiusEditor.Render;
using MobiusEditor.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using TGASharpLib;

namespace MobiusEditor.Model {
    [Flags]
    public enum MapLayerFlag {
        None = 0,
        Basic = 1 << 0,
        Map = 1 << 1,
        Template = 1 << 2,
        Terrain = 1 << 3,
        Resources = 1 << 4,
        Walls = 1 << 5,
        Overlay = 1 << 6,
        Smudge = 1 << 7,
        Waypoints = 1 << 8,
        CellTriggers = 1 << 9,
        Houses = 1 << 10,
        Infantry = 1 << 11,
        Units = 1 << 12,
        Buildings = 1 << 13,
        Boundaries = 1 << 14,
        TechnoTriggers = 1 << 15,

        OverlayAll = Resources | Walls | Overlay,
        Technos = Terrain | Walls | Infantry | Units | Buildings,

        All = int.MaxValue
    }

    public class MapContext : ITypeDescriptorContext {
        public IContainer Container {
            get; private set;
        }

        public object Instance {
            get; private set;
        }

        public PropertyDescriptor PropertyDescriptor {
            get; private set;
        }

        public Map Map => this.Instance as Map;

        public readonly bool FractionalPercentages;

        public MapContext(Map map, bool fractionalPercentages) {
            this.Instance = map;
            this.FractionalPercentages = fractionalPercentages;
        }

        public object GetService(Type serviceType) => null;

        public void OnComponentChanged() {
        }

        public bool OnComponentChanging() => true;
    }

    public class Map : ICloneable {
        private int updateCount = 0;
        private bool updating = false;
        private readonly IDictionary<MapLayerFlag, ISet<Point>> invalidateLayers = new Dictionary<MapLayerFlag, ISet<Point>>();
        private bool invalidateOverlappers;

        public readonly BasicSection BasicSection;

        public readonly MapSection MapSection = new MapSection();

        public readonly BriefingSection BriefingSection = new BriefingSection();

        public readonly SteamSection SteamSection = new SteamSection();

        public TheaterType Theater {
            get => this.MapSection.Theater; set => this.MapSection.Theater = value;
        }

        public Point TopLeft {
            get => new Point(this.MapSection.X, this.MapSection.Y);
            set {
                this.MapSection.X = value.X;
                this.MapSection.Y = value.Y;
            }
        }

        public Size Size {
            get => new Size(this.MapSection.Width, this.MapSection.Height);
            set {
                this.MapSection.Width = value.Width;
                this.MapSection.Height = value.Height;
            }
        }

        public Rectangle Bounds {
            get => new Rectangle(this.TopLeft, this.Size);
            set {
                this.MapSection.X = value.Left;
                this.MapSection.Y = value.Top;
                this.MapSection.Width = value.Width;
                this.MapSection.Height = value.Height;
            }
        }

        public readonly Type HouseType;

        public readonly HouseType[] HouseTypes;

        public readonly List<TheaterType> TheaterTypes;

        public readonly List<TemplateType> TemplateTypes;

        public readonly List<TerrainType> TerrainTypes;

        public readonly List<OverlayType> OverlayTypes;

        public readonly List<SmudgeType> SmudgeTypes;

        public readonly string[] EventTypes;

        public readonly string[] ActionTypes;

        public readonly string[] MissionTypes;

        public readonly List<DirectionType> DirectionTypes;

        public readonly List<InfantryType> InfantryTypes;

        public readonly List<UnitType> UnitTypes;

        public readonly List<BuildingType> BuildingTypes;

        public readonly string[] TeamMissionTypes;

        public readonly CellMetrics Metrics;

        public readonly CellGrid<Template> Templates;

        public readonly CellGrid<Overlay> Overlay;

        public readonly CellGrid<Smudge> Smudge;

        public readonly OccupierSet<ICellOccupier> Technos;

        public readonly OccupierSet<ICellOccupier> Buildings;

        public readonly OverlapperSet<ICellOverlapper> Overlappers;

        public readonly Waypoint[] Waypoints;

        public readonly CellGrid<CellTrigger> CellTriggers;

        public readonly ObservableCollection<Trigger> Triggers;

        public readonly List<TeamType> TeamTypes;

        public House[] Houses;

        public readonly List<string> MovieTypes;

        public int TiberiumOrGoldValue {
            get; set;
        }

        public int GemValue {
            get; set;
        }

        public int TotalResources {
            get {
                var totalResources = 0;
                foreach(var (cell, value) in this.Overlay) {
                    if(value.Type.IsResource) {
                        totalResources += (value.Icon + 1) * (value.Type.IsGem ? this.GemValue : this.TiberiumOrGoldValue);
                    }
                }
                return totalResources;
            }
        }

        public Map(BasicSection basicSection, TheaterType theater, Size cellSize, Type houseType,
            IEnumerable<HouseType> houseTypes, IEnumerable<TheaterType> theaterTypes, IEnumerable<TemplateType> templateTypes,
            IEnumerable<TerrainType> terrainTypes, IEnumerable<OverlayType> overlayTypes, IEnumerable<SmudgeType> smudgeTypes,
            IEnumerable<string> eventTypes, IEnumerable<string> actionTypes, IEnumerable<string> missionTypes,
            IEnumerable<DirectionType> directionTypes, IEnumerable<InfantryType> infantryTypes, IEnumerable<UnitType> unitTypes,
            IEnumerable<BuildingType> buildingTypes, IEnumerable<string> teamMissionTypes, IEnumerable<Waypoint> waypoints,
            IEnumerable<string> movieTypes) {
            this.BasicSection = basicSection;

            this.HouseType = houseType;
            this.HouseTypes = houseTypes.ToArray();
            this.TheaterTypes = new List<TheaterType>(theaterTypes);
            this.TemplateTypes = new List<TemplateType>(templateTypes);
            this.TerrainTypes = new List<TerrainType>(terrainTypes);
            this.OverlayTypes = new List<OverlayType>(overlayTypes);
            this.SmudgeTypes = new List<SmudgeType>(smudgeTypes);
            this.EventTypes = eventTypes.ToArray();
            this.ActionTypes = actionTypes.ToArray();
            this.MissionTypes = missionTypes.ToArray();
            this.DirectionTypes = new List<DirectionType>(directionTypes);
            this.InfantryTypes = new List<InfantryType>(infantryTypes);
            this.UnitTypes = new List<UnitType>(unitTypes);
            this.BuildingTypes = new List<BuildingType>(buildingTypes);
            this.TeamMissionTypes = teamMissionTypes.ToArray();
            this.MovieTypes = new List<string>(movieTypes);

            this.Metrics = new CellMetrics(cellSize);
            this.Templates = new CellGrid<Template>(this.Metrics);
            this.Overlay = new CellGrid<Overlay>(this.Metrics);
            this.Smudge = new CellGrid<Smudge>(this.Metrics);
            this.Technos = new OccupierSet<ICellOccupier>(this.Metrics);
            this.Buildings = new OccupierSet<ICellOccupier>(this.Metrics);
            this.Overlappers = new OverlapperSet<ICellOverlapper>(this.Metrics);
            this.Triggers = new ObservableCollection<Trigger>();
            this.TeamTypes = new List<TeamType>();
            this.Houses = this.HouseTypes.Select(t => { var h = (House)Activator.CreateInstance(this.HouseType, t); h.SetDefault(); return h; }).ToArray();
            this.Waypoints = waypoints.ToArray();
            this.CellTriggers = new CellGrid<CellTrigger>(this.Metrics);

            this.MapSection.SetDefault();
            this.BriefingSection.SetDefault();
            this.SteamSection.SetDefault();
            this.Templates.Clear();
            this.Overlay.Clear();
            this.Smudge.Clear();
            this.Technos.Clear();
            this.Overlappers.Clear();
            this.CellTriggers.Clear();

            this.TopLeft = new Point(1, 1);
            this.Size = this.Metrics.Size - new Size(2, 2);
            this.Theater = theater;

            this.Overlay.CellChanged += this.Overlay_CellChanged;
            this.Technos.OccupierAdded += this.Technos_OccupierAdded;
            this.Technos.OccupierRemoved += this.Technos_OccupierRemoved;
            this.Buildings.OccupierAdded += this.Buildings_OccupierAdded;
            this.Buildings.OccupierRemoved += this.Buildings_OccupierRemoved;
            this.Triggers.CollectionChanged += this.Triggers_CollectionChanged;
        }

        public void BeginUpdate() => this.updateCount++;

        public void EndUpdate() {
            if(--this.updateCount == 0) {
                this.Update();
            }
        }

        public void InitTheater(GameType gameType) {
            foreach(var templateType in this.TemplateTypes) {
                templateType.Init(this.Theater);
            }

            foreach(var smudgeType in this.SmudgeTypes) {
                smudgeType.Init(this.Theater);
            }

            foreach(var overlayType in this.OverlayTypes) {
                overlayType.Init(this.Theater);
            }

            foreach(var terrainType in this.TerrainTypes) {
                terrainType.Init(this.Theater);
            }

            foreach(var infantryType in this.InfantryTypes) {
                infantryType.Init(gameType, this.Theater, this.HouseTypes.Where(h => h.Equals(infantryType.OwnerHouse)).FirstOrDefault(), this.DirectionTypes.Where(d => d.Facing == FacingType.South).First());
            }

            foreach(var unitType in this.UnitTypes) {
                unitType.Init(gameType, this.Theater, this.HouseTypes.Where(h => h.Equals(unitType.OwnerHouse)).FirstOrDefault(), this.DirectionTypes.Where(d => d.Facing == FacingType.North).First());
            }

            foreach(var buildingType in this.BuildingTypes) {
                buildingType.Init(gameType, this.Theater, this.HouseTypes.Where(h => h.Equals(buildingType.OwnerHouse)).FirstOrDefault(), this.DirectionTypes.Where(d => d.Facing == FacingType.North).First());
            }
        }

        private void Update() {
            this.updating = true;

            if(this.invalidateLayers.TryGetValue(MapLayerFlag.Resources, out var locations)) {
                this.UpdateResourceOverlays(locations);
            }

            if(this.invalidateLayers.TryGetValue(MapLayerFlag.Walls, out locations)) {
                this.UpdateWallOverlays(locations);
            }

            if(this.invalidateOverlappers) {
                this.Overlappers.Clear();
                foreach(var (location, techno) in this.Technos) {
                    if(techno is ICellOverlapper) {
                        this.Overlappers.Add(location, techno as ICellOverlapper);
                    }
                }
            }

            this.invalidateLayers.Clear();
            this.invalidateOverlappers = false;
            this.updating = false;
        }

        private void UpdateResourceOverlays(ISet<Point> locations) {
            var tiberiumCounts = new int[] { 0, 1, 3, 4, 6, 7, 8, 10, 11 };
            var gemCounts = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2 };

            foreach(var (cell, overlay) in this.Overlay.IntersectsWith(locations).Where(o => o.Value.Type.IsResource)) {
                var count = 0;
                foreach(var facing in CellMetrics.AdjacentFacings) {
                    var adjacentTiberium = this.Overlay.Adjacent(cell, facing);
                    if(adjacentTiberium?.Type.IsResource ?? false) {
                        count++;
                    }
                }

                overlay.Icon = overlay.Type.IsGem ? gemCounts[count] : tiberiumCounts[count];
            }
        }

        private void UpdateWallOverlays(ISet<Point> locations) {
            foreach(var (cell, overlay) in this.Overlay.IntersectsWith(locations).Where(o => o.Value.Type.IsWall)) {
                var northWall = this.Overlay.Adjacent(cell, FacingType.North);
                var eastWall = this.Overlay.Adjacent(cell, FacingType.East);
                var southWall = this.Overlay.Adjacent(cell, FacingType.South);
                var westWall = this.Overlay.Adjacent(cell, FacingType.West);

                var icon = 0;
                if(northWall?.Type == overlay.Type) {
                    icon |= 1;
                }
                if(eastWall?.Type == overlay.Type) {
                    icon |= 2;
                }
                if(southWall?.Type == overlay.Type) {
                    icon |= 4;
                }
                if(westWall?.Type == overlay.Type) {
                    icon |= 8;
                }

                overlay.Icon = icon;
            }
        }

        private void RemoveBibs(Building building) {
            var bibCells = this.Smudge.IntersectsWith(building.BibCells).Where(x => (x.Value.Type.Flag & SmudgeTypeFlag.Bib) != SmudgeTypeFlag.None).Select(x => x.Cell).ToArray();
            foreach(var cell in bibCells) {
                this.Smudge[cell] = null;
            }
            building.BibCells.Clear();
        }

        private void AddBibs(Point location, Building building) {
            if(!building.Type.HasBib) {
                return;
            }

            var bib1Type = this.SmudgeTypes.Where(t => t.Flag == SmudgeTypeFlag.Bib1).FirstOrDefault();
            var bib2Type = this.SmudgeTypes.Where(t => t.Flag == SmudgeTypeFlag.Bib2).FirstOrDefault();
            var bib3Type = this.SmudgeTypes.Where(t => t.Flag == SmudgeTypeFlag.Bib3).FirstOrDefault();

            SmudgeType bibType = null;
            switch(building.Type.Size.Width) {
            case 2:
                bibType = bib3Type;
                break;
            case 3:
                bibType = bib2Type;
                break;
            case 4:
                bibType = bib1Type;
                break;
            }
            if(bibType != null) {
                var icon = 0;
                for(var y = 0; y < bibType.Size.Height; ++y) {
                    for(var x = 0; x < bibType.Size.Width; ++x, ++icon) {
                        if(this.Metrics.GetCell(new Point(location.X + x, location.Y + building.Type.Size.Height + y - 1), out var subCell)) {
                            this.Smudge[subCell] = new Smudge {
                                Type = bibType,
                                Icon = icon,
                                Data = 0,
                                Tint = building.Tint
                            };
                            building.BibCells.Add(subCell);
                        }
                    }
                }
            }
        }

        public Map Clone() {
            var map = new Map(this.BasicSection, this.Theater, this.Metrics.Size, this.HouseType,
                this.HouseTypes, this.TheaterTypes, this.TemplateTypes, this.TerrainTypes, this.OverlayTypes, this.SmudgeTypes,
                this.EventTypes, this.ActionTypes, this.MissionTypes, this.DirectionTypes, this.InfantryTypes, this.UnitTypes,
                this.BuildingTypes, this.TeamMissionTypes, this.Waypoints, this.MovieTypes) {
                TopLeft = TopLeft,
                Size = Size
            };

            map.BeginUpdate();

            this.MapSection.CopyTo(map.MapSection);
            this.BriefingSection.CopyTo(map.BriefingSection);
            this.SteamSection.CopyTo(map.SteamSection);
            this.Templates.CopyTo(map.Templates);
            this.Overlay.CopyTo(map.Overlay);
            this.Smudge.CopyTo(map.Smudge);
            this.CellTriggers.CopyTo(map.CellTriggers);
            Array.Copy(this.Houses, map.Houses, map.Houses.Length);

            foreach(var trigger in this.Triggers) {
                map.Triggers.Add(trigger);
            }

            foreach(var (location, occupier) in this.Technos) {
                if(occupier is InfantryGroup infantryGroup) {
                    var newInfantryGroup = new InfantryGroup();
                    Array.Copy(infantryGroup.Infantry, newInfantryGroup.Infantry, newInfantryGroup.Infantry.Length);
                    map.Technos.Add(location, newInfantryGroup);
                } else if(!(occupier is Building)) {
                    map.Technos.Add(location, occupier);
                }
            }

            foreach(var (location, building) in this.Buildings) {
                map.Buildings.Add(location, building);
            }

            map.TeamTypes.AddRange(this.TeamTypes);

            map.EndUpdate();

            return map;
        }

        public TGA GeneratePreview(Size previewSize, bool sharpen) {
            var mapBounds = new Rectangle(
                this.Bounds.Left * Globals.OriginalTileWidth,
                this.Bounds.Top * Globals.OriginalTileHeight,
                this.Bounds.Width * Globals.OriginalTileWidth,
                this.Bounds.Height * Globals.OriginalTileHeight
            );
            var previewScale = Math.Min(previewSize.Width / (float)mapBounds.Width, previewSize.Height / (float)mapBounds.Height);
            var scaledSize = new Size((int)(previewSize.Width / previewScale), (int)(previewSize.Height / previewScale));

            using(var fullBitmap = new Bitmap(this.Metrics.Width * Globals.OriginalTileWidth, this.Metrics.Height * Globals.OriginalTileHeight))
            using(var croppedBitmap = new Bitmap(previewSize.Width, previewSize.Height)) {
                var locations = this.Bounds.Points().ToHashSet();
                using(var g = Graphics.FromImage(fullBitmap)) {
                    MapRenderer.Render(GameType.None, this, g, locations, MapLayerFlag.Template | MapLayerFlag.Resources, 1);
                }

                using(var g = Graphics.FromImage(croppedBitmap)) {
                    var transform = new Matrix();
                    transform.Scale(previewScale, previewScale);
                    transform.Translate((scaledSize.Width - mapBounds.Width) / 2, (scaledSize.Height - mapBounds.Height) / 2);

                    g.Transform = transform;
                    g.Clear(Color.Black);
                    g.DrawImage(fullBitmap, new Rectangle(0, 0, mapBounds.Width, mapBounds.Height), mapBounds, GraphicsUnit.Pixel);
                }

                fullBitmap.Dispose();

                if(sharpen) {
                    using(var sharpenedImage = croppedBitmap.Sharpen(1.0f)) {
                        croppedBitmap.Dispose();
                        return TGA.FromBitmap(sharpenedImage);
                    }
                } else {
                    return TGA.FromBitmap(croppedBitmap);
                }
            }
        }

        public TGA GenerateMapPreview() => this.GeneratePreview(Globals.MapPreviewSize, false);

        public TGA GenerateWorkshopPreview() => this.GeneratePreview(Globals.WorkshopPreviewSize, true);

        object ICloneable.Clone() => this.Clone();

        private void Overlay_CellChanged(object sender, CellChangedEventArgs<Overlay> e) {
            if(e.OldValue?.Type.IsWall ?? false) {
                this.Buildings.Remove(e.OldValue);
            }

            if(e.Value?.Type.IsWall ?? false) {
                this.Buildings.Add(e.Location, e.Value);
            }

            if(this.updating) {
                return;
            }

            foreach(var overlay in new Overlay[] { e.OldValue, e.Value }) {
                if(overlay == null) {
                    continue;
                }

                var layer = MapLayerFlag.None;
                if(overlay.Type.IsResource) {
                    layer = MapLayerFlag.Resources;
                } else if(overlay.Type.IsWall) {
                    layer = MapLayerFlag.Walls;
                } else {
                    continue;
                }

                if(!this.invalidateLayers.TryGetValue(layer, out var locations)) {
                    locations = new HashSet<Point>();
                    this.invalidateLayers[layer] = locations;
                }

                locations.UnionWith(Rectangle.Inflate(new Rectangle(e.Location, new Size(1, 1)), 1, 1).Points());
            }

            if(this.updateCount == 0) {
                this.Update();
            }
        }

        private void Technos_OccupierAdded(object sender, OccupierAddedEventArgs<ICellOccupier> e) {
            if(e.Occupier is ICellOverlapper overlapper) {
                if(this.updateCount == 0) {
                    this.Overlappers.Add(e.Location, overlapper);
                } else {
                    this.invalidateOverlappers = true;
                }
            }
        }

        private void Technos_OccupierRemoved(object sender, OccupierRemovedEventArgs<ICellOccupier> e) {
            if(e.Occupier is ICellOverlapper overlapper) {
                if(this.updateCount == 0) {
                    this.Overlappers.Remove(overlapper);
                } else {
                    this.invalidateOverlappers = true;
                }
            }
        }

        private void Buildings_OccupierAdded(object sender, OccupierAddedEventArgs<ICellOccupier> e) {
            if(e.Occupier is Building building) {
                this.Technos.Add(e.Location, e.Occupier, building.Type.BaseOccupyMask);
                this.AddBibs(e.Location, building);
            } else {
                this.Technos.Add(e.Location, e.Occupier);
            }
        }

        private void Buildings_OccupierRemoved(object sender, OccupierRemovedEventArgs<ICellOccupier> e) {
            if(e.Occupier is Building building) {
                this.RemoveBibs(building);
            }

            this.Technos.Remove(e.Occupier);
        }

        private void Triggers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            foreach(var (_, building) in this.Buildings.OfType<Building>()) {
                if(!string.IsNullOrEmpty(building.Trigger)) {
                    if(this.Triggers.Where(t => building.Trigger.Equals(t.Name)).FirstOrDefault() == null) {
                        building.Trigger = Trigger.None;
                    }
                }
            }
        }
    }
}
