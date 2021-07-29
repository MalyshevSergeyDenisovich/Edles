using UnityEngine;

namespace Bin.WorldGeneration.Data
{
    [CreateAssetMenu]
    public class TerrainData : UpdatableData
    {
        public float uniformScale = 2f;
        public bool useFlatShading;
        public bool useFalloff;
        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;
    }
}