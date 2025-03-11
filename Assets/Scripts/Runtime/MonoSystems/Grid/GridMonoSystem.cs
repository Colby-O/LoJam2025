using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoJam.Grid;
using LoJam.Interactable;
using LoJam.Core;
using LoJam.Logic;
using LoJam.Player;
using static UnityEngine.Rendering.DebugUI.Table;

namespace LoJam.MonoSystem
{
    public class GridMonoSystem : MonoBehaviour, IGridMonoSystem
    {
        [Header("Grid Settings")]
        [SerializeField] private Vector2Int _bounds;
        [SerializeField] private Vector2 _tileSize;

        [Header("Spawner Settings")]
        [SerializeField] private SpawnerSettings _spawnerSettings;

        private Tile[,] _tiles;

        // I don't know if firewall shit should be here but frig it for the time being
        private FirewallController _firewall;

        private Tile _playAreaTile;
        private Tile _backgroundTile;
        private Tile _sideTile;
        private Tile _cornerTile;

        private PoissonSampler _sampler;

        private List<Vector2Int> _playerLastPos;

        private float _lastTick;

        private List<Vector2> _spawnPoints;

        public Vector2Int GetNumberOfTile() => new Vector2Int(Mathf.RoundToInt(_bounds.x / _tileSize.x), Mathf.RoundToInt(_bounds.y / _tileSize.y));

        public Vector2 GetTileSize() => _tileSize;

        public Vector2Int GetBounds() => _bounds;

        public Tile GetTileAt(int x, int y) => _tiles[y, x];

        public Tile GetTileAt(Vector2Int pos) => GetTileAt(pos.x, pos.y);

        public bool IsNearEdge(Vector2Int gridPos) => ArrayHelpers.ExtractRegion(_tiles, gridPos, 3).Cast<Tile>().Any(val => val.IsEdge());

        public bool IsNearEdge(Vector2 worldPos) => IsNearEdge(WorldToGrid(worldPos));

        public void AddFirewallDaemon(Side side) => _firewall.AddDaemon(side);

        public void RemoveFirewallDaemon(Side side) => _firewall.RemoveDaemon(side);

        public int GetDaemonCount(Side side) => _firewall.GetDaemonCount(side);

        public bool IsNearFirewall(Vector2 worldPos, Side side) => IsNearFirewall(WorldToGrid(worldPos), side);

        public bool IsNearFirewall(Vector2Int gridPos, Side side)
        {
            Vector2Int firwallPos = WorldToGrid(_firewall.transform.position);

            return (side == Side.Left) ? firwallPos.x > gridPos.x : firwallPos.x < gridPos.x;
        }

        public Vector2Int WorldToGrid(Vector2 pos) {
            Vector2 clamped = pos / _tileSize;
            return new Vector2Int(Mathf.FloorToInt(clamped.x), Mathf.FloorToInt(clamped.y));
        }

        public Vector2 GridToWorld(Vector2Int pos) {
            return new Vector2(pos.x * _tileSize.x, pos.y * _tileSize.y);
        }

        private void FillBackground()
        {
            for (int y = -_tiles.GetLength(0); y <= 2 * _tiles.GetLength(0); y++)
            {
                for (int x = -_tiles.GetLength(1); x <= 2 * _tiles.GetLength(1); x++)
                {
                    if (x >= 0 && y >= 0 && x < _tiles.GetLength(1) && y < _tiles.GetLength(0)) continue;
                    Tile tile = Instantiate(
                        _backgroundTile,
                        new Vector3(
                            GridToWorld(new Vector2Int(x, y)).x,
                            GridToWorld(new Vector2Int(x, y)).y,
                            0
                        ),
                        Quaternion.identity,
                        transform
                    );

                    tile.transform.localScale = Vector3.one.SetX(_tileSize.x).SetY(_tileSize.y);
                }
            }
        }

        private Tile RotateTile(Tile tile, float rot)
        {
            tile.GetSpriteGameObject().gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
            return tile;
        }

        private Tile SelectTile(int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return RotateTile(_cornerTile, 90);
            }
            else if (x == _tiles.GetLength(1) - 1 && y == 0)
            {
                return RotateTile(_cornerTile, 180);
            }
            else if (x == 0 && y == _tiles.GetLength(0) - 1)
            {
                return RotateTile(_cornerTile, 0);
            }
            else if (x == _tiles.GetLength(1) - 1 && y == _tiles.GetLength(0) - 1)
            {
                return RotateTile(_cornerTile, 270);
            }
            else if (x == 0)
            {
                return RotateTile(_sideTile, 90);
            }
            else if (y == 0)
            {
                return RotateTile(_sideTile, 180);
            }
            else if (x == _tiles.GetLength(1) - 1)
            {
                return RotateTile(_sideTile, 270);
            }
            else if (y == _tiles.GetLength(0) - 1)
            {
                return RotateTile(_sideTile, 0);
            }
            else
            {
                return _playAreaTile;
            }
        }

        private void GenerateMap()
        {
            ArrayHelpers.CreateAndFill(out _tiles, GetNumberOfTile(), (int x, int y) =>
            {
                Tile tile = null;

                tile = Instantiate(
                    SelectTile(x, y),
                    new Vector3(
                        GridToWorld(new Vector2Int(x, y)).x,
                        GridToWorld(new Vector2Int(x, y)).y,
                        0
                    ),
                    Quaternion.identity,
                    transform
                );

                tile.SetIsEdge(x == 0 || y == 0 || x == _tiles.GetLength(1) - 1 || y == _tiles.GetLength(0) - 1);

                tile.transform.localScale = Vector3.one.SetX(_tileSize.x).SetY(_tileSize.y);
                return tile;
            });

            FillBackground();
        }

        private void CheckPlayerEnter(Interactor player)
        {
            Vector2Int pos = WorldToGrid(player.transform.position);
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
                        ) region[y, x].GetInteractable().OnPlayerEnter(player);
                    }
                    else
                    {
                        if (
                            region[y, x] != null && 
                            !region[y, x].IsEdge() &&
                            region[y, x].HasInteractable()
                       ) region[y, x].GetInteractable().OnPlayerAdjancent(player);
                    }
                }
            }
        }

        private void CheckPlayerExit(Interactor player, Vector2Int _lastPos)
        {
            Vector2Int pos= _lastPos;
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
                        ) region[y, x].GetInteractable().OnPlayerExit(player);
                    }
                    else
                    {
                        if (
                            region[y, x] != null &&
                            !region[y, x].IsEdge() &&
                            region[y, x].HasInteractable()
                       ) region[y, x].GetInteractable().OnPlayerAdjancentExit(player);
                    }
                }
            }
        }

        public void AddToGrid(int x, int y, IInteractable obj)
        {
            Vector2Int space = obj.GetGridSize();

            for (int j = -Mathf.FloorToInt(space.y / 2f); j < Mathf.CeilToInt(space.y / 2f); j++)
            {
                for (int i = -Mathf.FloorToInt(space.x / 2f); i < Mathf.CeilToInt(space.x / 2f); i++)
                {
                    _tiles[y + j, x + i].SetInteractable(obj);
                }
            }
        }

        private void Tick()
        {
            _lastTick = Time.time;

            if (Random.value < _spawnerSettings.materialSpawnRate)
            {
                Debug.Log("Spawning Powerup");

                int maxTries = 100;
                int tries = 0;

                while (true)
                {
                    Vector2Int gridPT = WorldToGrid(_spawnPoints[Random.Range(0, _spawnPoints.Count)]);

                    if
                    (
                        gridPT.x < 0 ||
                        gridPT.y < 0 ||
                        gridPT.x >= _tiles.GetLength(1) ||
                        gridPT.y >= _tiles.GetLength(0) ||
                        _tiles[gridPT.y, gridPT.x] == null ||
                        _tiles[gridPT.y, gridPT.x].IsEdge() ||
                        _tiles[gridPT.y, gridPT.x].HasInteractable()
                    )
                    {
                        if (++tries > maxTries) break;
                        else continue;
                    }

                    CraftingMaterial craftingMaterial = Instantiate(
                        _spawnerSettings.matieralList[Random.Range(0, _spawnerSettings.matieralList.Count)],
                        new Vector3(
                            GridToWorld(new Vector2Int(gridPT.x, gridPT.y)).x,
                            GridToWorld(new Vector2Int(gridPT.x, gridPT.y)).y,
                            0
                        ),
                        Quaternion.identity,
                        transform
                    );

                    AddToGrid(gridPT.x, gridPT.y, craftingMaterial);

                    craftingMaterial.transform.localScale = Vector3.one.SetX(_tileSize.x).SetY(_tileSize.y);
                    break;
                }
            }
        }

        private void Awake()
        {
            _playAreaTile = Resources.Load<Tile>("Tiles/PlayArea");
            _backgroundTile = Resources.Load<Tile>("Tiles/Background");
            _sideTile = Resources.Load<Tile>("Tiles/Side");
            _cornerTile = Resources.Load<Tile>("Tiles/Corner");

            _firewall = Instantiate<FirewallController>(
                Resources.Load<FirewallController>("Firewall"), 
                new Vector3(_bounds.x / 2f, _bounds.y / 2f, 0), 
                Quaternion.identity,
                transform
            );

            GenerateMap();

            CraftingStation cs = Instantiate(
                Resources.Load<CraftingStation>("CraftingStation"),
                new Vector3(
                            GridToWorld(new Vector2Int(5, 5)).x,
                            GridToWorld(new Vector2Int(5, 5)).y,
                            0
                        ),
                        Quaternion.identity,
                        transform
            );
            cs.transform.localScale = Vector3.one.SetX(_tileSize.x).SetY(_tileSize.y);
            AddToGrid(5, 5, cs);
            _sampler = new PoissonSampler(_tiles.GetLength(1), _tiles.GetLength(0), _spawnerSettings.radius, (_spawnerSettings.seed >= 0) ? _spawnerSettings.seed : null, _spawnerSettings.k);
            _spawnPoints = _sampler.Sample(int.MaxValue);
        }

        private void Start()
        {
            _playerLastPos = new List<Vector2Int>();
            for (int i = 0; i < LoJamGameManager.players.Count; i++)
            {
                _playerLastPos.Add(WorldToGrid(LoJamGameManager.players[i].transform.position));
            }
        }

        private void Update()
        {
            for (int i = 0; i < LoJamGameManager.players.Count; i++)
            {
                if (_playerLastPos.Count > i)
                {
                    if (!(WorldToGrid(LoJamGameManager.players[i].transform.position) == _playerLastPos[i]))
                    {
                        CheckPlayerExit(LoJamGameManager.players[i], _playerLastPos[i]);
                        CheckPlayerEnter(LoJamGameManager.players[i]);
                    }
                }

                _playerLastPos[i] = WorldToGrid(LoJamGameManager.players[i].transform.position);
            }

            // Delete: For testing (Old input system don't use)
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Adding Left!");
                AddFirewallDaemon(Side.Left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Adding Right!");
                AddFirewallDaemon(Side.Right);
            }
        }

        private void FixedUpdate()
        {
            if (Time.time > _lastTick + _spawnerSettings.tickRate) Tick();
        }
    }
}
