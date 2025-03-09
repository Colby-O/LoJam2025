using LoJam.Core;
using UnityEngine;

namespace LoJam.MonoSystem
{
    public interface IGridMonoSystem : IMonoSystem
    {
        public Vector2 GetGridSize();
        public Vector2Int WorldToGrid(Vector2 pos);
        public Vector2 GridToWorld(Vector2Int pos);
    }
}
