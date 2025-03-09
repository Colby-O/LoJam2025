using System.Collections.Generic;
using UnityEngine;
using LoJam.Grid;
using LoJam.Interactable;
using LoJam.Core;
using static UnityEditor.PlayerSettings;

namespace LoJam.MonoSystem
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int _bounds;
        [SerializeField] private Vector2 _tileSize;

        [Header("Spawner Settings")]
        [SerializeField] private float _tickRate;
        [SerializeField, Range(0, 1)] private float _powerUpSpawnRate;

        [Header("Testing")]
        [SerializeField] private Vector2Int _player;

        private Tile[,] _tiles;

        private Tile _backgroundTile;
        private Tile _edgeTile;

        private float _lastTick;

        public Vector2 GetTileSize() {
            return _tileSize;
        }

        public Vector2Int WorldToGrid(Vector2 pos) {
            Vector2 clamped = pos / _tileSize;
            return new Vector2Int(Mathf.RoundToInt(clamped.x), Mathf.RoundToInt(clamped.y));
        }

        public Vector2 GridToWorld(Vector2Int pos) {
            return new Vector2(pos.x * _tileSize.x, pos.y * _tileSize.y);
        }

        private void GenerateMap()
        {
            ArrayHelpers.CreateAndFill(out _tiles, _bounds, (int x, int y) =>
            {
                Tile tile = null;

                if (x == 0 || y == 0 || x == _tiles.GetLength(1) - 1 || y == _tiles.GetLength(0) - 1)
                {
                    tile = Instantiate(
                        _edgeTile,
                        new Vector3(
                            GridToWorld(new Vector2Int(x - _tiles.GetLength(1) / 2, y - _tiles.GetLength(0) / 2)).x, 
                            GridToWorld(new Vector2Int(x - _tiles.GetLength(1) / 2, y - _tiles.GetLength(0) / 2)).y, 
                            0
                        ),
                        Quaternion.identity
                    );

                    tile.SetIsEdge(false);
                }
                else
                {
                    tile = Instantiate(
                        _backgroundTile,
                        new Vector3(
                            GridToWorld(new Vector2Int(x - _tiles.GetLength(1) / 2, y - _tiles.GetLength(0) / 2)).x, 
                            GridToWorld(new Vector2Int(x - _tiles.GetLength(1) / 2, y - _tiles.GetLength(0) / 2)).y, 
                            0
                        ),
                        Quaternion.identity
                    );

                    tile.SetIsEdge(true);
                }

                tile.transform.parent = transform;
                return tile;
            });

            _tiles[5, 5].SetInteractable(new TestItem());
        }

        // This will take a reference to the player script which will be passed to OnPlayerEnter and OnPlayerAdjancent from IInteractable Interface.
        // For testing until player scripts are ready it's taking a vec2
        private void CheckPlayer(Vector2Int pos)
        {
            Tile[,] region = ArrayHelpers.ExtractRegion(_tiles, pos, 3);

            for (int i = 0; i < region.GetLength(0); i++)
            {
                for (int j = 0; j < region.GetLength(1); j++)
                {
                    if (i == 1 && j == 1)
                    {
                        if (
                            region[j, i] != null && 
                            !region[j, i].IsEdge() && 
                            region[j, i].HasInteractable()
                        ) region[j, i].GetInteractable().OnPlayerEnter(/* Player */);
                    }
                    else
                    {
                        if (
                            region[j, i] != null && 
                            !region[j, i].IsEdge() && 
                            region[j, i].HasInteractable()
                       ) region[j, i].GetInteractable().OnPlayerAdjancent(/* Player */);
                    }
                }
            }
        }

        private void Tick()
        {
            _lastTick = Time.time;

            if (Random.value > _powerUpSpawnRate)
            {
                Debug.Log("Spawning Powerup");
            }
        }

        private void Awake()
        {
            _backgroundTile = Resources.Load<Tile>("Tiles/Background");
            _edgeTile = Resources.Load<Tile>("Tiles/Edge");

            GenerateMap();
        }

        private void Update()
        {
            // Should be ran for each player
            CheckPlayer(_player);
        }

        private void FixedUpdate()
        {
            if (Time.time > _lastTick + _tickRate) Tick();
        }
    }
}
