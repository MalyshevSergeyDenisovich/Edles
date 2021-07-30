using UnityEngine;

namespace Bin.WorldGeneration
{
	public class HideOnPlay : MonoBehaviour
	{
		private void Start()
		{
			gameObject.SetActive(false);
		}
	}
}
