using Scripts;
using UnityEditor;
using UnityEngine;

namespace EditorScripts {
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mapGenerator = (MapGenerator)target;

            if (GUILayout.Button("Initialize")) {
                mapGenerator.Refresh();
            }

            if (GUILayout.Button("Generate")) {
                mapGenerator.Generate();
            }
        }
    }
}
