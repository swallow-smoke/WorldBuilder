# WorldBuilder

Unity 6 URP 전용 씬 편집 패키지.

---

## 설치

1. `Packages/manifest.json` 열기
2. `dependencies`에 추가:
```json
"com.emiteat.worldbuilder": "file:../packages/com.emiteat/worldbuilder"
```
3. Unity 에디터 포커스 → 자동 import

---

## 실행

메뉴바 → `WorldBuilder > Open`

좌측 패널에서 툴 선택 → 우측 패널에서 설정

---

## 툴 목록

### MeshEditing
- 씬에서 MeshFilter 선택
- 버텍스가 핸들로 표시됨
- 핸들 드래그 → 버텍스 위치 실시간 수정
- 박스 드래그 → 다중 버텍스 선택
- 모든 조작 Undo 지원 (`Ctrl+Z`)

### PrefabBrush
- InspectorGUI에서 프리팹 리스트 등록
- 브러시 반경 / 밀도 슬라이더로 조절
- 씬에서 마우스 드래그 → 프리팹 자동 배치
- 랜덤 Y rotation / scale 적용
- 표면 법선 정렬 토글 지원
- Erase 모드: 브러시 반경 내 프리팹 제거

### TerrainPainting
- MeshRenderer (버텍스 컬러) 대상
- 컬러 피커로 색상 선택
- 씬에서 마우스 드래그 → 버텍스 컬러 페인팅
- 브러시 강도 조절 가능
- 브러시 범위 SceneGUI에 원형으로 미리보기

### BiomeSetter
- InspectorGUI에서 BiomeType 선택 (Ocean / Beach / Forest / Rocky / DeepSea)
- 씬 클릭 → 해당 청크에 BiomeType 할당
- 할당된 청크는 BiomeData 색상으로 SceneGUI에 시각화
- 청크 크기 조절 가능 (기본값 16)

### SpawnEditing
- ISpawner 구현 오브젝트를 씬에 배치
- 씬 클릭 → 해당 위치에 스포너 배치
- 모든 ISpawner에 prefabId 라벨 표시
- Remove 모드: 기존 스포너 클릭 → 제거

### Export
- 씬 전체 데이터를 `.bin`으로 직렬화
- 포함 데이터: ChunkData (position, BiomeType, SpawnData[])
- 출력 경로: `Assets/WorldBuilder/Export/world.bin`
- Export 버튼 클릭 → 진행률 표시 → 완료 후 AssetDatabase 자동 갱신

---

## 새 툴 추가 방법

1. `IWorldBuilderTool` 구현하는 클래스 생성
2. `WorldBuilderBootstrap.cs`에 한 줄 등록
3. 끝 — 다른 파일 수정 없음

---

## Runtime Data 구조

| 파일 | 타입 | 설명 |
|---|---|---|
| `BiomeType.cs` | enum | 바이옴 종류 |
| `BiomeData.cs` | ScriptableObject | 바이옴 메타데이터 (색상 등) |
| `VoxelData.cs` | struct | Marching Cubes용 밀도 배열 |
| `SpawnData.cs` | struct | 스폰 위치/회전/스케일 |
| `ChunkData.cs` | struct | 청크 단위 전체 데이터 |

---

## 주의사항

- TerrainPainting은 버텍스 컬러를 지원하는 메시에서만 동작
- VoxelData Export는 현재 빈 기본값으로 직렬화됨 (Marching Cubes 연동 후 확장 예정)
- BiomeSetter 데이터는 씬 세션 중 메모리에만 존재 → Export 전에 반드시 저장
