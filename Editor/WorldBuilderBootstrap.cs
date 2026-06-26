using UnityEditor;
using WorldBuilder.Editor.BiomeSetter;
using WorldBuilder.Editor.Export;
using WorldBuilder.Editor.MeshEditing;
using WorldBuilder.Editor.PrefabBrush;
using WorldBuilder.Editor.SpawnEditing;
using WorldBuilder.Editor.TerrainPainting;

namespace WorldBuilder.Editor
{
    [InitializeOnLoad]
    public static class WorldBuilderBootstrap
    {
        static WorldBuilderBootstrap()
        {
            IChunkBiomeMap biomeMap = new ChunkBiomeMap();
            IBiomeDataProvider biomeProvider = new EditorBiomeDataProvider();
            ISpawnerSceneQuery spawnerQuery = new SpawnerSceneQuery();
            IChunkSerializeStrategy serializeStrategy = new BinaryChunkSerializeStrategy();
            ChunkSerializer serializer = new ChunkSerializer(serializeStrategy);
            SceneDataCollector collector = new SceneDataCollector(biomeMap, spawnerQuery);

            WorldBuilderToolRegistry.Register(new MeshEditTool());
            WorldBuilderToolRegistry.Register(new PrefabBrushTool());
            WorldBuilderToolRegistry.Register(new TerrainPaintTool());
            WorldBuilderToolRegistry.Register(new BiomeSetterTool(biomeMap, biomeProvider));
            WorldBuilderToolRegistry.Register(new SpawnEditTool(spawnerQuery));
            WorldBuilderToolRegistry.Register(new ExportTool(collector, serializer));
        }
    }
}
