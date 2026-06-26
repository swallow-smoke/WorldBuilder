using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WorldBuilder.Editor.TerrainPainting
{
    public sealed class TerrainPaintTool : IWorldBuilderTool, IUndoable, IRaycastConsumer
    {
        [SerializeField] private float brushRadius = 2f;
        [SerializeField] private float brushStrength = 0.5f;
        [SerializeField] private Color paintColor = Color.red;

        private readonly VertexPicker picker = new VertexPicker();
        private readonly List<int> affected = new List<int>();
        private MeshRenderer target;

        public string ToolName => "Terrain Painting";

        public void OnEnable()
        {
        }

        public bool TryRaycast(out RaycastHit hit)
        {
            return SceneRaycaster.TryRaycast(Event.current.mousePosition, out hit);
        }

        public void RecordUndo(string label)
        {
            Mesh mesh = GetMesh();
            if (mesh != null)
            {
                Undo.RecordObject(mesh, label);
            }
        }

        public void OnInspectorGUI()
        {
            target = (MeshRenderer)EditorGUILayout.ObjectField("Mesh Renderer", target, typeof(MeshRenderer), true);
            brushRadius = EditorGUILayout.Slider("Brush Radius", brushRadius, 0.1f, 50f);
            brushStrength = EditorGUILayout.Slider("Brush Strength", brushStrength, 0f, 1f);
            paintColor = EditorGUILayout.ColorField("Paint Color", paintColor);

            if (target != null && GetMesh() == null)
            {
                EditorGUILayout.HelpBox("Target needs a MeshFilter with a mesh.", MessageType.Warning);
            }
        }

        public void OnSceneGUI()
        {
            Mesh mesh = GetMesh();
            if (mesh == null)
            {
                return;
            }

            Event e = Event.current;
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (!TryRaycast(out RaycastHit hit))
            {
                return;
            }

            Handles.color = paintColor;
            Handles.DrawWireDisc(hit.point, hit.normal, brushRadius);
            SceneView.RepaintAll();

            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0 && !e.alt)
            {
                Paint(mesh, hit.point);
                e.Use();
            }
        }

        private void Paint(Mesh mesh, Vector3 center)
        {
            picker.FindWithinRadius(mesh, target.transform, center, brushRadius, affected);
            if (affected.Count == 0)
            {
                return;
            }

            RecordUndo("Paint Vertex Colors");

            Color32[] colors = mesh.colors32;
            if (colors == null || colors.Length != mesh.vertexCount)
            {
                colors = new Color32[mesh.vertexCount];
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = Color.white;
                }
            }

            Color32 destination = paintColor;
            for (int i = 0; i < affected.Count; i++)
            {
                int index = affected[i];
                colors[index] = Color32.Lerp(colors[index], destination, brushStrength);
            }

            mesh.colors32 = colors;
        }

        private Mesh GetMesh()
        {
            if (target == null)
            {
                return null;
            }

            MeshFilter filter = target.GetComponent<MeshFilter>();
            return filter != null ? filter.sharedMesh : null;
        }
    }
}
