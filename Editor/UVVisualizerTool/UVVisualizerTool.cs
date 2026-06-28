using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace WorldBuilder.Editor.UVVisualizerTool
{
    public enum UVChannel
    {
        UV0,
        UV1,
        UV2,
        UV3
    }

    public sealed class UVVisualizerTool : IWorldBuilderTool
    {
        [SerializeField] private bool useSelection = true;
        [SerializeField] private UVChannel channel = UVChannel.UV0;
        [SerializeField] private Color edgeColor = Color.green;
        [SerializeField] private Color seamColor = new Color(1f, 0.3f, 0.1f);
        [SerializeField] private float displaySize = 3f;

        private MeshFilter target;

        public string ToolName => WorldBuilderLocalization.Get("tool.uvVisualizer");

        public Texture2D ToolIcon => null;

        public void OnEnable()
        {
        }

        public VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.Add(InspectorHelp.Build(ToolName, "help.uvVisualizer"));

            ObjectField targetField = new ObjectField("Target MeshFilter")
            {
                objectType = typeof(MeshFilter),
                allowSceneObjects = true,
                value = target
            };
            targetField.RegisterValueChangedCallback(evt => target = evt.newValue as MeshFilter);
            root.Add(targetField);

            Toggle selectionToggle = new Toggle("Use Selection") { value = useSelection };
            selectionToggle.RegisterValueChangedCallback(evt =>
            {
                useSelection = evt.newValue;
                targetField.SetEnabled(!useSelection);
            });
            targetField.SetEnabled(!useSelection);
            root.Add(selectionToggle);

            EnumField channelField = new EnumField("UV Channel", channel);
            channelField.RegisterValueChangedCallback(evt =>
            {
                channel = (UVChannel)evt.newValue;
                SceneView.RepaintAll();
            });
            root.Add(channelField);

            ColorField colorField = new ColorField("Edge Color") { value = edgeColor };
            colorField.RegisterValueChangedCallback(evt =>
            {
                edgeColor = evt.newValue;
                SceneView.RepaintAll();
            });
            root.Add(colorField);

            return root;
        }

        public void OnSceneGUI()
        {
            MeshFilter filter = ResolveTarget();
            if (filter == null || filter.sharedMesh == null)
            {
                return;
            }

            Mesh mesh = filter.sharedMesh;
            List<Vector2> uv = new List<Vector2>();
            mesh.GetUVs((int)channel, uv);
            if (uv.Count == 0)
            {
                return;
            }

            int[] triangles = mesh.triangles;
            Vector3 origin = filter.transform.position + Vector3.up * (filter.sharedMesh.bounds.size.y + 1f);
            Vector3 right = Vector3.right * displaySize;
            Vector3 up = Vector3.forward * displaySize;

            Dictionary<long, int> edgeCounts = new Dictionary<long, int>();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                CountEdge(edgeCounts, triangles[i], triangles[i + 1]);
                CountEdge(edgeCounts, triangles[i + 1], triangles[i + 2]);
                CountEdge(edgeCounts, triangles[i + 2], triangles[i]);
            }

            for (int i = 0; i < triangles.Length; i += 3)
            {
                DrawEdge(uv, edgeCounts, triangles[i], triangles[i + 1], origin, right, up);
                DrawEdge(uv, edgeCounts, triangles[i + 1], triangles[i + 2], origin, right, up);
                DrawEdge(uv, edgeCounts, triangles[i + 2], triangles[i], origin, right, up);
            }
        }

        private void CountEdge(Dictionary<long, int> edgeCounts, int a, int b)
        {
            long key = EdgeKey(a, b);
            edgeCounts.TryGetValue(key, out int count);
            edgeCounts[key] = count + 1;
        }

        private void DrawEdge(List<Vector2> uv, Dictionary<long, int> edgeCounts, int a, int b, Vector3 origin, Vector3 right, Vector3 up)
        {
            if (a >= uv.Count || b >= uv.Count)
            {
                return;
            }

            Vector3 pa = origin + right * uv[a].x + up * uv[a].y;
            Vector3 pb = origin + right * uv[b].x + up * uv[b].y;

            edgeCounts.TryGetValue(EdgeKey(a, b), out int count);
            Handles.color = count <= 1 ? seamColor : edgeColor;
            Handles.DrawLine(pa, pb);
        }

        private long EdgeKey(int a, int b)
        {
            int min = Mathf.Min(a, b);
            int max = Mathf.Max(a, b);
            return ((long)min << 32) | (uint)max;
        }

        private MeshFilter ResolveTarget()
        {
            if (!useSelection)
            {
                return target;
            }

            return Selection.activeGameObject != null ? Selection.activeGameObject.GetComponent<MeshFilter>() : null;
        }
    }
}
