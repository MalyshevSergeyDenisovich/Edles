using UnityEngine;

namespace Bin.WorldGeneration.Data
{
	[CreateAssetMenu]
	public class MeshSettings : UpdatableData {

		public const int NumSupportedLODs = 5;
		private const int NumSupportedChunkSizes = 9;
		private const int NumSupportedFlatshadedChunkSizes = 3;
		private static readonly int[] SupportedChunkSizes = {48,72,96,120,144,168,192,216,240};
	
		public float meshScale = 2.5f;
		public bool useFlatShading;

		[Range(0,NumSupportedChunkSizes-1)]
		public int chunkSizeIndex;
		[Range(0,NumSupportedFlatshadedChunkSizes-1)]
		public int flatshadedChunkSizeIndex;


		
		public int NumVertsPerLine => SupportedChunkSizes [useFlatShading ? flatshadedChunkSizeIndex : chunkSizeIndex] + 5;

		public float MeshWorldSize => (NumVertsPerLine - 3) * meshScale;
	}
}
