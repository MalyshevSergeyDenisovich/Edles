using UnityEngine;

namespace Bin.WorldGeneration.Data
{
	[CreateAssetMenu]
	public class HeightMapSettings : UpdatableData
	{
		public NoiseSettings noiseSettings;

		public float heightMultiplier;
		public AnimationCurve heightCurve;

		public float MINHeight => heightMultiplier * heightCurve.Evaluate (0);

		public float MAXHeight => heightMultiplier * heightCurve.Evaluate (1);

#if UNITY_EDITOR

		protected override void OnValidate() 
		{
			noiseSettings.ValidateValues ();
			base.OnValidate ();
		}
#endif

	}
}
