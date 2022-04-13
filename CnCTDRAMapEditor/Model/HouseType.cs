﻿//
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiusEditor.Model {
    public class HouseType {
        public sbyte ID {
            get; private set;
        }

        public string Name {
            get; private set;
        }

        public string UnitTeamColor {
            get; private set;
        }

        public string BuildingTeamColor {
            get; private set;
        }

        public IDictionary<string, string> OverrideTeamColors {
            get; private set;
        }

        public HouseType(sbyte id, string name, string unitTeamColor, string buildingTeamColor, params (string type, string teamColor)[] overrideTeamColors) {
            this.ID = id;
            this.Name = name;
            this.UnitTeamColor = unitTeamColor;
            this.BuildingTeamColor = buildingTeamColor;
            this.OverrideTeamColors = overrideTeamColors.ToDictionary(x => x.type, x => x.teamColor);
        }

        public HouseType(sbyte id, string name, string teamColor)
            : this(id, name, teamColor, teamColor) {
        }

        public HouseType(sbyte id, string name)
            : this(id, name, null) {
        }

        public override bool Equals(object obj) {
            if(obj is HouseType) {
                return this == obj;
            } else if(obj is sbyte) {
                return this.ID == (sbyte)obj;
            } else if(obj is string) {
                return string.Equals(this.Name, obj as string, StringComparison.OrdinalIgnoreCase);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode() => this.ID.GetHashCode();

        public override string ToString() => this.Name;
    }
}
