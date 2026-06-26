using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WorldBuilder.Editor.MeshEditing
{
    public sealed class MeshEditTool : IWorldBuilderTool, IUndoable
    {
        [SerializeField] private float handleSize = 0.04f;

        private readonly MeshSelector selector = new MeshSelector();
        private readonly HashSet<int> selection = new HashSet<int>();
        private Vector3[] vertices;
        private Mesh activeMesh;
        private bool isBoxSelecting;
        private Vector2 boxStart;
        private Vector2 boxEnd;

        public string ToolName => "Mesh Editing";

        public void OnEnable()
        {
            Release();
        }

        public void RecordUndo(string label)
        {
            if (activeMesh != null)
            {
                Undo.RecordObject(activeMesh, label);
            }
        }

        public void OnInspectorGUI()
        {
            selector.DrawSelector();
            handleSize = EditorGUILayout.Slider("Handle Size", handleSize, 0.005f, 0.3f);

            if (!selector.HasMesh)
            {
                EditorGUILayout.HelpBox("Select a MeshFilter with a mesh. Shift-drag in the scene to box-select vertices.", MessageType.Info);
                Release();
                return;
            }

            if (selector.Mesh != activeMesh)
            {
                LoadMesh(selector.Mesh);
            }

            EditorGUILayout.LabelField("Vertices", vertices != null ? vertices.Length.ToString() : "0");
            EditorGUILayout.LabelField("Selected", selection.Count.ToString());

            if (GUILayout.Button("Clear Selection"))
            {
                selection.Clear();
                SceneView.RepaintAll();
            }
        }

        public void OnSceneGUI()
        {
            if (vertices == null || activeMesh == null || selector.Target == null)
            {
                return;
            }

            Transform t = selector.Target.transform;

            HandleBoxSelection(t);

            if (selection.Count > 0)
            {
                DrawGroupHandle(t);
            }
            else
            {
                DrawPerVertexHandles(t);
            }

            DrawSelectionMarkers(t);
        }

        private void DrawPerVertexHandles(Transform t)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 world = t.TransformPoint(vertices[i]);
                float size = HandleUtility.GetHandleSize(world) * handleSize * 4f;

                EditorGUI.BeginChangeCheck();
                Vector3 moved = Handles.FreeMoveHandle(world, size, Vector3.zero, Handles.DotHandleCap);
                if (EditorGUI.EndChangeCheck())
                {
                    RecordUndo("Move Vertex");
                    vertices[i] = t.InverseTransformPoint(moved);
                    ApplyVertices();
                }
            }
        }

        private void DrawGroupHandle(Transform t)
        {
            Vector3 centroidLocal = Vector3.zero;
            foreach (int i in selection)
            {
                centroidLocal += vertices[i];
            }
            centroidLocal /= selection.Count;

            Vector3 world = t.TransformPoint(centroidLocal);
            float size = HandleUtility.GetHandleSize(world) * handleSize * 6f;

            EditorGUI.BeginChangeCheck();
            Vector3 moved = Handles.FreeMoveHandle(world, size, Vector3.zero, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                RecordUndo("Move Vertices");
                Vector3 deltaLocal = t.InverseTransformPoint(moved) - centroidLocal;
                foreach (int i in selection)
                {
                    vertices[i] += deltaLocal;
                }
                ApplyVertices();
            }
        }

        private void DrawSelectionMarkers(Transform t)
        {
            Handles.color = Color.yellow;
            foreach (int i in selection)
            {
                Vector3 world = t.TransformPoint(vertices[i]);
                float size = HandleUtility.GetHandleSize(world) * handleSize * 2f;
                Handles.DotHandleCap(0, world, Quaternion.identity, size, EventType.Repaint);
            }
        }

        private void HandleBoxSelection(Transform t)
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
            {
                isBoxSelecting = true;
                boxStart = e.mousePosition;
                boxEnd = e.mousePosition;
                e.Use();
            }
            else if (isBoxSelecting && e.type == EventType.MouseDrag)
            {
                boxEnd = e.mousePosition;
                e.Use();
            }
            else if (isBoxSelecting && e.type == EventType.MouseUp)
            {
                boxEnd = e.mousePosition;
                ApplyBoxSelection(t);
                isBoxSelecting = false;
                e.Use();
            }

            if (isBoxSelecting)
            {
                Handles.BeginGUI();
                Rect rect = GetBoxRect();
                Handles.DrawSolidRectangleWithOutline(rect, new Color(0.3f, 0.6f, 1f, 0.15f), new Color(0.3f, 0.6f, 1f, 0.8f));
                Handles.EndGUI();
                SceneView.RepaintAll();
            }
        }

        private void ApplyBoxSelection(Transform t)
        {
            Rect rect = GetBoxRect();
            selection.Clear();

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 world = t.TransformPoint(vertices[i]);
                Vector2 gui = HandleUtility.WorldToGUIPoint(world);
                if (rect.Contains(gui))
                {
                    selection.Add(i);
                }
            }
        }

        private Rect GetBoxRect()
        {
            float x = Mathf.Min(boxStart.x, boxEnd.x);
            float y = Mathf.Min(boxStart.y, boxEnd.y);
            float w = Mathf.Abs(boxStart.x - boxEnd.x);
            float h = Mathf.Abs(boxStart.y - boxEnd.y);
            return new Rect(x, y, w, h);
        }

        private void ApplyVertices()
        {
            activeMesh.vertices = vertices;
            activeMesh.RecalculateNormals();
            activeMesh.RecalculateBounds();
        }

        private void LoadMesh(Mesh mesh)
        {
            activeMesh = mesh;
            vertices = mesh.vertices;
            selection.Clear();
        }

        private void Release()
        {
            activeMesh = null;
            vertices = null;
            selection.Clear();
            isBoxSelecting = false;
        }
    }
}
