using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WorldBuilder.Editor
{
    public sealed class WorldBuilderWindow : EditorWindow
    {
        [SerializeField] private float leftPanelWidth = 180f;

        private int selectedIndex;
        private Vector2 toolListScroll;
        private Vector2 inspectorScroll;

        [MenuItem("WorldBuilder/Open")]
        public static void Open()
        {
            WorldBuilderWindow window = GetWindow<WorldBuilderWindow>();
            window.titleContent = new GUIContent("WorldBuilder");
            window.Show();
        }

        private void OnEnable()
        {
            IReadOnlyList<IWorldBuilderTool> tools = WorldBuilderToolRegistry.GetAll();
            for (int i = 0; i < tools.Count; i++)
            {
                tools[i].OnEnable();
            }

            SceneView.duringSceneGui += OnSceneGui;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGui;
        }

        private void OnGUI()
        {
            IReadOnlyList<IWorldBuilderTool> tools = WorldBuilderToolRegistry.GetAll();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawToolList(tools);
                DrawInspector(tools);
            }
        }

        private void OnSceneGui(SceneView sceneView)
        {
            IWorldBuilderTool tool = GetSelectedTool();
            if (tool == null)
            {
                return;
            }

            tool.OnSceneGUI();
        }

        private IWorldBuilderTool GetSelectedTool()
        {
            IReadOnlyList<IWorldBuilderTool> tools = WorldBuilderToolRegistry.GetAll();
            if (tools.Count == 0)
            {
                return null;
            }

            selectedIndex = Mathf.Clamp(selectedIndex, 0, tools.Count - 1);
            return tools[selectedIndex];
        }

        private void DrawToolList(IReadOnlyList<IWorldBuilderTool> tools)
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(leftPanelWidth)))
            {
                EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

                toolListScroll = EditorGUILayout.BeginScrollView(toolListScroll);
                for (int i = 0; i < tools.Count; i++)
                {
                    bool isSelected = i == selectedIndex;
                    if (GUILayout.Toggle(isSelected, tools[i].ToolName, EditorStyles.toolbarButton) && !isSelected)
                    {
                        selectedIndex = i;
                        SceneView.RepaintAll();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawInspector(IReadOnlyList<IWorldBuilderTool> tools)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (tools.Count == 0)
                {
                    EditorGUILayout.HelpBox("No tools registered.", MessageType.Info);
                    return;
                }

                selectedIndex = Mathf.Clamp(selectedIndex, 0, tools.Count - 1);
                IWorldBuilderTool tool = tools[selectedIndex];

                EditorGUILayout.LabelField(tool.ToolName, EditorStyles.boldLabel);

                inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll);
                tool.OnInspectorGUI();
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
