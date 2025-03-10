using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LoJam.Grid;
using LoJam.Interactable;
using LoJam.Core;
using LoJam.Logic;
using LoJam.Player;

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

        private Tile _backgroundTile;
        private Tile _edgeTile;

        private PoissonSampler _sampler;

        private List<Vector2Int> _playerLastPos;

        private float _lastTick;

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
                        Quaternion.identity,
                        transform
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
                        Quaternion.identity,
                        transform
                    );

                    tile.SetIsEdge(false);
                }

                tile.transform.localScale = Vector3.one.SetX(_tileSize.x).SetY(_tileSize.y);
                return tile;
            });
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

        private void Tick()
        {
            _lastTick = Time.time;

            if (Random.value < _spawnerSettings.materialSpawnRate)
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

                    CraftingMaterial powerUp = Instantiate(
                        _spawnerSettings.matieralList[Random.Range(0, _spawnerSettings.matieralList.Count)],
                        new Vector3(
                            GridToWorld(new Vector2Int(gridPT.x, gridPT.y)).x,
                            GridToWorld(new Vector2Int(gridPT.x, gridPT.y)).y,
                            0
                        ),
                        Quaternion.identity,
                        transform
                    );
                     
                    _tiles[gridPT.y, gridPT.x].SetInteractable(powerUp);


                    powerUp.transform.localScale = Vector3.one.SetX(_tileSize.x).SetY(_tileSize.y);
                    break;
                }
            }
        }

        private void Awake()
        {
            _backgroundTile = Resources.Load<Tile>("Tiles/Background");
            _edgeTile = Resources.Load<Tile>("Tiles/Edge");

            _firewall = Instantiate<FirewallController>(
                Resources.Load<FirewallController>("Firewall"), 
                new Vector3(_bounds.x / 2f, _bounds.y / 2f, 0), 
                Quaternion.identity,
                transform
            );

            _firewall.transform.localScale = Vector3.one.SetY(_bounds.y).SetX(_tileSize.x);

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
            _tiles[5, 5].SetInteractable(cs);
            // !! NOTE: This is bias as fuck please fix me at some point !!
            _sampler = new PoissonSampler(_tiles.GetLength(1), _tiles.GetLength(0), _spawnerSettings.radius, (_spawnerSettings.seed >= 0) ? _spawnerSettings.seed : null, _spawnerSettings.k);
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
                    CheckPlayerExit(LoJamGameManager.players[i], _playerLastPos[i]);
                    CheckPlayerEnter(LoJamGameManager.players[i]);
                }

                _playerLastPos[i] = WorldToGrid(LoJamGameManager.players[i].transform.position);
            }

            // Test Code
            //Destroy(_playerTest);

            //_playerTest = Instantiate<GameObject>(
            //            Resources.Load<GameObject>("PlayerTest"),
            //            new Vector3(
            //                GridToWorld(new Vector2Int(_player.x, _player.y)).x,
            //                GridToWorld(new Vector2Int(_player.x, _player.y)).y,
            //                0
            //            ),
            //            Quaternion.identity
            //);

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
