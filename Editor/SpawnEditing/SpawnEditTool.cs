using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WorldBuilder.Runtime.Data;

namespace WorldBuilder.Editor.SpawnEditing
{
    public sealed class SpawnEditTool : IWorldBuilderTool, IRaycastConsumer
    {
        [SerializeField] private GameObject spawnerPrefab;
        [SerializeField] private bool removeMode;
        [SerializeField] private float removeThreshold = 1f;

        private readonly ISpawnerSceneQuery query;

        public SpawnEditTool(ISpawnerSceneQuery query)
        {
            this.query = query;
        }

        public string ToolName => "Spawn Editing";

        public void OnEnable()
        {
        }

        public bool TryRaycast(out RaycastHit hit)
        {
            return SceneRaycaster.TryRaycast(Event.current.mousePosition, out hit);
        }

        public void OnInspectorGUI()
        {
            spawnerPrefab = (GameObject)EditorGUILayout.ObjectField("Spawner Prefab", spawnerPrefab, typeof(GameObject), false);
            removeMode = EditorGUILayout.Toggle("Remove Mode", removeMode);
            removeThreshold = EditorGUILayout.Slider("Remove Threshold", removeThreshold, 0.1f, 10f);

            if (spawnerPrefab != null && spawnerPrefab.GetComponent<ISpawner>() == null)
            {
                EditorGUILayout.HelpBox("Prefab must contain a component implementing ISpawner.", MessageType.Warning);
            }
        }

        public void OnSceneGUI()
        {
            DrawLabels();

            Event e = Event.current;
            if (e.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                if (!TryRaycast(out RaycastHit hit))
                {
                    return;
                }

                if (removeMode)
                {
                    RemoveNear(hit.point);
                }
                else
                {
                    Place(hit.point);
                }
                e.Use();
                SceneView.RepaintAll();
            }
        }

        private void DrawLabels()
        {
            IReadOnlyList<ISpawner> spawners = query.GetAll();
            for (int i = 0; i < spawners.Count; i++)
            {
                ISpawner spawner = spawners[i];
                Handles.Label(spawner.SpawnPosition + Vector3.up * 0.5f, "id:" + spawner.PrefabId);
            }
        }

        private void Place(Vector3 point)
        {
            if (spawnerPrefab == null || spawnerPrefab.GetComponent<ISpawner>() == null)
            {
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(spawnerPrefab);
            instance.transform.position = point;
            Undo.RegisterCreatedObjectUndo(instance, "Place Spawner");
        }

        private void RemoveNear(Vector3 point)
        {
            IReadOnlyList<ISpawner> spawners = query.GetAll();
            float best = float.MaxValue;
            ISpawner nearest = null;

            for (int i = 0; i < spawners.Count; i++)
            {
                float distance = Vector3.Distance(spawners[i].SpawnPosition, point);
                if (distance < best)
                {
                    best = distance;
                    nearest = spawners[i];
                }
            }

            if (nearest == null || best > removeThreshold)
            {
                return;
            }

            if (nearest is MonoBehaviour behaviour && behaviour != null)
            {
                Undo.DestroyObjectImmediate(behaviour.gameObject);
            }
        }
    }
}
