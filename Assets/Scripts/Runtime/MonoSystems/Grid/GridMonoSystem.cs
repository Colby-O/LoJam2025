using System.Collections.Generic;
using UnityEngine;
using LoJam.Grid;
using LoJam.Interactable;
using LoJam.Core;

namespace LoJam.MonoSystem
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int _bounds;
        [SerializeField] private Vector2 _tileSize;

        [Header("Spawner Settings")]
        [SerializeField] private SpawnerSettings _spawnerSettings;

        [Header("Testing")]
        [SerializeField] private Vector2Int _player;

        private Tile[,] _tiles;

        private Tile _backgroundTile;
        private Tile _edgeTile;

        private PoissonSampler _sampler;

        private float _lastTick;

        // Testing Varible
        private GameObject _playerTest;

        public Vector2Int GetNumberOfTile() => new Vector2Int(Mathf.RoundToInt(_bounds.x / _tileSize.x), Mathf.RoundToInt(_bounds.y / _tileSize.y));

        public Vector2 GetTileSize() => _tileSize;

        public Vector2Int GetBounds() => _bounds;

        public Tile GetTileAt(int x, int y) => _tiles[y, x];

        public Tile GetTileAt(Vector2Int pos) => GetTileAt(pos.x, pos.y);

        public Vector2Int WorldToGrid(Vector2 pos) {
            Vector2 clamped = pos / _tileSize;
            return new Vector2Int(Mathf.FloorToInt(clamped.x), Mathf.FloorToInt(clamped.y));
        }

        public Vector2 GridToWorld(Vector2Int pos) {
            return new Vector2(pos.x * _tileSize.x, pos.y * _tileSize.y);
        }

        private void GenerateMap()
        {
            ArrayHelpers.CreateAndFill(out _tiles, GetNumberOfTile(), (int x, int y) =>
            {
                Tile tile = null;

                if (x == 0 || y == 0 || x == _tiles.GetLength(1) - 1 || y == _tiles.GetLength(0) - 1)
                {
                    tile = Instantiate(
                        _edgeTile,
                        new Vector3(
                            GridToWorld(new Vector2Int(x, y)).x, 
                            GridToWorld(new Vector2Int(x, y)).y, 
                            0
                        ),
                        Quaternion.identity
                    );

                    tile.SetIsEdge(true);
                }
                else
                {
                    tile = Instantiate(
                        _backgroundTile,
                        new Vector3(
                            GridToWorld(new Vector2Int(x, y)).x, 
                            GridToWorld(new Vector2Int(x, y)).y, 
                            0
                        ),
                        Quaternion.identity
                    );

                    tile.SetIsEdge(false);
                }

                tile.transform.parent = transform;
                return tile;
            });
        }

        // This will take a reference to the player script which will be passed to OnPlayerEnter and OnPlayerAdjancent from IInteractable Interface.
        // For testing until player scripts are ready it's taking a vec2
        private void CheckPlayer(Vector2Int pos)
        {
            Tile[,] region = ArrayHelpers.ExtractRegion(_tiles, pos, 3);
            for (int y = 0; y < region.GetLength(0); y++)
            {
                for (int x = 0; x < region.GetLength(1); x++)
                {
                    if (x == 1 && y == 1)
                    {
                        if (
                            region[y, x] != null && 
                            !region[y, x].IsEdge() && 
                            region[y, x].HasInteractable()
                        ) region[y, x].GetInteractable().OnPlayerEnter(/* Player */);
                    }
                    else
                    {
                        if (
                            region[y, x] != null && 
                            !region[y, x].IsEdge() &&
                            region[y, x].HasInteractable()
                       ) region[y, x].GetInteractable().OnPlayerAdjancent(/* Player */);
                    }
                }
            }
        }

        private void Tick()
        {
            _lastTick = Time.time;

            if (Random.value < _spawnerSettings.powerUpSpawnRate)
            {
                Debug.Log("Spawning Powerup");
                foreach (Vector2 sample in _sampler.Sample(true))
                {
                    Vector2Int gridPT = WorldToGrid(sample);

                    if 
                    (
                        gridPT.x < 0                        || 
                        gridPT.y < 0                        || 
                        gridPT.x >= _tiles.GetLength(1)     || 
                        gridPT.y >= _tiles.GetLength(0)     || 
                        _tiles[gridPT.y, gridPT.x] == null  ||
                        _tiles[gridPT.y, gridPT.x].IsEdge() ||
                        _tiles[gridPT.y, gridPT.x].HasInteractable()
                    ) continue;

                    BasePowerup powerUp = Instantiate(
                        _spawnerSettings.powerupList[Random.Range(0, _spawnerSettings.powerupList.Count)],
                        new Vector3(
                            GridToWorld(new Vector2Int(gridPT.x, gridPT.y)).x,
                            GridToWorld(new Vector2Int(gridPT.x, gridPT.y)).y,
                            0
                        ),
                        Quaternion.identity
                    );
                     
                    _tiles[gridPT.y, gridPT.x].SetInteractable(powerUp);

                    powerUp.transform.parent = transform;
                    break;
                }
            }
        }

        private void Awake()
        {
            _backgroundTile = Resources.Load<Tile>("Tiles/Background");
            _edgeTile = Resources.Load<Tile>("Tiles/Edge");

            GenerateMap();

            _sampler = new PoissonSampler(_tiles.GetLength(1), _tiles.GetLength(0), _spawnerSettings.radius, (_spawnerSettings.seed >= 0) ? _spawnerSettings.seed : null, _spawnerSettings.k);
        }

        private void Update()
        {
            // Should be ran for each player
            CheckPlayer(_player);

            // Test Code
            DestroyImmediate(_playerTest);

            _playerTest = Instantiate<GameObject>(
                        Resources.Load<GameObject>("PlayerTest"),
                        new Vector3(
                            GridToWorld(new Vector2Int(_player.x, _player.y)).x,
                            GridToWorld(new Vector2Int(_player.x, _player.y)).y,
                            0
                        ),
                        Quaternion.identity
            );
        }

        private void FixedUpdate()
        {
            if (Time.time > _lastTick + _spawnerSettings.tickRate) Tick();
        }
    }
}
