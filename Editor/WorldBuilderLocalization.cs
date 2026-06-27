using System.Collections.Generic;
using UnityEditor;

namespace WorldBuilder.Editor
{
    public static class WorldBuilderLocalization
    {
        public enum Language
        {
            Korean,
            English
        }

        private const string PrefKey = "WB_Language";

        private static readonly Dictionary<string, string> KoreanTable = new Dictionary<string, string>
        {
            { "tool.meshEdit", "메시 편집" },
            { "tool.prefabBrush", "프리팹 브러시" },
            { "tool.terrainPaint", "지형 페인팅" },
            { "tool.biomeSetter", "바이옴 설정" },
            { "tool.spawnEdit", "스폰 편집" },
            { "tool.export", "내보내기" },
            { "tool.voxelPaint", "복셀 페인팅" },
            { "tool.chunkGrid", "청크 그리드" },
            { "tool.heightBiome", "높이 바이옴" },
            { "tool.path", "경로" },
            { "tool.binImport", "가져오기" },
            { "tool.heatmap", "히트맵" },
            { "tool.undoHistory", "실행 취소 기록" },
            { "tool.airPocket", "에어포켓" },
            { "tool.waterCurrent", "수중 조류" },
            { "tool.bioluminescence", "생물발광" },
            { "tool.pressureZone", "수압 존" },
            { "tool.temperatureZone", "온도 존" },
            { "tool.toxicZone", "독성 존" },
            { "tool.visibilityZone", "가시거리 존" },
            { "tool.wreckage", "잔해 배치" },
            { "tool.creatureSpawn", "크리처 스폰" },
            { "tool.eventTrigger", "이벤트 트리거" },
            { "tool.depthLayer", "수심 레이어" },
            { "tool.envOverlay", "환경 오버레이" },
            { "tool.materialBatch", "머티리얼 일괄" },
            { "header.title", "월드 빌더" },
            { "status.ready", "준비" },
            { "btn.export", "내보내기" },
            { "btn.load", "불러오기" },
            { "btn.apply", "적용" },
            { "btn.clear", "초기화" },
            { "help.title", "사용법" },
            { "help.meshEdit", "MeshFilter를 지정한 뒤 씬에서 정점을 드래그해 편집합니다. Shift+드래그로 박스 선택, 선택된 정점은 그룹으로 함께 이동합니다." },
            { "help.prefabBrush", "프리팹 목록을 채우고 씬을 클릭/드래그하면 브러시 반경 안에 프리팹을 분산 배치합니다. Erase 모드는 반경 내 프리팹 인스턴스를 삭제합니다." },
            { "help.terrainPaint", "MeshRenderer를 지정하고 씬을 칠하면 정점 컬러를 브러시로 섞습니다. 반경/강도/색을 조절하세요." },
            { "help.biomeSetter", "바이옴과 청크 크기를 정한 뒤 씬을 클릭해 청크에 바이옴을 지정합니다. Erase 모드는 지정을 해제합니다." },
            { "help.spawnEdit", "ISpawner 프리팹을 지정하고 씬을 클릭해 스포너를 배치합니다. Remove 모드는 임계 거리 내 가장 가까운 스포너를 제거합니다." },
            { "help.export", "현재 씬의 청크/스폰/복셀 데이터를 StreamingAssets/WorldBuilder/world.bin으로 내보냅니다." },
            { "help.voxelPaint", "모드(추가/감소/평활)를 고르고 씬을 칠해 복셀 밀도를 편집합니다. 결과는 복셀 스토어에 저장되어 내보내기에 반영됩니다." },
            { "help.chunkGrid", "씬 뷰 피벗 주변 반경의 청크 격자를 표시합니다. 바이옴이 지정된 청크는 초록, 나머지는 회색입니다." },
            { "help.heightBiome", "고도→바이옴 곡선을 설정하고 적용을 누르면 각 청크 높이에 따라 바이옴을 자동 할당합니다." },
            { "help.path", "컨트롤 포인트를 추가하고 씬 핸들로 드래그하면 대상 MeshFilter에 경로 메시가 생성됩니다. 너비/세그먼트를 조절하세요." },
            { "help.binImport", "world.bin을 읽어 바이옴·복셀을 복원합니다. PrefabRegistry를 지정하면 스폰 오브젝트도 함께 복원됩니다." },
            { "help.heatmap", "씬의 스포너 밀도를 색으로 표시합니다(파랑=낮음, 빨강=높음). 반경과 최대 밀도를 조절하세요." },
            { "help.undoHistory", "기록된 작업 라벨 목록입니다. 항목을 클릭하면 실행 취소하고, 초기화로 기록을 비웁니다." },
            { "help.airPocket", "크기와 라벨을 정하고 씬을 클릭해 에어포켓 볼륨을 배치합니다. 배치된 볼륨은 와이어 큐브로 표시되며 핸들로 위치를 옮길 수 있습니다." },
            { "help.waterCurrent", "강도를 정하고 씬에서 드래그하면 드래그 방향으로 조류 벡터가 설정됩니다. 화살표로 방향을 시각화합니다." },
            { "help.bioluminescence", "반경/강도/색상을 정하고 씬을 클릭해 생물발광 존을 배치합니다. 설정 색상의 와이어 디스크로 표시됩니다." },
            { "help.pressureZone", "반경/압력/데미지를 정하고 씬을 클릭해 수압 존을 배치합니다. 빨간 와이어 디스크로 표시됩니다." },
            { "help.temperatureZone", "온도와 반경을 정하고 씬을 클릭해 온도 존을 배치합니다. 양수는 주황, 0 이하는 파란 디스크로 표시됩니다." },
            { "help.toxicZone", "반경/강도/데미지를 정하고 씬을 클릭해 독성 수역을 배치합니다. 초록 와이어 디스크로 표시됩니다." },
            { "help.visibilityZone", "반경/가시거리/안개 색을 정하고 씬을 클릭해 가시거리 존을 배치합니다. 회색 반투명 디스크로 표시됩니다." },
            { "help.wreckage", "프리팹과 LOG 번호를 정하고 씬을 클릭해 잔해를 배치합니다. LOG 번호가 있으면 LOG_XXX 라벨이 표시됩니다." },
            { "help.creatureSpawn", "바이옴/프리팹 ID/밀도/반경을 정하고 씬을 클릭해 크리처 스폰 존을 배치합니다. 바이옴 색상으로 디스크가 표시됩니다." },
            { "help.eventTrigger", "이벤트 ID/반경/1회성을 정하고 씬을 클릭해 트리거 존을 배치합니다. 노란 디스크와 이벤트 ID 라벨로 표시됩니다." },
            { "help.depthLayer", "얕은물/중간/심해 깊이와 색을 정하면 씬 뷰 피벗 기준으로 수심 레이어 평면을 시각화합니다." },
            { "help.envOverlay", "오버레이 타입과 투명도를 정하면 환경 존들을 강도 기반 색상 그라디언트로 오버레이 표시합니다." },
            { "help.materialBatch", "탭으로 머티리얼 교체, 셰이더 교체, 프로퍼티 일괄 수정, 바이옴 머티리얼 프리셋을 수행합니다. 모든 작업은 실행 취소를 지원합니다." }
        };

        private static readonly Dictionary<string, string> EnglishTable = new Dictionary<string, string>
        {
            { "tool.meshEdit", "Mesh Edit" },
            { "tool.prefabBrush", "Prefab Brush" },
            { "tool.terrainPaint", "Terrain Painting" },
            { "tool.biomeSetter", "Biome Setter" },
            { "tool.spawnEdit", "Spawn Edit" },
            { "tool.export", "Export" },
            { "tool.voxelPaint", "Voxel Painting" },
            { "tool.chunkGrid", "Chunk Grid" },
            { "tool.heightBiome", "Height Biome" },
            { "tool.path", "Path" },
            { "tool.binImport", "Import" },
            { "tool.heatmap", "Heatmap" },
            { "tool.undoHistory", "Undo History" },
            { "tool.airPocket", "Air Pocket" },
            { "tool.waterCurrent", "Water Current" },
            { "tool.bioluminescence", "Bioluminescence" },
            { "tool.pressureZone", "Pressure Zone" },
            { "tool.temperatureZone", "Temperature Zone" },
            { "tool.toxicZone", "Toxic Zone" },
            { "tool.visibilityZone", "Visibility Zone" },
            { "tool.wreckage", "Wreckage" },
            { "tool.creatureSpawn", "Creature Spawn" },
            { "tool.eventTrigger", "Event Trigger" },
            { "tool.depthLayer", "Depth Layer" },
            { "tool.envOverlay", "Environment Overlay" },
            { "tool.materialBatch", "Material Batch" },
            { "header.title", "World Builder" },
            { "status.ready", "Ready" },
            { "btn.export", "Export" },
            { "btn.load", "Load" },
            { "btn.apply", "Apply" },
            { "btn.clear", "Clear" },
            { "help.title", "Usage" },
            { "help.meshEdit", "Assign a MeshFilter, then drag vertices in the scene to edit. Shift-drag to box-select; selected vertices move together as a group." },
            { "help.prefabBrush", "Fill the prefab list and click/drag in the scene to scatter prefabs within the brush radius. Erase mode deletes prefab instances inside the radius." },
            { "help.terrainPaint", "Assign a MeshRenderer and paint in the scene to blend vertex colors with the brush. Adjust radius, strength and color." },
            { "help.biomeSetter", "Pick a biome and chunk size, then click in the scene to assign a biome to a chunk. Erase mode removes the assignment." },
            { "help.spawnEdit", "Assign an ISpawner prefab and click in the scene to place spawners. Remove mode deletes the nearest spawner within the threshold distance." },
            { "help.export", "Exports the current scene's chunk/spawn/voxel data to StreamingAssets/WorldBuilder/world.bin." },
            { "help.voxelPaint", "Pick a mode (Add/Subtract/Smooth) and paint in the scene to edit voxel density. Results are saved to the voxel store and included in export." },
            { "help.chunkGrid", "Shows the chunk grid within a radius around the scene view pivot. Chunks with an assigned biome are green, others gray." },
            { "help.heightBiome", "Set a height-to-biome curve and press Apply to auto-assign biomes by each chunk's height." },
            { "help.path", "Add control points and drag the scene handles to generate a path mesh on the target MeshFilter. Adjust width and segments." },
            { "help.binImport", "Reads world.bin to restore biomes and voxels. Assign a PrefabRegistry to also restore spawned objects." },
            { "help.heatmap", "Visualizes spawner density by color (blue = low, red = high). Adjust radius and max density." },
            { "help.undoHistory", "A list of recorded operation labels. Click an entry to undo it; Clear empties the history." },
            { "help.airPocket", "Set size and label, then click in the scene to place an air pocket volume. Placed volumes show as wire cubes and can be moved with a handle." },
            { "help.waterCurrent", "Set strength and drag in the scene to define the current direction along the drag. The direction is visualized with an arrow." },
            { "help.bioluminescence", "Set radius, intensity and color, then click in the scene to place a bioluminescence zone, shown as a wire disc in the chosen color." },
            { "help.pressureZone", "Set radius, pressure and damage, then click in the scene to place a pressure zone, shown as a red wire disc." },
            { "help.temperatureZone", "Set temperature and radius, then click in the scene to place a temperature zone. Positive values are orange, zero or below are blue." },
            { "help.toxicZone", "Set radius, intensity and damage, then click in the scene to place a toxic zone, shown as a green wire disc." },
            { "help.visibilityZone", "Set radius, visibility and fog color, then click in the scene to place a visibility zone, shown as a translucent gray disc." },
            { "help.wreckage", "Set a prefab and LOG number, then click in the scene to place wreckage. A LOG_XXX label appears when a number is set." },
            { "help.creatureSpawn", "Set biome, prefab id, density and radius, then click in the scene to place a creature spawn zone, drawn in the biome color." },
            { "help.eventTrigger", "Set event id, radius and one-shot, then click in the scene to place a trigger zone, shown as a yellow disc with the event id label." },
            { "help.depthLayer", "Set shallow/mid/deep depths and colors to visualize depth layer planes around the scene view pivot." },
            { "help.envOverlay", "Pick an overlay type and opacity to overlay environment zones with an intensity-based color gradient." },
            { "help.materialBatch", "Use the tabs to replace materials, replace shaders, batch-edit properties, and manage biome material presets. All operations support undo." }
        };

        private static Language current = (Language)EditorPrefs.GetInt(PrefKey, (int)Language.Korean);

        public static Language Current
        {
            get => current;
            set
            {
                current = value;
                EditorPrefs.SetInt(PrefKey, (int)value);
            }
        }

        public static string Get(string key)
        {
            Dictionary<string, string> table = current == Language.Korean ? KoreanTable : EnglishTable;
            return table.TryGetValue(key, out string value) ? value : key;
        }
    }
}
