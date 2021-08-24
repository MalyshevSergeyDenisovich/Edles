using UnityEngine;

namespace Bin.Edit
{
	public class HideOnPlay : MonoBehaviour
	{
		private void Start()
		{
			gameObject.SetActive(false);
		}
	}
}
