using LoJam.Core;
using LoJam.Grid;
using UnityEngine;

namespace LoJam.MonoSystem
{
    public interface IGridMonoSystem : IMonoSystem
    {
        public Vector2Int GetNumberOfTile();
        public Vector2 GetTileSize();
        public Vector2Int GetBounds();
        public Tile GetTileAt(int x, int y);
        public Tile GetTileAt(Vector2Int pos);
        public Vector2Int WorldToGrid(Vector2 pos);
        public Vector2 GridToWorld(Vector2Int pos);
    }
}
