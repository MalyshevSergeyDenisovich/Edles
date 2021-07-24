using System.Collections.Generic;

namespace Bin.Global
{
    public static class Globals
    {
        public const string TerrainTag = "Terrain";
        public const string UnitTag = "Unit";
        public static readonly List<UnitManager> SelectedUnits = new List<UnitManager>();
    }
}