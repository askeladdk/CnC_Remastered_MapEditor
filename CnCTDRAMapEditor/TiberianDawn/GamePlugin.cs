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
using MobiusEditor.Model;
using MobiusEditor.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MobiusEditor.TiberianDawn {
    public class GamePlugin : IGamePlugin {
        private static readonly Regex MovieRegex = new Regex(@"^(.*?\\)*(.*?)\.BK2$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly IEnumerable<ITechnoType> technoTypes;

        private readonly IEnumerable<string> movieTypes;

        public GameType GameType => GameType.TiberianDawn;

        public Map Map {
            get;
        }

        public Image MapImage {
            get; private set;
        }

        public bool Dirty {
            get; set;
        }

        private INISectionCollection extraSections;

        static GamePlugin() => technoTypes = InfantryTypes.GetTypes().Cast<ITechnoType>().Concat(UnitTypes.GetTypes().Cast<ITechnoType>());

        public GamePlugin(bool mapImage) {
            var playerWaypoints = Enumerable.Range(0, 8).Select(i => new Waypoint(string.Format("P{0}", i), WaypointFlag.PlayerStart));
            var generalWaypoints = Enumerable.Range(8, 17).Select(i => new Waypoint(i.ToString()));
            var specialWaypoints = new Waypoint[] { new Waypoint("Flare"), new Waypoint("Home"), new Waypoint("Reinf.") };
            var waypoints = playerWaypoints.Concat(generalWaypoints).Concat(specialWaypoints);

            var movies = new List<string>(new string[] { "x" });
            using(var megafile = new Megafile(Path.Combine(Globals.MegafilePath, "MOVIES_TD.MEG"))) {
                foreach(var filename in megafile) {
                    var m = MovieRegex.Match(filename);
                    if(m.Success) {
                        movies.Add(m.Groups[m.Groups.Count - 1].ToString());
                    }
                }
            }
            this.movieTypes = movies.ToArray();

            var basicSection = new BasicSection();
            basicSection.SetDefault();

            var houseTypes = HouseTypes.GetTypes();
            basicSection.Player = houseTypes.First().Name;

            this.Map = new Map(basicSection, null, Constants.MaxSize, typeof(House),
                houseTypes, TheaterTypes.GetTypes(), TemplateTypes.GetTypes(), TerrainTypes.GetTypes(),
                OverlayTypes.GetTypes(), SmudgeTypes.GetTypes(), EventTypes.GetTypes(), ActionTypes.GetTypes(),
                MissionTypes.GetTypes(), DirectionTypes.GetTypes(), InfantryTypes.GetTypes(), UnitTypes.GetTypes(),
                BuildingTypes.GetTypes(), TeamMissionTypes.GetTypes(), waypoints, this.movieTypes) {
                TiberiumOrGoldValue = 25
            };

            this.Map.BasicSection.PropertyChanged += this.BasicSection_PropertyChanged;
            this.Map.MapSection.PropertyChanged += this.MapSection_PropertyChanged;

            if(mapImage) {
                this.MapImage = new Bitmap(this.Map.Metrics.Width * Globals.TileWidth, this.Map.Metrics.Height * Globals.TileHeight);
            }
        }

        public GamePlugin()
            : this(true) {
        }

        public void New(string theater) {
            this.Map.Theater = this.Map.TheaterTypes.Where(t => t.Equals(theater)).FirstOrDefault() ?? TheaterTypes.Temperate;
            this.Map.TopLeft = new Point(1, 1);
            this.Map.Size = this.Map.Metrics.Size - new Size(2, 2);

            this.UpdateBasePlayerHouse();

            this.Dirty = true;
        }

        public IEnumerable<string> Load(string path, FileType fileType) {
            var errors = new List<string>();
            switch(fileType) {
            case FileType.INI:
            case FileType.BIN: {
                var iniPath = Path.ChangeExtension(path, ".ini");
                var binPath = Path.ChangeExtension(path, ".bin");
                var ini = new INI();
                using(var iniReader = new StreamReader(iniPath))
                using(var binReader = new BinaryReader(new FileStream(binPath, FileMode.Open, FileAccess.Read))) {
                    ini.Parse(iniReader);
                    errors.AddRange(this.LoadINI(ini));
                    this.LoadBinary(binReader);
                }
            }
            break;
            case FileType.MEG:
            case FileType.PGM: {
                using(var megafile = new Megafile(path)) {
                    var iniFile = megafile.Where(p => Path.GetExtension(p).ToLower() == ".ini").FirstOrDefault();
                    var binFile = megafile.Where(p => Path.GetExtension(p).ToLower() == ".bin").FirstOrDefault();
                    if((iniFile != null) && (binFile != null)) {
                        var ini = new INI();
                        using(var iniReader = new StreamReader(megafile.Open(iniFile)))
                        using(var binReader = new BinaryReader(megafile.Open(binFile))) {
                            ini.Parse(iniReader);
                            errors.AddRange(this.LoadINI(ini));
                            this.LoadBinary(binReader);
                        }
                    }
                }
            }
            break;
            default:
                throw new NotSupportedException();
            }
            return errors;
        }

        private IEnumerable<string> LoadINI(INI ini) {
            var errors = new List<string>();

            this.Map.BeginUpdate();

            var basicSection = ini.Sections.Extract("Basic");
            if(basicSection != null) {
                INI.ParseSection(new MapContext(this.Map, false), basicSection, this.Map.BasicSection);
            }

            this.Map.BasicSection.Player = this.Map.HouseTypes.Where(t => t.Equals(this.Map.BasicSection.Player)).FirstOrDefault()?.Name ?? this.Map.HouseTypes.First().Name;

            var mapSection = ini.Sections.Extract("Map");
            if(mapSection != null) {
                INI.ParseSection(new MapContext(this.Map, false), mapSection, this.Map.MapSection);
            }

            var briefingSection = ini.Sections.Extract("Briefing");
            if(briefingSection != null) {
                if(briefingSection.Keys.Contains("Text")) {
                    this.Map.BriefingSection.Briefing = briefingSection["Text"].Replace("@", Environment.NewLine);
                } else {
                    this.Map.BriefingSection.Briefing = string.Join(" ", briefingSection.Keys.Select(k => k.Value)).Replace("@", Environment.NewLine);
                }
            }

            var steamSection = ini.Sections.Extract("Steam");
            if(steamSection != null) {
                INI.ParseSection(new MapContext(this.Map, false), steamSection, this.Map.SteamSection);
            }

            var teamTypesSection = ini.Sections.Extract("TeamTypes");
            if(teamTypesSection != null) {
                foreach(var (Key, Value) in teamTypesSection) {
                    try {
                        var teamType = new TeamType { Name = Key };

                        var tokens = Value.Split(',').ToList();
                        teamType.House = this.Map.HouseTypes.Where(t => t.Equals(tokens[0])).FirstOrDefault();
                        tokens.RemoveAt(0);
                        teamType.IsRoundAbout = int.Parse(tokens[0]) != 0;
                        tokens.RemoveAt(0);
                        teamType.IsLearning = int.Parse(tokens[0]) != 0;
                        tokens.RemoveAt(0);
                        teamType.IsSuicide = int.Parse(tokens[0]) != 0;
                        tokens.RemoveAt(0);
                        teamType.IsAutocreate = int.Parse(tokens[0]) != 0;
                        tokens.RemoveAt(0);
                        teamType.IsMercenary = int.Parse(tokens[0]) != 0;
                        tokens.RemoveAt(0);
                        teamType.RecruitPriority = int.Parse(tokens[0]);
                        tokens.RemoveAt(0);
                        teamType.MaxAllowed = byte.Parse(tokens[0]);
                        tokens.RemoveAt(0);
                        teamType.InitNum = byte.Parse(tokens[0]);
                        tokens.RemoveAt(0);
                        teamType.Fear = byte.Parse(tokens[0]);
                        tokens.RemoveAt(0);

                        var numClasses = int.Parse(tokens[0]);
                        tokens.RemoveAt(0);
                        for(var i = 0; i < Math.Min(Globals.MaxTeamClasses, numClasses); ++i) {
                            var classTokens = tokens[0].Split(':');
                            tokens.RemoveAt(0);
                            if(classTokens.Length == 2) {
                                var type = technoTypes.Where(t => t.Equals(classTokens[0])).FirstOrDefault();
                                var count = byte.Parse(classTokens[1]);
                                if(type != null) {
                                    teamType.Classes.Add(new TeamTypeClass { Type = type, Count = count });
                                } else {
                                    errors.Add(string.Format("Team '{0}' references unknown class '{1}'", Key, classTokens[0]));
                                }
                            } else {
                                errors.Add(string.Format("Team '{0}' has wrong number of tokens for class index {1} (expecting 2)", Key, i));
                            }
                        }

                        var numMissions = int.Parse(tokens[0]);
                        tokens.RemoveAt(0);
                        for(var i = 0; i < Math.Min(Globals.MaxTeamMissions, numMissions); ++i) {
                            var missionTokens = tokens[0].Split(':');
                            tokens.RemoveAt(0);
                            if(missionTokens.Length == 2) {
                                teamType.Missions.Add(new TeamTypeMission { Mission = missionTokens[0], Argument = int.Parse(missionTokens[1]) });
                            } else {
                                errors.Add(string.Format("Team '{0}' has wrong number of tokens for mission index {1} (expecting 2)", Key, i));
                            }
                        }

                        if(tokens.Count > 0) {
                            teamType.IsReinforcable = int.Parse(tokens[0]) != 0;
                            tokens.RemoveAt(0);
                        }

                        if(tokens.Count > 0) {
                            teamType.IsPrebuilt = int.Parse(tokens[0]) != 0;
                            tokens.RemoveAt(0);
                        }

                        this.Map.TeamTypes.Add(teamType);
                    } catch(ArgumentOutOfRangeException) { }
                }
            }

            var triggersSection = ini.Sections.Extract("Triggers");
            if(triggersSection != null) {
                foreach(var (Key, Value) in triggersSection) {
                    var tokens = Value.Split(',');
                    if(tokens.Length >= 5) {
                        var trigger = new Trigger { Name = Key };

                        trigger.Event1.EventType = tokens[0];
                        trigger.Event1.Data = long.Parse(tokens[2]);
                        trigger.Action1.ActionType = tokens[1];
                        trigger.House = this.Map.HouseTypes.Where(t => t.Equals(tokens[3])).FirstOrDefault()?.Name ?? "None";
                        trigger.Action1.Team = tokens[4];
                        trigger.PersistantType = TriggerPersistantType.Volatile;

                        if(tokens.Length >= 6) {
                            trigger.PersistantType = (TriggerPersistantType)int.Parse(tokens[5]);
                        }

                        this.Map.Triggers.Add(trigger);
                    } else {
                        errors.Add(string.Format("Trigger '{0}' has too few tokens (expecting at least 5)", Key));
                    }
                }
            }

            var terrainSection = ini.Sections.Extract("Terrain");
            if(terrainSection != null) {
                foreach(var (Key, Value) in terrainSection) {
                    var cell = int.Parse(Key);
                    var tokens = Value.Split(',');
                    if(tokens.Length == 2) {
                        var terrainType = this.Map.TerrainTypes.Where(t => t.Equals(tokens[0])).FirstOrDefault();
                        if(terrainType != null) {
                            if(!this.Map.Technos.Add(cell, new Terrain {
                                Type = terrainType,
                                Icon = terrainType.IsTransformable ? 22 : 0,
                                Trigger = tokens[1]
                            })) {
                                var techno = this.Map.Technos[cell];
                                if(techno is Building building) {
                                    errors.Add(string.Format("Terrain '{0}' overlaps structure '{1}' in cell {2}, skipping", tokens[0], building.Type.Name, cell));
                                } else if(techno is Overlay overlay) {
                                    errors.Add(string.Format("Terrain '{0}' overlaps overlay '{1}' in cell {2}, skipping", tokens[0], overlay.Type.Name, cell));
                                } else if(techno is Terrain terrain) {
                                    errors.Add(string.Format("Terrain '{0}' overlaps terrain '{1}' in cell {2}, skipping", tokens[0], terrain.Type.Name, cell));
                                } else if(techno is InfantryGroup infantry) {
                                    errors.Add(string.Format("Terrain '{0}' overlaps infantry in cell {1}, skipping", tokens[0], cell));
                                } else if(techno is Unit unit) {
                                    errors.Add(string.Format("Terrain '{0}' overlaps unit '{1}' in cell {2}, skipping", tokens[0], unit.Type.Name, cell));
                                } else {
                                    errors.Add(string.Format("Terrain '{0}' overlaps unknown techno in cell {1}, skipping", tokens[0], cell));
                                }
                            }
                        } else {
                            errors.Add(string.Format("Terrain '{0}' references unknown terrain", tokens[0]));
                        }
                    } else {
                        errors.Add(string.Format("Terrain '{0}' has wrong number of tokens (expecting 2)", Key));
                    }
                }
            }

            var overlaySection = ini.Sections.Extract("Overlay");
            if(overlaySection != null) {
                foreach(var (Key, Value) in overlaySection) {
                    var cell = int.Parse(Key);
                    var overlayType = this.Map.OverlayTypes.Where(t => t.Equals(Value)).FirstOrDefault();
                    if(overlayType != null) {
                        this.Map.Overlay[cell] = new Overlay { Type = overlayType, Icon = 0 };
                    } else {
                        errors.Add(string.Format("Overlay '{0}' references unknown overlay", Value));
                    }
                }
            }

            var smudgeSection = ini.Sections.Extract("Smudge");
            if(smudgeSection != null) {
                foreach(var (Key, Value) in smudgeSection) {
                    var cell = int.Parse(Key);
                    var tokens = Value.Split(',');
                    if(tokens.Length == 3) {
                        var smudgeType = this.Map.SmudgeTypes.Where(t => t.Equals(tokens[0])).FirstOrDefault();
                        if(smudgeType != null) {
                            if(((smudgeType.Flag & SmudgeTypeFlag.Bib) == SmudgeTypeFlag.None)) {
                                this.Map.Smudge[cell] = new Smudge {
                                    Type = smudgeType,
                                    Icon = 0,
                                    Data = int.Parse(tokens[2])
                                };
                            } else {
                                errors.Add(string.Format("Smudge '{0}' is a bib, skipped", tokens[0]));
                            }
                        } else {
                            errors.Add(string.Format("Smudge '{0}' references unknown smudge", tokens[0]));
                        }
                    }
                }
            }

            var infantrySections = ini.Sections.Extract("Infantry");
            if(infantrySections != null) {
                foreach(var (_, Value) in infantrySections) {
                    var tokens = Value.Split(',');
                    if(tokens.Length == 8) {
                        var infantryType = this.Map.InfantryTypes.Where(t => t.Equals(tokens[1])).FirstOrDefault();
                        if(infantryType != null) {
                            var cell = int.Parse(tokens[3]);
                            var infantryGroup = this.Map.Technos[cell] as InfantryGroup;
                            if((infantryGroup == null) && (this.Map.Technos[cell] == null)) {
                                infantryGroup = new InfantryGroup();
                                this.Map.Technos.Add(cell, infantryGroup);
                            }

                            if(infantryGroup != null) {
                                var stoppingPos = int.Parse(tokens[4]);
                                if(stoppingPos < Globals.NumInfantryStops) {
                                    var direction = (byte)((int.Parse(tokens[6]) + 0x08) & ~0x0F);

                                    if(infantryGroup.Infantry[stoppingPos] == null) {
                                        infantryGroup.Infantry[stoppingPos] = new Infantry(infantryGroup) {
                                            Type = infantryType,
                                            House = this.Map.HouseTypes.Where(t => t.Equals(tokens[0])).FirstOrDefault(),
                                            Strength = int.Parse(tokens[2]),
                                            Direction = this.Map.DirectionTypes.Where(d => d.Equals(direction)).FirstOrDefault(),
                                            Mission = this.Map.MissionTypes.Where(t => t.Equals(tokens[5])).FirstOrDefault(),
                                            Trigger = tokens[7]
                                        };
                                    } else {
                                        errors.Add(string.Format("Infantry '{0}' overlaps another infantry at position {1} in cell {2}, skipping", tokens[1], stoppingPos, cell));
                                    }
                                } else {
                                    errors.Add(string.Format("Infantry '{0}' has invalid position {1} in cell {2}, skipping", tokens[1], stoppingPos, cell));
                                }
                            } else {
                                var techno = this.Map.Technos[cell];
                                if(techno is Building building) {
                                    errors.Add(string.Format("Infantry '{0}' overlaps structure '{1}' in cell {2}, skipping", tokens[1], building.Type.Name, cell));
                                } else if(techno is Overlay overlay) {
                                    errors.Add(string.Format("Infantry '{0}' overlaps overlay '{1}' in cell {2}, skipping", tokens[1], overlay.Type.Name, cell));
                                } else if(techno is Terrain terrain) {
                                    errors.Add(string.Format("Infantry '{0}' overlaps terrain '{1}' in cell {2}, skipping", tokens[1], terrain.Type.Name, cell));
                                } else if(techno is Unit unit) {
                                    errors.Add(string.Format("Infantry '{0}' overlaps unit '{1}' in cell {2}, skipping", tokens[1], unit.Type.Name, cell));
                                } else {
                                    errors.Add(string.Format("Infantry '{0}' overlaps unknown techno in cell {1}, skipping", tokens[1], cell));
                                }
                            }
                        } else {
                            errors.Add(string.Format("Infantry '{0}' references unknown infantry", tokens[1]));
                        }
                    } else {
                        errors.Add(string.Format("Infantry '{0}' has wrong number of tokens (expecting 8)", tokens[1]));
                    }
                }
            }

            var unitsSections = ini.Sections.Extract("Units");
            if(unitsSections != null) {
                foreach(var (_, Value) in unitsSections) {
                    var tokens = Value.Split(',');
                    if(tokens.Length == 7) {
                        var unitType = this.Map.UnitTypes.Where(t => t.IsUnit && t.Equals(tokens[1])).FirstOrDefault();
                        if(unitType != null) {
                            var direction = (byte)((int.Parse(tokens[4]) + 0x08) & ~0x0F);

                            var cell = int.Parse(tokens[3]);
                            if(!this.Map.Technos.Add(cell, new Unit() {
                                Type = unitType,
                                House = this.Map.HouseTypes.Where(t => t.Equals(tokens[0])).FirstOrDefault(),
                                Strength = int.Parse(tokens[2]),
                                Direction = this.Map.DirectionTypes.Where(d => d.Equals(direction)).FirstOrDefault(),
                                Mission = this.Map.MissionTypes.Where(t => t.Equals(tokens[5])).FirstOrDefault(),
                                Trigger = tokens[6]
                            })) {
                                var techno = this.Map.Technos[cell];
                                if(techno is Building building) {
                                    errors.Add(string.Format("Unit '{0}' overlaps structure '{1}' in cell {2}, skipping", tokens[1], building.Type.Name, cell));
                                } else if(techno is Overlay overlay) {
                                    errors.Add(string.Format("Unit '{0}' overlaps overlay '{1}' in cell {2}, skipping", tokens[1], overlay.Type.Name, cell));
                                } else if(techno is Terrain terrain) {
                                    errors.Add(string.Format("Unit '{0}' overlaps terrain '{1}' in cell {2}, skipping", tokens[1], terrain.Type.Name, cell));
                                } else if(techno is InfantryGroup infantry) {
                                    errors.Add(string.Format("Unit '{0}' overlaps infantry in cell {1}, skipping", tokens[1], cell));
                                } else if(techno is Unit unit) {
                                    errors.Add(string.Format("Unit '{0}' overlaps unit '{1}' in cell {2}, skipping", tokens[1], unit.Type.Name, cell));
                                } else {
                                    errors.Add(string.Format("Unit '{0}' overlaps unknown techno in cell {1}, skipping", tokens[1], cell));
                                }
                            }
                        } else {
                            errors.Add(string.Format("Unit '{0}' references unknown unit", tokens[1]));
                        }
                    } else {
                        errors.Add(string.Format("Unit '{0}' has wrong number of tokens (expecting 7)", tokens[1]));
                    }
                }
            }

            var aircraftSections = ini.Sections.Extract("Aircraft");
            if(aircraftSections != null) {
                foreach(var (_, Value) in aircraftSections) {
                    var tokens = Value.Split(',');
                    if(tokens.Length == 6) {
                        var aircraftType = this.Map.UnitTypes.Where(t => t.IsAircraft && t.Equals(tokens[1])).FirstOrDefault();
                        if(aircraftType != null) {
                            var direction = (byte)((int.Parse(tokens[4]) + 0x08) & ~0x0F);

                            var cell = int.Parse(tokens[3]);
                            if(!this.Map.Technos.Add(cell, new Unit() {
                                Type = aircraftType,
                                House = this.Map.HouseTypes.Where(t => t.Equals(tokens[0])).FirstOrDefault(),
                                Strength = int.Parse(tokens[2]),
                                Direction = this.Map.DirectionTypes.Where(d => d.Equals(direction)).FirstOrDefault(),
                                Mission = this.Map.MissionTypes.Where(t => t.Equals(tokens[5])).FirstOrDefault()
                            })) {
                                var techno = this.Map.Technos[cell];
                                if(techno is Building building) {
                                    errors.Add(string.Format("Aircraft '{0}' overlaps structure '{1}' in cell {2}, skipping", tokens[1], building.Type.Name, cell));
                                } else if(techno is Overlay overlay) {
                                    errors.Add(string.Format("Aircraft '{0}' overlaps overlay '{1}' in cell {2}, skipping", tokens[1], overlay.Type.Name, cell));
                                } else if(techno is Terrain terrain) {
                                    errors.Add(string.Format("Aircraft '{0}' overlaps terrain '{1}' in cell {2}, skipping", tokens[1], terrain.Type.Name, cell));
                                } else if(techno is InfantryGroup infantry) {
                                    errors.Add(string.Format("Aircraft '{0}' overlaps infantry in cell {1}, skipping", tokens[1], cell));
                                } else if(techno is Unit unit) {
                                    errors.Add(string.Format("Aircraft '{0}' overlaps unit '{1}' in cell {2}, skipping", tokens[1], unit.Type.Name, cell));
                                } else {
                                    errors.Add(string.Format("Aircraft '{0}' overlaps unknown techno in cell {1}, skipping", tokens[1], cell));
                                }
                            }
                        } else {
                            errors.Add(string.Format("Aircraft '{0}' references unknown aircraft", tokens[1]));
                        }
                    } else {
                        errors.Add(string.Format("Aircraft '{0}' has wrong number of tokens (expecting 6)", tokens[1]));
                    }
                }
            }

            var structuresSection = ini.Sections.Extract("Structures");
            if(structuresSection != null) {
                foreach(var (_, Value) in structuresSection) {
                    var tokens = Value.Split(',');
                    if(tokens.Length == 6) {
                        var buildingType = this.Map.BuildingTypes.Where(t => t.Equals(tokens[1])).FirstOrDefault();
                        if(buildingType != null) {
                            var direction = (byte)((int.Parse(tokens[4]) + 0x08) & ~0x0F);

                            var cell = int.Parse(tokens[3]);
                            if(!this.Map.Buildings.Add(cell, new Building() {
                                Type = buildingType,
                                House = this.Map.HouseTypes.Where(t => t.Equals(tokens[0])).FirstOrDefault(),
                                Strength = int.Parse(tokens[2]),
                                Direction = this.Map.DirectionTypes.Where(d => d.Equals(direction)).FirstOrDefault(),
                                Trigger = tokens[5]
                            })) {
                                var techno = this.Map.Technos[cell];
                                if(techno is Building building) {
                                    errors.Add(string.Format("Structure '{0}' overlaps structure '{1}' in cell {2}, skipping", tokens[1], building.Type.Name, cell));
                                } else if(techno is Overlay overlay) {
                                    errors.Add(string.Format("Structure '{0}' overlaps overlay '{1}' in cell {2}, skipping", tokens[1], overlay.Type.Name, cell));
                                } else if(techno is Terrain terrain) {
                                    errors.Add(string.Format("Structure '{0}' overlaps terrain '{1}' in cell {2}, skipping", tokens[1], terrain.Type.Name, cell));
                                } else if(techno is InfantryGroup infantry) {
                                    errors.Add(string.Format("Structure '{0}' overlaps infantry in cell {1}, skipping", tokens[1], cell));
                                } else if(techno is Unit unit) {
                                    errors.Add(string.Format("Structure '{0}' overlaps unit '{1}' in cell {2}, skipping", tokens[1], unit.Type.Name, cell));
                                } else {
                                    errors.Add(string.Format("Structure '{0}' overlaps unknown techno in cell {1}, skipping", tokens[1], cell));
                                }
                            }
                        } else {
                            errors.Add(string.Format("Structure '{0}' references unknown structure", tokens[1]));
                        }
                    } else {
                        errors.Add(string.Format("Structure '{0}' has wrong number of tokens (expecting 6)", tokens[1]));
                    }
                }
            }

            var baseSection = ini.Sections.Extract("Base");
            if(baseSection != null) {
                foreach(var (Key, Value) in baseSection) {
                    if(int.TryParse(Key, out var priority)) {
                        var tokens = Value.Split(',');
                        if(tokens.Length == 2) {
                            var buildingType = this.Map.BuildingTypes.Where(t => t.Equals(tokens[0])).FirstOrDefault();
                            if(buildingType != null) {
                                var coord = int.Parse(tokens[1]);
                                var location = new Point((coord >> 8) & 0x3F, (coord >> 24) & 0x3F);
                                if(this.Map.Buildings.OfType<Building>().Where(x => x.Location == location).FirstOrDefault().Occupier is Building building) {
                                    building.BasePriority = priority;
                                } else {
                                    this.Map.Buildings.Add(location, new Building() {
                                        Type = buildingType,
                                        Strength = 256,
                                        Direction = DirectionTypes.North,
                                        BasePriority = priority,
                                        IsPrebuilt = false
                                    });
                                }
                            } else {
                                errors.Add(string.Format("Base priority {0} references unknown structure '{1}'", priority, tokens[0]));
                            }
                        } else {
                            errors.Add(string.Format("Base priority {0} has wrong number of tokens (expecting 2)", priority));
                        }
                    } else if(!Key.Equals("Count", StringComparison.CurrentCultureIgnoreCase)) {
                        errors.Add(string.Format("Invalid base priority '{0}' (expecting integer)", Key));
                    }
                }
            }

            var waypointsSection = ini.Sections.Extract("Waypoints");
            if(waypointsSection != null) {
                foreach(var (Key, Value) in waypointsSection) {
                    if(int.TryParse(Key, out var waypoint)) {
                        if(int.TryParse(Value, out var cell)) {
                            if((waypoint >= 0) && (waypoint < this.Map.Waypoints.Length)) {
                                if(this.Map.Metrics.Contains(cell)) {
                                    this.Map.Waypoints[waypoint].Cell = cell;
                                } else {
                                    this.Map.Waypoints[waypoint].Cell = null;
                                    if(cell != -1) {
                                        errors.Add(string.Format("Waypoint {0} cell value {1} out of range (expecting between {2} and {3})", waypoint, cell, 0, this.Map.Metrics.Length - 1));
                                    }
                                }
                            } else if(cell != -1) {
                                errors.Add(string.Format("Waypoint {0} out of range (expecting between {1} and {2})", waypoint, 0, this.Map.Waypoints.Length - 1));
                            }
                        } else {
                            errors.Add(string.Format("Waypoint {0} has invalid cell '{1}' (expecting integer)", waypoint, Value));
                        }
                    } else {
                        errors.Add(string.Format("Invalid waypoint '{0}' (expecting integer)", Key));
                    }
                }
            }

            var cellTriggersSection = ini.Sections.Extract("CellTriggers");
            if(cellTriggersSection != null) {
                foreach(var (Key, Value) in cellTriggersSection) {
                    if(int.TryParse(Key, out var cell)) {
                        if(this.Map.Metrics.Contains(cell)) {
                            this.Map.CellTriggers[cell] = new CellTrigger {
                                Trigger = Value
                            };
                        } else {
                            errors.Add(string.Format("Cell trigger {0} outside map bounds", cell));
                        }
                    } else {
                        errors.Add(string.Format("Invalid cell trigger '{0}' (expecting integer)", Key));
                    }
                }
            }

            foreach(var house in this.Map.Houses) {
                if(house.Type.ID < 0) {
                    continue;
                }

                var houseSection = ini.Sections.Extract(house.Type.Name);
                if(houseSection != null) {
                    INI.ParseSection(new MapContext(this.Map, false), houseSection, house);
                    house.Enabled = true;
                } else {
                    house.Enabled = false;
                }
            }

            this.UpdateBasePlayerHouse();

            this.extraSections = ini.Sections;

            this.Map.EndUpdate();

            return errors;
        }

        private void LoadBinary(BinaryReader reader) {
            this.Map.Templates.Clear();

            for(var y = 0; y < this.Map.Metrics.Height; ++y) {
                for(var x = 0; x < this.Map.Metrics.Width; ++x) {
                    var typeValue = reader.ReadByte();
                    var iconValue = reader.ReadByte();
                    var templateType = this.Map.TemplateTypes.Where(t => t.Equals(typeValue)).FirstOrDefault();
                    if((templateType != null) && !templateType.Theaters.Contains(this.Map.Theater)) {
                        templateType = null;
                    }
                    if((templateType ?? TemplateTypes.Clear) != TemplateTypes.Clear) {
                        if(iconValue >= templateType.NumIcons) {
                            templateType = null;
                        }
                    }
                    this.Map.Templates[x, y] = (templateType != null) ? new Template { Type = templateType, Icon = iconValue } : null;
                }
            }
        }

        public bool Save(string path, FileType fileType) {
            if(!this.Validate()) {
                return false;
            }

            switch(fileType) {
            case FileType.INI:
            case FileType.BIN: {
                var iniPath = Path.ChangeExtension(path, ".ini");
                var binPath = Path.ChangeExtension(path, ".bin");
                var tgaPath = Path.ChangeExtension(path, ".tga");
                var jsonPath = Path.ChangeExtension(path, ".json");

                var ini = new INI();
                using(var iniWriter = new StreamWriter(iniPath))
                using(var binStream = new FileStream(binPath, FileMode.Create))
                using(var binWriter = new BinaryWriter(binStream))
                using(var tgaStream = new FileStream(tgaPath, FileMode.Create))
                using(var jsonStream = new FileStream(jsonPath, FileMode.Create))
                using(var jsonWriter = new JsonTextWriter(new StreamWriter(jsonStream))) {
                    this.SaveINI(ini, fileType);
                    iniWriter.Write(ini.ToString());
                    this.SaveBinary(binWriter);
                    this.SaveMapPreview(tgaStream);
                    this.SaveJSON(jsonWriter);
                }
            }
            break;
            case FileType.MEG:
            case FileType.PGM: {
                using(var iniStream = new MemoryStream())
                using(var binStream = new MemoryStream())
                using(var tgaStream = new MemoryStream())
                using(var jsonStream = new MemoryStream())
                using(var binWriter = new BinaryWriter(binStream))
                using(var jsonWriter = new JsonTextWriter(new StreamWriter(jsonStream)))
                using(var megafileBuilder = new MegafileBuilder(@"", path)) {
                    var ini = new INI();
                    this.SaveINI(ini, fileType);
                    var iniWriter = new StreamWriter(iniStream);
                    iniWriter.Write(ini.ToString());
                    iniWriter.Flush();
                    iniStream.Position = 0;

                    this.SaveBinary(binWriter);
                    binWriter.Flush();
                    binStream.Position = 0;

                    this.SaveMapPreview(tgaStream);
                    tgaStream.Position = 0;

                    this.SaveJSON(jsonWriter);
                    jsonWriter.Flush();
                    jsonStream.Position = 0;

                    var iniFile = Path.ChangeExtension(Path.GetFileName(path), ".ini").ToUpper();
                    var binFile = Path.ChangeExtension(Path.GetFileName(path), ".bin").ToUpper();
                    var tgaFile = Path.ChangeExtension(Path.GetFileName(path), ".tga").ToUpper();
                    var jsonFile = Path.ChangeExtension(Path.GetFileName(path), ".json").ToUpper();

                    megafileBuilder.AddFile(iniFile, iniStream);
                    megafileBuilder.AddFile(binFile, binStream);
                    megafileBuilder.AddFile(tgaFile, tgaStream);
                    megafileBuilder.AddFile(jsonFile, jsonStream);
                    megafileBuilder.Write();
                }
            }
            break;
            default:
                throw new NotSupportedException();
            }

            return true;
        }

        private void SaveINI(INI ini, FileType fileType) {
            if(this.extraSections != null) {
                ini.Sections.AddRange(this.extraSections);
            }

            INI.WriteSection(new MapContext(this.Map, false), ini.Sections.Add("Basic"), this.Map.BasicSection);
            INI.WriteSection(new MapContext(this.Map, false), ini.Sections.Add("Map"), this.Map.MapSection);

            if(fileType != FileType.PGM) {
                INI.WriteSection(new MapContext(this.Map, false), ini.Sections.Add("Steam"), this.Map.SteamSection);
            }

            ini.Sections.Remove("Briefing");
            if(!string.IsNullOrEmpty(this.Map.BriefingSection.Briefing)) {
                var briefingSection = ini.Sections.Add("Briefing");
                briefingSection["Text"] = this.Map.BriefingSection.Briefing.Replace(Environment.NewLine, "@");
            }

            var cellTriggersSection = ini.Sections.Add("CellTriggers");
            foreach(var (cell, cellTrigger) in this.Map.CellTriggers) {
                cellTriggersSection[cell.ToString()] = cellTrigger.Trigger;
            }

            var teamTypesSection = ini.Sections.Add("TeamTypes");
            foreach(var teamType in this.Map.TeamTypes) {
                var classes = teamType.Classes
                    .Select(c => string.Format("{0}:{1}", c.Type.Name.ToLower(), c.Count))
                    .ToArray();
                var missions = teamType.Missions
                    .Select(m => string.Format("{0}:{1}", m.Mission, m.Argument))
                    .ToArray();

                var tokens = new List<string>
                {
                    teamType.House.Name,
                    teamType.IsRoundAbout ? "1" : "0",
                    teamType.IsLearning ? "1" : "0",
                    teamType.IsSuicide ? "1" : "0",
                    teamType.IsAutocreate ? "1" : "0",
                    teamType.IsMercenary ? "1" : "0",
                    teamType.RecruitPriority.ToString(),
                    teamType.MaxAllowed.ToString(),
                    teamType.InitNum.ToString(),
                    teamType.Fear.ToString(),
                    classes.Length.ToString(),
                    string.Join(",", classes),
                    missions.Length.ToString(),
                    string.Join(",", missions),
                    teamType.IsReinforcable ? "1" : "0",
                    teamType.IsPrebuilt ? "1" : "0"
                };

                teamTypesSection[teamType.Name] = string.Join(",", tokens.Where(t => !string.IsNullOrEmpty(t)));
            }

            var triggersSection = ini.Sections.Add("Triggers");
            foreach(var trigger in this.Map.Triggers) {
                if(string.IsNullOrEmpty(trigger.Name)) {
                    continue;
                }

                var tokens = new List<string>
                {
                    trigger.Event1.EventType,
                    trigger.Action1.ActionType,
                    trigger.Event1.Data.ToString(),
                    trigger.House,
                    trigger.Action1.Team,
                    ((int)trigger.PersistantType).ToString()
                };

                triggersSection[trigger.Name] = string.Join(",", tokens);
            }

            var waypointsSection = ini.Sections.Add("Waypoints");
            for(var i = 0; i < this.Map.Waypoints.Length; ++i) {
                var waypoint = this.Map.Waypoints[i];
                if(waypoint.Cell.HasValue) {
                    waypointsSection[i.ToString()] = waypoint.Cell.Value.ToString();
                }
            }

            var baseSection = ini.Sections.Add("Base");
            var baseBuildings = this.Map.Buildings.OfType<Building>().Where(x => x.Occupier.BasePriority >= 0).OrderByDescending(x => x.Occupier.BasePriority).ToArray();
            var baseIndex = baseBuildings.Length - 1;
            foreach(var (location, building) in baseBuildings) {
                var key = baseIndex.ToString("D3");
                baseIndex--;

                baseSection[key] = string.Format("{0},{1}",
                    building.Type.Name.ToUpper(),
                    ((location.Y & 0x3F) << 24) | ((location.X & 0x3F) << 8)
                );
            }
            baseSection["Count"] = baseBuildings.Length.ToString();

            var infantrySection = ini.Sections.Add("Infantry");
            var infantryIndex = 0;
            foreach(var (location, infantryGroup) in this.Map.Technos.OfType<InfantryGroup>()) {
                for(var i = 0; i < infantryGroup.Infantry.Length; ++i) {
                    var infantry = infantryGroup.Infantry[i];
                    if(infantry == null) {
                        continue;
                    }

                    var key = infantryIndex.ToString("D3");
                    infantryIndex++;

                    this.Map.Metrics.GetCell(location, out var cell);
                    infantrySection[key] = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                        infantry.House.Name,
                        infantry.Type.Name,
                        infantry.Strength,
                        cell,
                        i,
                        infantry.Mission,
                        infantry.Direction.ID,
                        infantry.Trigger
                    );
                }
            }

            var structuresSection = ini.Sections.Add("Structures");
            var structureIndex = 0;
            foreach(var (location, building) in this.Map.Buildings.OfType<Building>().Where(x => x.Occupier.IsPrebuilt)) {
                var key = structureIndex.ToString("D3");
                structureIndex++;

                this.Map.Metrics.GetCell(location, out var cell);
                structuresSection[key] = string.Format("{0},{1},{2},{3},{4},{5}",
                    building.House.Name,
                    building.Type.Name,
                    building.Strength,
                    cell,
                    building.Direction.ID,
                    building.Trigger
                );
            }

            var unitsSection = ini.Sections.Add("Units");
            var unitIndex = 0;
            foreach(var (location, unit) in this.Map.Technos.OfType<Unit>().Where(u => u.Occupier.Type.IsUnit)) {
                var key = unitIndex.ToString("D3");
                unitIndex++;

                this.Map.Metrics.GetCell(location, out var cell);
                unitsSection[key] = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    unit.House.Name,
                    unit.Type.Name,
                    unit.Strength,
                    cell,
                    unit.Direction.ID,
                    unit.Mission,
                    unit.Trigger
                );
            }

            var aircraftSection = ini.Sections.Add("Aircraft");
            var aircraftIndex = 0;
            foreach(var (location, aircraft) in this.Map.Technos.OfType<Unit>().Where(u => u.Occupier.Type.IsAircraft)) {
                var key = aircraftIndex.ToString("D3");
                aircraftIndex++;

                this.Map.Metrics.GetCell(location, out var cell);
                aircraftSection[key] = string.Format("{0},{1},{2},{3},{4},{5}",
                    aircraft.House.Name,
                    aircraft.Type.Name,
                    aircraft.Strength,
                    cell,
                    aircraft.Direction.ID,
                    aircraft.Mission
                );
            }

            foreach(var house in this.Map.Houses) {
                if((house.Type.ID < 0) || !house.Enabled) {
                    continue;
                }

                INI.WriteSection(new MapContext(this.Map, false), ini.Sections.Add(house.Type.Name), house);
            }

            var overlaySection = ini.Sections.Add("Overlay");
            foreach(var (cell, overlay) in this.Map.Overlay) {
                overlaySection[cell.ToString()] = overlay.Type.Name;
            }

            var smudgeSection = ini.Sections.Add("Smudge");
            foreach(var (cell, smudge) in this.Map.Smudge.Where(item => (item.Value.Type.Flag & SmudgeTypeFlag.Bib) == SmudgeTypeFlag.None)) {
                smudgeSection[cell.ToString()] = string.Format("{0},{1},{2}", smudge.Type.Name, cell, smudge.Data);
            }

            var terrainSection = ini.Sections.Add("Terrain");
            foreach(var (location, terrain) in this.Map.Technos.OfType<Terrain>()) {
                this.Map.Metrics.GetCell(location, out var cell);
                terrainSection[cell.ToString()] = string.Format("{0},None", terrain.Type.Name);
            }
        }

        private void SaveBinary(BinaryWriter writer) {
            for(var y = 0; y < this.Map.Metrics.Height; ++y) {
                for(var x = 0; x < this.Map.Metrics.Width; ++x) {
                    var template = this.Map.Templates[x, y];
                    if(template != null) {
                        writer.Write((byte)template.Type.ID);
                        writer.Write((byte)template.Icon);
                    } else {
                        writer.Write(byte.MaxValue);
                        writer.Write(byte.MaxValue);
                    }
                }
            }
        }

        private void SaveMapPreview(Stream stream) => this.Map.GenerateMapPreview().Save(stream);

        private void SaveJSON(JsonTextWriter writer) {
            writer.WriteStartObject();
            writer.WritePropertyName("MapTileX");
            writer.WriteValue(this.Map.MapSection.X);
            writer.WritePropertyName("MapTileY");
            writer.WriteValue(this.Map.MapSection.Y);
            writer.WritePropertyName("MapTileWidth");
            writer.WriteValue(this.Map.MapSection.Width);
            writer.WritePropertyName("MapTileHeight");
            writer.WriteValue(this.Map.MapSection.Height);
            writer.WritePropertyName("Theater");
            writer.WriteValue(this.Map.MapSection.Theater.Name.ToUpper());
            writer.WritePropertyName("Waypoints");
            writer.WriteStartArray();
            foreach(var waypoint in this.Map.Waypoints.Where(w => (w.Flag == WaypointFlag.PlayerStart) && w.Cell.HasValue)) {
                writer.WriteValue(waypoint.Cell.Value);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        private bool Validate() {
            var sb = new StringBuilder("Error(s) during map validation:");

            var ok = true;
            var numAircraft = this.Map.Technos.OfType<Unit>().Where(u => u.Occupier.Type.IsAircraft).Count();
            var numBuildings = this.Map.Buildings.OfType<Building>().Where(x => x.Occupier.IsPrebuilt).Count();
            var numInfantry = this.Map.Technos.OfType<InfantryGroup>().Sum(item => item.Occupier.Infantry.Count(i => i != null));
            var numTerrain = this.Map.Technos.OfType<Terrain>().Count();
            var numUnits = this.Map.Technos.OfType<Unit>().Where(u => u.Occupier.Type.IsUnit).Count();
            var numWaypoints = this.Map.Waypoints.Count(w => w.Cell.HasValue);

            if(numAircraft > Constants.MaxAircraft) {
                sb.Append(Environment.NewLine + string.Format("Maximum number of aircraft exceeded ({0} > {1})", numAircraft, Constants.MaxAircraft));
                ok = false;
            }

            if(numBuildings > Constants.MaxBuildings) {
                sb.Append(Environment.NewLine + string.Format("Maximum number of structures exceeded ({0} > {1})", numBuildings, Constants.MaxBuildings));
                ok = false;
            }

            if(numInfantry > Constants.MaxInfantry) {
                sb.Append(Environment.NewLine + string.Format("Maximum number of infantry exceeded ({0} > {1})", numInfantry, Constants.MaxInfantry));
                ok = false;
            }

            if(numTerrain > Constants.MaxTerrain) {
                sb.Append(Environment.NewLine + string.Format("Maximum number of terrain objects exceeded ({0} > {1})", numTerrain, Constants.MaxTerrain));
                ok = false;
            }

            if(numUnits > Constants.MaxUnits) {
                sb.Append(Environment.NewLine + string.Format("Maximum number of units exceeded ({0} > {1})", numUnits, Constants.MaxUnits));
                ok = false;
            }

            if(this.Map.TeamTypes.Count > Constants.MaxTeams) {
                sb.Append(Environment.NewLine + string.Format("Maximum number of team types exceeded ({0} > {1})", this.Map.TeamTypes.Count, Constants.MaxTeams));
                ok = false;
            }

            if(this.Map.Triggers.Count > Constants.MaxTriggers) {
                sb.Append(Environment.NewLine + string.Format("Maximum number of triggers exceeded ({0} > {1})", this.Map.Triggers.Count, Constants.MaxTriggers));
                ok = false;
            }

            if(!this.Map.BasicSection.SoloMission && (numWaypoints < 2)) {
                sb.Append(Environment.NewLine + "Skirmish/Multiplayer maps need at least 2 waypoints for player starting locations.");
                ok = false;
            }

            var homeWaypoint = this.Map.Waypoints.Where(w => w.Equals("Home")).FirstOrDefault();
            if(this.Map.BasicSection.SoloMission && !homeWaypoint.Cell.HasValue) {
                sb.Append(Environment.NewLine + string.Format("Single-player maps need the Home waypoint to be placed.", this.Map.Triggers.Count, Constants.MaxTriggers));
                ok = false;
            }

            if(!ok) {
                MessageBox.Show(sb.ToString(), "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ok;
        }

        private void BasicSection_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch(e.PropertyName) {
            case "Player": {
                this.UpdateBasePlayerHouse();
            }
            break;
            }
        }

        private void MapSection_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch(e.PropertyName) {
            case "Theater": {
                this.Map.InitTheater(this.GameType);
            }
            break;
            }
        }

        private void UpdateBasePlayerHouse() {
            this.Map.BasicSection.BasePlayer = HouseTypes.GetBasePlayer(this.Map.BasicSection.Player);

            var basePlayer = this.Map.HouseTypes.Where(h => h.Equals(this.Map.BasicSection.BasePlayer)).FirstOrDefault() ?? this.Map.HouseTypes.First();
            foreach(var (_, building) in this.Map.Buildings.OfType<Building>()) {
                if(!building.IsPrebuilt) {
                    building.House = basePlayer;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if(!this.disposedValue) {
                if(disposing) {
                    this.MapImage?.Dispose();
                }
                this.disposedValue = true;
            }
        }

        public void Dispose() => this.Dispose(true);
        #endregion IDisposable Support
    }
}
