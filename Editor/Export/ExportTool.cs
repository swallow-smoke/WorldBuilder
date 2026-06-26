using System.IO;
using UnityEditor;
using UnityEngine;
using WorldBuilder.Runtime.Data;

namespace WorldBuilder.Editor.Export
{
    public sealed class ExportTool : IWorldBuilderTool
    {
        [SerializeField] private float chunkSize = 16f;

        private readonly SceneDataCollector collector;
        private readonly ChunkSerializer serializer;

        public ExportTool(SceneDataCollector collector, ChunkSerializer serializer)
        {
            this.collector = collector;
            this.serializer = serializer;
        }

        public string ToolName => "Export";

        public void OnEnable()
        {
        }

        public void OnSceneGUI()
        {
        }

        public void OnInspectorGUI()
        {
            chunkSize = EditorGUILayout.FloatField("Chunk Size", chunkSize);

            if (GUILayout.Button("Export World"))
            {
                Export();
            }
        }

        private void Export()
        {
            string path = Path.Combine(Application.dataPath, "WorldBuilder/Export/world.bin");

            try
            {
                EditorUtility.DisplayProgressBar("WorldBuilder Export", "Collecting scene data...", 0.33f);
                ChunkData[] chunks = collector.Collect(chunkSize);

                EditorUtility.DisplayProgressBar("WorldBuilder Export", "Serializing chunks...", 0.66f);
                serializer.Write(path, chunks);

                EditorUtility.DisplayProgressBar("WorldBuilder Export", "Finalizing...", 1f);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();
            Debug.Log("WorldBuilder export complete: " + path);
        }
    }
}
