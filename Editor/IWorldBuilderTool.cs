namespace WorldBuilder.Editor
{
    public interface IWorldBuilderTool
    {
        string ToolName { get; }

        void OnEnable();
        void OnSceneGUI();
        void OnInspectorGUI();
    }
}
