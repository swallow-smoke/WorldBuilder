using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace WorldBuilder.Editor
{
    public sealed class WorldBuilderWindow : EditorWindow
    {
        private const string UIPath = "Packages/com.emiteat.worldbuilder/Editor/UI/";

        private WorldBuilderUIController controller;
        private VisualTreeAsset itemAsset;

        private Label titleLabel;
        private Label statusLabel;
        private Button languageToggle;
        private ScrollView toolList;
        private VisualElement inspectorContainer;

        private IWorldBuilderTool selectedTool;
        private Button selectedButton;

        [MenuItem("WorldBuilder/Open")]
        public static void Open()
        {
            WorldBuilderWindow window = GetWindow<WorldBuilderWindow>();
            window.titleContent = new GUIContent("WorldBuilder");
            window.minSize = new Vector2(600f, 400f);
            window.Show();
        }

        public void CreateGUI()
        {
            controller = new WorldBuilderUIController();

            VisualTreeAsset tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UIPath + "WorldBuilderWindow.uxml");
            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(UIPath + "WorldBuilderWindow.uss");
            itemAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UIPath + "ToolListItem.uxml");

            tree.CloneTree(rootVisualElement);
            rootVisualElement.styleSheets.Add(style);

            titleLabel = rootVisualElement.Q<Label>("title");
            statusLabel = rootVisualElement.Q<Label>("status-label");
            languageToggle = rootVisualElement.Q<Button>("language-toggle");
            toolList = rootVisualElement.Q<ScrollView>("tool-list");
            inspectorContainer = rootVisualElement.Q<VisualElement>("inspector-container");

            languageToggle.clicked += OnLanguageToggleClicked;

            EnableTools();
            Rebuild();

            rootVisualElement.schedule.Execute(UpdateStatus).Every(500);
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGui;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGui;
        }

        private void OnSceneGui(SceneView sceneView)
        {
            if (selectedTool != null)
            {
                selectedTool.OnSceneGUI();
            }
        }

        private void EnableTools()
        {
            IReadOnlyList<IWorldBuilderTool> tools = WorldBuilderToolRegistry.GetAll();
            for (int i = 0; i < tools.Count; i++)
            {
                tools[i].OnEnable();
            }
        }

        private void Rebuild()
        {
            titleLabel.text = WorldBuilderLocalization.Get("header.title");
            languageToggle.text = WorldBuilderLocalization.Current == WorldBuilderLocalization.Language.Korean ? "KO" : "EN";

            BuildToolList();

            IReadOnlyList<IWorldBuilderTool> tools = WorldBuilderToolRegistry.GetAll();
            if (selectedTool == null && tools.Count > 0)
            {
                selectedTool = tools[0];
            }

            if (selectedTool != null)
            {
                SelectTool(selectedTool);
            }
            else
            {
                statusLabel.text = WorldBuilderLocalization.Get("status.ready");
            }
        }

        private void BuildToolList()
        {
            toolList.Clear();
            selectedButton = null;

            IReadOnlyList<IWorldBuilderTool> tools = WorldBuilderToolRegistry.GetAll();
            for (int i = 0; i < tools.Count; i++)
            {
                IWorldBuilderTool tool = tools[i];
                VisualElement item = itemAsset.Instantiate();
                Button button = item.Q<Button>("tool-button");
                Image icon = item.Q<Image>("tool-icon");
                Label label = item.Q<Label>("tool-name");

                icon.image = tool.ToolIcon;
                label.text = tool.ToolName;
                button.userData = tool;
                button.clicked += () => SelectTool(tool);

                toolList.Add(item);
            }
        }

        private void SelectTool(IWorldBuilderTool tool)
        {
            selectedTool = tool;
            controller.OnToolSelected(tool, inspectorContainer);

            if (selectedButton != null)
            {
                selectedButton.RemoveFromClassList("selected");
            }

            selectedButton = FindButton(tool);
            if (selectedButton != null)
            {
                selectedButton.AddToClassList("selected");
            }

            statusLabel.text = tool.ToolName;
        }

        private Button FindButton(IWorldBuilderTool tool)
        {
            List<Button> buttons = toolList.Query<Button>("tool-button").ToList();
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].userData == tool)
                {
                    return buttons[i];
                }
            }

            return null;
        }

        private void OnLanguageToggleClicked()
        {
            controller.OnLanguageToggle(statusLabel);
            Rebuild();
        }

        private void UpdateStatus()
        {
            if (selectedTool == null)
            {
                statusLabel.text = WorldBuilderLocalization.Get("status.ready");
                return;
            }

            IReadOnlyList<string> history = UndoHistory.Entries;
            if (history.Count > 0)
            {
                statusLabel.text = selectedTool.ToolName + "  |  " + history[history.Count - 1];
            }
            else
            {
                statusLabel.text = selectedTool.ToolName;
            }
        }
    }
}
