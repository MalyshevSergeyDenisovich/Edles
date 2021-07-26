using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Bin.Global
{
    public static class Globals
    {
        public static Color ScreenRectColor = new Color(0.5f, 1f, 0.4f, 0.2f);
        public static Color ScreenRectBorderColor = new Color(0.5f, 1f, 0.4f);

        public static readonly RuntimeAnimatorController IdleAnimation = Resources.Load<RuntimeAnimatorController>("AnimationControllers/Idle");
        public static readonly RuntimeAnimatorController WalkAnimation = Resources.Load<RuntimeAnimatorController>("AnimationControllers/Walk");
        
        public const string TerrainTag = "Terrain";
        public const string UnitTag = "Unit";
        public static readonly List<UnitManager> SelectedUnits = new List<UnitManager>();
        public static readonly List<UnitManager> SelectableUnits = new List<UnitManager>();
        public static readonly Random Random = new Random();
    }
}