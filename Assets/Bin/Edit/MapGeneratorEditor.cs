using UnityEngine;
using Bin.WorldGeneration;
using UnityEditor;

namespace Bin.Edit
{
    [CustomEditor (typeof (MapGenerator) )]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var mapGen = (MapGenerator) target;

            if (DrawDefaultInspector())
            {
                if (mapGen.autoUpdate)
                {
                    mapGen.DrawMapInEditor();
                }
            }

            if (GUILayout.Button("Generate"))
            {
                mapGen.DrawMapInEditor();
            }
        }
    }
}