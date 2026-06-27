using UnityEngine.UIElements;

namespace WorldBuilder.Editor
{
    public sealed class WorldBuilderUIController
    {
        public void OnToolSelected(IWorldBuilderTool tool, VisualElement inspectorContainer)
        {
            inspectorContainer.Clear();

            if (tool == null)
            {
                return;
            }

            inspectorContainer.Add(tool.CreateInspectorGUI());
        }

        public void OnLanguageToggle(Label statusLabel)
        {
            WorldBuilderLocalization.Current =
                WorldBuilderLocalization.Current == WorldBuilderLocalization.Language.Korean
                    ? WorldBuilderLocalization.Language.English
                    : WorldBuilderLocalization.Language.Korean;

            statusLabel.text = WorldBuilderLocalization.Get("status.ready");
        }
    }
}
