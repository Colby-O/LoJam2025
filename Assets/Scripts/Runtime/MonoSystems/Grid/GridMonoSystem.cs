using System.Collections.Generic;
using UnityEngine;
using LoJam.Grid;
using LoJam.Interactable;

namespace LoJam.MonoSystem
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int _bounds;
        [SerializeField] private Vector2 _gridSize;

        [Header("Testing")]
        [SerializeField] private Vector2Int _player;

        private Dictionary<Vector2Int, Tile> _tiles;

        public Vector2 GetGridSize() {
            return _gridSize;
        }

        public Vector2Int WorldToGrid(Vector2 pos) {
            Vector2 clamped = pos / _gridSize;
            return new Vector2Int(Mathf.RoundToInt(clamped.x), Mathf.RoundToInt(clamped.y));
        }

        public Vector2 GridToWorld(Vector2Int pos) {
            return new Vector2(pos.x * _gridSize.x, pos.y * _gridSize.y);
        }

        private void GenerateMap() {
             _tiles = new Dictionary<Vector2Int, Tile>();

            for (int i = 0; i < _bounds.x; i++) {
                for (int j = 0; j < _bounds.y; j++) {
                    _tiles.Add(new Vector2Int(j, i), new Tile());
                }
            }

            _tiles[new Vector2Int(5, 5)].interactable = new TestItem();
        }

        public void Awake()
        {
            GenerateMap();
        }

        public void Update()
        {
            
        }
    }
}
