using UnityEditor;
using WorldBuilder.Editor.BinImporter;
using WorldBuilder.Editor.BiomeSetter;
using WorldBuilder.Editor.ChunkGridVisualizer;
using WorldBuilder.Editor.Export;
using WorldBuilder.Editor.HeightBiomeMapper;
using WorldBuilder.Editor.MeshEditing;
using WorldBuilder.Editor.PrefabBrush;
using WorldBuilder.Editor.SpawnEditing;
using WorldBuilder.Editor.SpawnHeatmap;
using WorldBuilder.Editor.TerrainPainting;
using WorldBuilder.Editor.UndoHistoryPanel;
using WorldBuilder.Editor.VoxelPainting;

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
            VoxelStoreAsset voxelStore = VoxelStoreLocator.LoadOrCreate();
            IChunkSerializeStrategy serializeStrategy = new BinaryChunkSerializeStrategy();
            ChunkSerializer serializer = new ChunkSerializer(serializeStrategy);
            SceneDataCollector collector = new SceneDataCollector(biomeMap, spawnerQuery, voxelStore);

            WorldBuilderToolRegistry.Register(new MeshEditTool());
            WorldBuilderToolRegistry.Register(new PrefabBrushTool());
            WorldBuilderToolRegistry.Register(new TerrainPaintTool());
            WorldBuilderToolRegistry.Register(new BiomeSetterTool(biomeMap, biomeProvider));
            WorldBuilderToolRegistry.Register(new SpawnEditTool(spawnerQuery));
            WorldBuilderToolRegistry.Register(new ExportTool(collector, serializer));

            WorldBuilderToolRegistry.Register(new VoxelPaintTool(voxelStore));
            WorldBuilderToolRegistry.Register(new ChunkGridVisualizerTool(biomeMap));
            WorldBuilderToolRegistry.Register(new HeightBiomeMapperTool(biomeMap));
            WorldBuilderToolRegistry.Register(new WorldBuilder.Editor.PathTool.PathTool());
            WorldBuilderToolRegistry.Register(new BinImporterTool(biomeMap, voxelStore));
            WorldBuilderToolRegistry.Register(new SpawnHeatmapTool(spawnerQuery));
            WorldBuilderToolRegistry.Register(new UndoHistoryTool());
        }
    }
}
