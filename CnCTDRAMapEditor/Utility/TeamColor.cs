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
using System.Drawing;
using System.Numerics;
using System.Xml;

namespace MobiusEditor.Utility {
    public class TeamColor {
        private readonly TeamColorManager teamColorManager;
        private readonly MegafileManager megafileManager;

        public string Variant {
            get; private set;
        }

        public string Name {
            get; private set;
        }

        private Color? lowerBounds;
        public Color LowerBounds => this.lowerBounds.HasValue ? this.lowerBounds.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].LowerBounds : default);

        private Color? upperBounds;
        public Color UpperBounds => this.upperBounds.HasValue ? this.upperBounds.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].UpperBounds : default);

        private float? fudge;
        public float Fudge => this.fudge.HasValue ? this.fudge.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].Fudge : default);

        private Vector3? hsvShift;
        public Vector3 HSVShift => this.hsvShift.HasValue ? this.hsvShift.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].HSVShift : default);

        private Vector3? inputLevels;
        public Vector3 InputLevels => this.inputLevels.HasValue ? this.inputLevels.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].InputLevels : default);

        private Vector2? outputLevels;
        public Vector2 OutputLevels => this.outputLevels.HasValue ? this.outputLevels.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].OutputLevels : default);

        private Vector3? overallInputLevels;
        public Vector3 OverallInputLevels => this.overallInputLevels.HasValue ? this.overallInputLevels.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].OverallInputLevels : default);

        private Vector2? overallOutputLevels;
        public Vector2 OverallOutputLevels => this.overallOutputLevels.HasValue ? this.overallOutputLevels.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].OverallOutputLevels : default);

        private Color? radarMapColor;
        public Color RadarMapColor => this.radarMapColor.HasValue ? this.radarMapColor.Value : ((this.Variant != null) ? this.teamColorManager[this.Variant].RadarMapColor : default);

        public TeamColor(TeamColorManager teamColorManager, MegafileManager megafileManager) {
            this.teamColorManager = teamColorManager;
            this.megafileManager = megafileManager;
        }

        public void Load(string xml) {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            var node = xmlDoc.FirstChild;
            this.Name = node.Attributes["Name"].Value;
            this.Variant = node.Attributes["Variant"]?.Value;

            var lowerBoundsNode = node.SelectSingleNode("LowerBounds");
            if(lowerBoundsNode != null) {
                this.lowerBounds = Color.FromArgb(
                    (int)(float.Parse(lowerBoundsNode.SelectSingleNode("R").InnerText) * 255),
                    (int)(float.Parse(lowerBoundsNode.SelectSingleNode("G").InnerText) * 255),
                    (int)(float.Parse(lowerBoundsNode.SelectSingleNode("B").InnerText) * 255)
                );
            }

            var upperBoundsNode = node.SelectSingleNode("UpperBounds");
            if(upperBoundsNode != null) {
                this.upperBounds = Color.FromArgb(
                    (int)(float.Parse(upperBoundsNode.SelectSingleNode("R").InnerText) * 255),
                    (int)(float.Parse(upperBoundsNode.SelectSingleNode("G").InnerText) * 255),
                    (int)(float.Parse(upperBoundsNode.SelectSingleNode("B").InnerText) * 255)
                );
            }

            var fudgeNode = node.SelectSingleNode("Fudge");
            if(fudgeNode != null) {
                this.fudge = float.Parse(fudgeNode.InnerText);
            }

            var hsvShiftNode = node.SelectSingleNode("HSVShift");
            if(hsvShiftNode != null) {
                this.hsvShift = new Vector3(
                    float.Parse(hsvShiftNode.SelectSingleNode("X").InnerText),
                    float.Parse(hsvShiftNode.SelectSingleNode("Y").InnerText),
                    float.Parse(hsvShiftNode.SelectSingleNode("Z").InnerText)
                );
            }

            var inputLevelsNode = node.SelectSingleNode("InputLevels");
            if(inputLevelsNode != null) {
                this.inputLevels = new Vector3(
                    float.Parse(inputLevelsNode.SelectSingleNode("X").InnerText),
                    float.Parse(inputLevelsNode.SelectSingleNode("Y").InnerText),
                    float.Parse(inputLevelsNode.SelectSingleNode("Z").InnerText)
                );
            }

            var outputLevelsNode = node.SelectSingleNode("OutputLevels");
            if(outputLevelsNode != null) {
                this.outputLevels = new Vector2(
                    float.Parse(outputLevelsNode.SelectSingleNode("X").InnerText),
                    float.Parse(outputLevelsNode.SelectSingleNode("Y").InnerText)
                );
            }

            var overallInputLevelsNode = node.SelectSingleNode("OverallInputLevels");
            if(overallInputLevelsNode != null) {
                this.overallInputLevels = new Vector3(
                    float.Parse(overallInputLevelsNode.SelectSingleNode("X").InnerText),
                    float.Parse(overallInputLevelsNode.SelectSingleNode("Y").InnerText),
                    float.Parse(overallInputLevelsNode.SelectSingleNode("Z").InnerText)
                );
            }

            var overallOutputLevelsNode = node.SelectSingleNode("OverallOutputLevels");
            if(outputLevelsNode != null) {
                this.overallOutputLevels = new Vector2(
                    float.Parse(overallOutputLevelsNode.SelectSingleNode("X").InnerText),
                    float.Parse(overallOutputLevelsNode.SelectSingleNode("Y").InnerText)
                );
            }

            var radarMapColorNode = node.SelectSingleNode("RadarMapColor");
            if(radarMapColorNode != null) {
                this.radarMapColor = Color.FromArgb(
                    (int)(float.Parse(radarMapColorNode.SelectSingleNode("R").InnerText) * 255),
                    (int)(float.Parse(radarMapColorNode.SelectSingleNode("G").InnerText) * 255),
                    (int)(float.Parse(radarMapColorNode.SelectSingleNode("B").InnerText) * 255)
                );
            }
        }

        public void Flatten() {
            this.lowerBounds = this.LowerBounds;
            this.upperBounds = this.UpperBounds;
            this.fudge = this.Fudge;
            this.hsvShift = this.HSVShift;
            this.inputLevels = this.InputLevels;
            this.outputLevels = this.OutputLevels;
            this.overallInputLevels = this.OverallInputLevels;
            this.overallOutputLevels = this.OverallOutputLevels;
            this.radarMapColor = this.RadarMapColor;
        }
    }
}
