using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WorldBuilder.Editor.PrefabBrush
{
    public sealed class PrefabBrushTool : IWorldBuilderTool, IRaycastConsumer
    {
        [SerializeField] private float brushRadius = 3f;
        [SerializeField] private int brushDensity = 5;
        [SerializeField] private float minScale = 0.8f;
        [SerializeField] private float maxScale = 1.2f;
        [SerializeField] private bool alignToNormal = true;
        [SerializeField] private bool eraseMode;
        [SerializeField] private List<GameObject> prefabs = new List<GameObject>();

        public string ToolName => "Prefab Brush";

        public void OnEnable()
        {
        }

        public bool TryRaycast(out RaycastHit hit)
        {
            return SceneRaycaster.TryRaycast(Event.current.mousePosition, out hit);
        }

        public void OnInspectorGUI()
        {
            brushRadius = EditorGUILayout.Slider("Brush Radius", brushRadius, 0.1f, 50f);
            brushDensity = EditorGUILayout.IntSlider("Brush Density", brushDensity, 1, 50);
            minScale = EditorGUILayout.FloatField("Min Scale", minScale);
            maxScale = EditorGUILayout.FloatField("Max Scale", maxScale);
            alignToNormal = EditorGUILayout.Toggle("Align To Normal", alignToNormal);
            eraseMode = EditorGUILayout.Toggle("Erase Mode", eraseMode);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);

            int removeIndex = -1;
            for (int i = 0; i < prefabs.Count; i++)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    prefabs[i] = (GameObject)EditorGUILayout.ObjectField(prefabs[i], typeof(GameObject), false);
                    if (GUILayout.Button("X", GUILayout.Width(22f)))
                    {
                        removeIndex = i;
                    }
                }
            }

            if (removeIndex >= 0)
            {
                prefabs.RemoveAt(removeIndex);
            }

            if (GUILayout.Button("Add Prefab Slot"))
            {
                prefabs.Add(null);
            }
        }

        public void OnSceneGUI()
        {
            Event e = Event.current;
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (!TryRaycast(out RaycastHit hit))
            {
                return;
            }

            Handles.color = eraseMode ? Color.red : Color.green;
            Handles.DrawWireDisc(hit.point, hit.normal, brushRadius);
            SceneView.RepaintAll();

            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0 && !e.alt)
            {
                if (eraseMode)
                {
                    Erase(hit.point);
                }
                else
                {
                    Paint(hit);
                }
                e.Use();
            }
        }

        private void Paint(RaycastHit origin)
        {
            if (prefabs.Count == 0)
            {
                return;
            }

            for (int i = 0; i < brushDensity; i++)
            {
                GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
                if (prefab == null)
                {
                    continue;
                }

                Vector2 circle = Random.insideUnitCircle * brushRadius;
                Vector3 sample = origin.point + new Vector3(circle.x, 0f, circle.y);

                Vector3 position = sample;
                Vector3 normal = origin.normal;
                if (SceneRaycaster.TryRaycastDown(sample, out RaycastHit groundHit))
                {
                    position = groundHit.point;
                    normal = groundHit.normal;
                }

                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.position = position;

                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                if (alignToNormal)
                {
                    rotation = Quaternion.FromToRotation(Vector3.up, normal) * rotation;
                }
                instance.transform.rotation = rotation;
                instance.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);

                Undo.RegisterCreatedObjectUndo(instance, "Paint Prefab");
            }
        }

        private void Erase(Vector3 center)
        {
            GameObject[] all = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            float sqr = brushRadius * brushRadius;

            for (int i = 0; i < all.Length; i++)
            {
                GameObject go = all[i];
                if (!PrefabUtility.IsAnyPrefabInstanceRoot(go))
                {
                    continue;
                }

                if ((go.transform.position - center).sqrMagnitude <= sqr)
                {
                    Undo.DestroyObjectImmediate(go);
                }
            }
        }
    }
}
