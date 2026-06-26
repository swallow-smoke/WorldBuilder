using UnityEngine;

namespace WorldBuilder.Editor
{
    public sealed class ChunkCoordCalculator
    {
        public Vector3Int ToChunkCoord(Vector3 worldPoint, float chunkSize)
        {
            float size = Mathf.Max(0.0001f, chunkSize);
            return new Vector3Int(
                Mathf.FloorToInt(worldPoint.x / size),
                Mathf.FloorToInt(worldPoint.y / size),
                Mathf.FloorToInt(worldPoint.z / size));
        }

        public Vector3 ToWorldOrigin(Vector3Int coord, float chunkSize)
        {
            return new Vector3(coord.x * chunkSize, coord.y * chunkSize, coord.z * chunkSize);
        }

        public Vector3 ToWorldCenter(Vector3Int coord, float chunkSize)
        {
            return new Vector3(
                (coord.x + 0.5f) * chunkSize,
                (coord.y + 0.5f) * chunkSize,
                (coord.z + 0.5f) * chunkSize);
        }
    }
}
