using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WorldBuilder.Runtime.Data;

namespace WorldBuilder.Editor.BiomeSetter
{
    public sealed class BiomeSetterTool : IWorldBuilderTool, IRaycastConsumer
    {
        [SerializeField] private float chunkSize = 16f;
        [SerializeField] private BiomeType selectedBiome = BiomeType.Forest;
        [SerializeField] private bool eraseMode;

        private readonly IChunkBiomeMap biomeMap;
        private readonly IBiomeDataProvider biomeProvider;
        private readonly ChunkCoordCalculator calculator = new ChunkCoordCalculator();
        private readonly Dictionary<BiomeType, Color> colorLookup = new Dictionary<BiomeType, Color>();

        public BiomeSetterTool(IChunkBiomeMap biomeMap, IBiomeDataProvider biomeProvider)
        {
            this.biomeMap = biomeMap;
            this.biomeProvider = biomeProvider;
        }

        public string ToolName => "Biome Setter";

        public void OnEnable()
        {
            RefreshColors();
        }

        public bool TryRaycast(out RaycastHit hit)
        {
            return SceneRaycaster.TryRaycast(Event.current.mousePosition, out hit);
        }

        public void OnInspectorGUI()
        {
            selectedBiome = (BiomeType)EditorGUILayout.EnumPopup("Biome", selectedBiome);
            chunkSize = EditorGUILayout.FloatField("Chunk Size", chunkSize);
            eraseMode = EditorGUILayout.Toggle("Erase Mode", eraseMode);

            EditorGUILayout.LabelField("Assigned Chunks", biomeMap.Entries.Count.ToString());

            if (GUILayout.Button("Refresh Biome Colors"))
            {
                RefreshColors();
            }
        }

        public void OnSceneGUI()
        {
            DrawChunks();

            Event e = Event.current;
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                if (TryRaycast(out RaycastHit hit))
                {
                    Vector3Int coord = calculator.ToChunkCoord(hit.point, chunkSize);
                    if (eraseMode)
                    {
                        biomeMap.Remove(coord);
                    }
                    else
                    {
                        biomeMap.Set(coord, selectedBiome);
                    }
                    e.Use();
                    SceneView.RepaintAll();
                }
            }
        }

        private void DrawChunks()
        {
            float half = chunkSize * 0.5f;

            foreach (KeyValuePair<Vector3Int, BiomeType> entry in biomeMap.Entries)
            {
                Vector3 center = calculator.ToWorldCenter(entry.Key, chunkSize);
                Vector3[] corners =
                {
                    center + new Vector3(-half, 0f, -half),
                    center + new Vector3(-half, 0f, half),
                    center + new Vector3(half, 0f, half),
                    center + new Vector3(half, 0f, -half)
                };

                Color color = GetColor(entry.Value);
                Color face = new Color(color.r, color.g, color.b, 0.25f);
                Handles.DrawSolidRectangleWithOutline(corners, face, color);
            }
        }

        private Color GetColor(BiomeType biome)
        {
            if (colorLookup.TryGetValue(biome, out Color color))
            {
                return color;
            }

            return Color.gray;
        }

        private void RefreshColors()
        {
            colorLookup.Clear();
            BiomeData[] all = biomeProvider.GetAll();
            for (int i = 0; i < all.Length; i++)
            {
                colorLookup[all[i].BiomeType] = all[i].DebugColor;
            }
        }
    }
}
