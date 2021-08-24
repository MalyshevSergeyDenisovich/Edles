using Bin.WorldGeneration.Data;
using UnityEditor;
using UnityEngine;

namespace Bin.Edit
{
	[CustomEditor (typeof(UpdatableData), true)]
	public class UpdatableDataEditor : Editor {

		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			var data = (UpdatableData)target;

			if (GUILayout.Button ("Update")) {
				data.NotifyOfUpdatedValues ();
				EditorUtility.SetDirty (target);
			}
		}
	
	}
}
