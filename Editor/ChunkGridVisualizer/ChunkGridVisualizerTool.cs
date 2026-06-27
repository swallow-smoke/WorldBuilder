using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using WorldBuilder.Runtime.Data;

namespace WorldBuilder.Editor.ChunkGridVisualizer
{
    public sealed class ChunkGridVisualizerTool : IWorldBuilderTool
    {
        [SerializeField] private int chunkSize = 16;
        [SerializeField] private int viewRadius = 4;

        private readonly IChunkBiomeMap biomeMap;
        private readonly ChunkCoordCalculator calculator = new ChunkCoordCalculator();

        public ChunkGridVisualizerTool(IChunkBiomeMap biomeMap)
        {
            this.biomeMap = biomeMap;
        }

        public string ToolName => WorldBuilderLocalization.Get("tool.chunkGrid");

        public Texture2D ToolIcon => null;

        public void OnEnable()
        {
        }

        public VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.Add(InspectorHelp.Build(ToolName, "help.chunkGrid"));

            SliderInt size = new SliderInt("Chunk Size", 1, 128) { value = chunkSize };
            size.RegisterValueChangedCallback(evt => chunkSize = evt.newValue);
            root.Add(size);

            SliderInt radius = new SliderInt("View Radius", 1, 32) { value = viewRadius };
            radius.RegisterValueChangedCallback(evt => viewRadius = evt.newValue);
            root.Add(radius);

            return root;
        }

        public void OnSceneGUI()
        {
            SceneView view = SceneView.lastActiveSceneView;
            if (view == null)
            {
                return;
            }

            Vector3 pivot = view.pivot;
            Vector3Int center = calculator.ToChunkCoord(pivot, chunkSize);

            for (int x = -viewRadius; x <= viewRadius; x++)
            {
                for (int z = -viewRadius; z <= viewRadius; z++)
                {
                    Vector3Int coord = new Vector3Int(center.x + x, center.y, center.z + z);
                    bool loaded = biomeMap.TryGet(coord, out BiomeType _);
                    Handles.color = loaded ? Color.green : Color.gray;
                    DrawCell(coord);
                }
            }
        }

        private void DrawCell(Vector3Int coord)
        {
            Vector3 origin = new Vector3(coord.x * chunkSize, 0f, coord.z * chunkSize);
            Vector3 right = new Vector3(chunkSize, 0f, 0f);
            Vector3 forward = new Vector3(0f, 0f, chunkSize);

            Vector3 a = origin;
            Vector3 b = origin + right;
            Vector3 c = origin + right + forward;
            Vector3 d = origin + forward;

            Handles.DrawLine(a, b);
            Handles.DrawLine(b, c);
            Handles.DrawLine(c, d);
            Handles.DrawLine(d, a);
        }
    }
}
