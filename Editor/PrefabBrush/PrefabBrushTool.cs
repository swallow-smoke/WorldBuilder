using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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

        public string ToolName => WorldBuilderLocalization.Get("tool.prefabBrush");

        public Texture2D ToolIcon => null;

        public void OnEnable()
        {
        }

        public bool TryRaycast(out RaycastHit hit)
        {
            return SceneRaycaster.TryRaycast(Event.current.mousePosition, out hit);
        }

        public VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            root.Add(InspectorHelp.Build(ToolName, "help.prefabBrush"));

            Slider radius = new Slider("Brush Radius", 0.1f, 50f) { value = brushRadius };
            radius.RegisterValueChangedCallback(evt => brushRadius = evt.newValue);
            root.Add(radius);

            SliderInt density = new SliderInt("Brush Density", 1, 50) { value = brushDensity };
            density.RegisterValueChangedCallback(evt => brushDensity = evt.newValue);
            root.Add(density);

            FloatField min = new FloatField("Min Scale") { value = minScale };
            min.RegisterValueChangedCallback(evt => minScale = evt.newValue);
            root.Add(min);

            FloatField max = new FloatField("Max Scale") { value = maxScale };
            max.RegisterValueChangedCallback(evt => maxScale = evt.newValue);
            root.Add(max);

            Toggle align = new Toggle("Align To Normal") { value = alignToNormal };
            align.RegisterValueChangedCallback(evt => alignToNormal = evt.newValue);
            root.Add(align);

            Toggle erase = new Toggle("Erase Mode") { value = eraseMode };
            erase.RegisterValueChangedCallback(evt => eraseMode = evt.newValue);
            root.Add(erase);

            VisualElement list = new VisualElement();
            root.Add(list);

            Button add = new Button(() =>
            {
                prefabs.Add(null);
                RebuildList(list);
            })
            {
                text = "Add Prefab Slot"
            };
            root.Add(add);

            RebuildList(list);
            return root;
        }

        private void RebuildList(VisualElement list)
        {
            list.Clear();

            for (int i = 0; i < prefabs.Count; i++)
            {
                int index = i;

                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;

                ObjectField field = new ObjectField
                {
                    objectType = typeof(GameObject),
                    allowSceneObjects = false,
                    value = prefabs[index]
                };
                field.style.flexGrow = 1f;
                field.RegisterValueChangedCallback(evt => prefabs[index] = evt.newValue as GameObject);

                Button remove = new Button(() =>
                {
                    prefabs.RemoveAt(index);
                    RebuildList(list);
                })
                {
                    text = "X"
                };
                remove.style.width = 22f;

                row.Add(field);
                row.Add(remove);
                list.Add(row);
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
                UndoHistory.Push("Paint Prefab");
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
                    UndoHistory.Push("Erase Prefab");
                }
            }
        }
    }
}
