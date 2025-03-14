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

        private Tile _playAreaTile;
        private Tile _backgroundTile;
        private Tile _sideTile;
        private Tile _cornerTile;

        private PoissonSampler _sampler;

        private List<Vector2Int> _playerLastPos;

        private float _lastTick;
        private int _tickID = 0;

        private Dictionary<Side, List<Vector2Int>> _spawnPoints;

        [SerializeField] private SerializableDictionary<Side, List<CraftingMaterial>> _itemsReference;

        public Vector2Int GetNumberOfTile() => new Vector2Int(Mathf.RoundToInt(_bounds.x / _tileSize.x), Mathf.RoundToInt(_bounds.y / _tileSize.y));

        public Vector2 GetTileSize() => _tileSize;

        public Vector2Int GetBounds() => _bounds;

        public Tile GetTileAt(int x, int y) => _tiles[y, x];

        public Tile GetTileAt(Vector2Int pos) => GetTileAt(pos.x, pos.y);

        public bool IsNearEdge(Vector2Int gridPos) => ArrayHelpers.ExtractRegion(_tiles, gridPos, 3).Cast<Tile>().Any(val => val.IsEdge());

        public bool IsNearEdge(Vector2 worldPos) => IsNearEdge(WorldToGrid(worldPos));

        public Vector3 GetFirewallPos() => _firewall.transform.position;

        public void AddFirewallDaemon(Side side) => _firewall.AddDaemon(side);

        public void RemoveFirewallDaemon(Side side) => _firewall.RemoveDaemon(side);

        public int GetDaemonCount(Side side) => _firewall.GetDaemonCount(side);

        public bool IsNearFirewall(Vector2 worldPos) => IsNearFirewall(WorldToGrid(worldPos));

        public bool IsOnFirewall(Vector2 worldPos) => IsOnFirewall(WorldToGrid(worldPos));

        public Side GetFirewallSide(Vector2 worldPos) => GetFirewallSide(WorldToGrid(worldPos));

        public Side GetFirewallSide(Vector2Int gridPos)
        {
            Vector2Int firwallPos = WorldToGrid(_firewall.transform.position);
            return (firwallPos.x > gridPos.x) ? Side.Left : Side.Right;
        }

        public Side GetSide(Vector2Int gridPos)
        {
            return (GetNumberOfTile().x / 2 > gridPos.x) ? Side.Left : Side.Right;
        }
        public Side GetSide(Vector2 worldPos)
        {
            return GetSide(WorldToGrid(worldPos));
        }

        public bool IsNearFirewall(Vector2Int gridPos)
        {
            Vector2Int firwallPos = WorldToGrid(_firewall.transform.position);

            return  firwallPos.x == gridPos.x  || firwallPos.x == gridPos.x + 1 || firwallPos.x == gridPos.x - 1;
        }

        public bool IsOnFirewall(Vector2Int gridPos)
        {
            Vector2Int firwallPos = WorldToGrid(_firewall.transform.position);

            return firwallPos.x == gridPos.x;
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

        public void RemoveItemReference(CraftingMaterial cm)
        {
            _itemsReference[Side.Left].Remove(cm);
            _itemsReference[Side.Right].Remove(cm);
        }

        public bool Spawn<T>(Side side, T obj) where T : MonoBehaviour, IInteractable => Spawn<T>(side, obj, out T _);
        
        public bool Spawn<T>(Side side, T obj, out T cm) where T: MonoBehaviour, IInteractable
        {
            cm = null;

            Vector2Int gridPT = _spawnPoints[side][Random.Range(0, _spawnPoints[side].Count)];
            if
            (
                gridPT.x < 0                                  ||
                gridPT.y < 0                                  ||
                gridPT.x >= _tiles.GetLength(1)               ||
                gridPT.y >= _tiles.GetLength(0)               ||
                _tiles[gridPT.y, gridPT.x] == null            ||
                _tiles[gridPT.y, gridPT.x].IsEdge()           ||
                _tiles[gridPT.y, gridPT.x].HasInteractable()  ||
                IsNearFirewall(gridPT)                        ||
                gridPT.x < 5                                  ||
                gridPT.x > GetNumberOfTile().x - 5            ||
                LoJamGameManager.players.Any(player => WorldToGrid(player.transform.position) == gridPT)
            ) return false;

            T objectInstance = Instantiate<T>(
                obj,
                new Vector3(
                    GridToWorld(new Vector2Int(gridPT.x, gridPT.y)).x,
                    GridToWorld(new Vector2Int(gridPT.x, gridPT.y)).y,
                    0
                ),
                Quaternion.identity,
                transform
            );

            AddToGrid(gridPT.x, gridPT.y, objectInstance);

            objectInstance.transform.localScale = Vector3.one.SetX(_tileSize.x).SetY(_tileSize.y);

            cm = objectInstance;
            Debug.Log(cm);
            return true;
        }

        private void Tick()
        {
            // Random Spawning Code
            //_lastTick = Time.time;

            //int maxTries = 100;
            //int tries = 0;

            //Side side = (Side)(_tickID % 2);

            //CraftingMaterial cm = null;
            //while (
            //    !Spawn(
            //    side,
            //    _spawnerSettings.FetchMaterial(_itemsReference[side]),
            //    out cm
            //    )
            //)
            //{
            //    if (maxTries < ++tries) break;
            //}

            //if (cm != null) _itemsReference[side].Add(cm);

            //_tickID++;
        }

        private void GenerateSpawnPoints()
        {
            _spawnPoints = new Dictionary<Side, List<Vector2Int>>();
            _spawnPoints[Side.Left] = new List<Vector2Int>();
            _spawnPoints[Side.Right] = new List<Vector2Int>();

            _sampler = new PoissonSampler(_tiles.GetLength(1), _tiles.GetLength(0), _spawnerSettings.radius, (_spawnerSettings.seed >= 0) ? _spawnerSettings.seed : null, _spawnerSettings.k);
            foreach (Vector2 pt in _sampler.Sample())
            {
                if (GetFirewallSide(pt) == Side.Left)
                {
                    _spawnPoints[Side.Left].Add(WorldToGrid(pt));
                }
                else
                {
                    _spawnPoints[Side.Right].Add(WorldToGrid(pt));
                }
            }
        }

        private void SpawnCraftingStations(int x, int y, Side side)
        {
            CraftingStation cs = Instantiate(
            Resources.Load<CraftingStation>((side == Side.Left) ? "CraftingStationLeft" : "CraftingStationRight"),
            new Vector3(
                GridToWorld(new Vector2Int(x, y)).x,
                GridToWorld(new Vector2Int(x, y)).y,
                0
            ),
           Quaternion.Euler(0f, 0f, (side == Side.Left) ? 90f : -90f),
            transform
            );
            cs.transform.localScale = Vector3.one.SetX(_tileSize.x).SetY(_tileSize.y);
            AddToGrid(x, y, cs);

            LoJamGameManager.craftingStations.Add(cs);
        }

        private void SpawnDisp(int x, int y, Side side, string name)
        {
            CraftingStation cs = Instantiate(
            Resources.Load<CraftingStation>((side == Side.Left) ? $"Disp/{name}L" : $"Disp/{name}R"),
            new Vector3(
                GridToWorld(new Vector2Int(x, y)).x,
                GridToWorld(new Vector2Int(x, y)).y,
                0
            ),
           Quaternion.Euler(0f, 0f, (side == Side.Left) ? 90f : -90f),
            transform
            );
            cs.transform.localScale = Vector3.one.SetX(_tileSize.x - 0.15f).SetY(_tileSize.y - 0.15f);
            AddToGrid(x, y, cs);
        }

        private void Awake()
        {
            _playAreaTile = Resources.Load<Tile>("Tiles/PlayArea");
            _backgroundTile = Resources.Load<Tile>("Tiles/Background");
            _sideTile = Resources.Load<Tile>("Tiles/Side");
            _cornerTile = Resources.Load<Tile>("Tiles/Corner");

            _itemsReference[Side.Left] = new List<CraftingMaterial>();
            _itemsReference[Side.Right] = new List<CraftingMaterial>();

            _tickID = Random.Range(0, 2);

            _firewall = Instantiate<FirewallController>(
                Resources.Load<FirewallController>("Firewall"), 
                new Vector3(_bounds.x / 2f, _bounds.y / 2f, 0), 
                Quaternion.identity,
                transform
            );

            _firewall.OnMove.AddListener(GenerateSpawnPoints);

            GenerateMap();

            SpawnCraftingStations(3, GetNumberOfTile().y / 2, Side.Left);

            SpawnDisp(3, 6 * GetNumberOfTile().y / 7, Side.Left, "Circle");
            SpawnDisp(3, 5 * GetNumberOfTile().y / 7, Side.Left, "Triangle");
            SpawnDisp(3, 2 * GetNumberOfTile().y / 7, Side.Left, "Square");
            SpawnDisp(3, GetNumberOfTile().y / 7, Side.Left, "Cross");

            SpawnCraftingStations(GetNumberOfTile().x - 3, GetNumberOfTile().y / 2, Side.Right);

            SpawnDisp(GetNumberOfTile().x - 3, 6 * GetNumberOfTile().y / 7, Side.Right, "Circle");
            SpawnDisp(GetNumberOfTile().x - 3, 5 * GetNumberOfTile().y / 7, Side.Right, "Triangle");
            SpawnDisp(GetNumberOfTile().x - 3, 2 * GetNumberOfTile().y / 7, Side.Right, "Square");
            SpawnDisp(GetNumberOfTile().x - 3, GetNumberOfTile().y / 7, Side.Right, "Cross");

            GenerateSpawnPoints();
        }

        private void Start()
        {
            _playerLastPos = new List<Vector2Int>();
            for (int i = 0; i < LoJamGameManager.players.Count; i++)
            {
                _playerLastPos.Add(WorldToGrid(LoJamGameManager.players[i].transform.position));

                if (LoJamGameManager.players[i].GetSide() == Side.Left)
                    LoJamGameManager.players[i].transform.position = new Vector3(GridToWorld(new Vector2Int(GetNumberOfTile().x / 4, GetNumberOfTile().y / 2)).x, GridToWorld(new Vector2Int(GetNumberOfTile().x / 4, GetNumberOfTile().y / 2)).y, 0);
                else
                    LoJamGameManager.players[i].transform.position = new Vector3(GridToWorld(new Vector2Int(3 * GetNumberOfTile().x / 4, GetNumberOfTile().y / 2)).x, GridToWorld(new Vector2Int(3 * GetNumberOfTile().x / 4, GetNumberOfTile().y / 2)).y, 0);
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
        }

        private void FixedUpdate()
        {
            if (Time.time > _lastTick + _spawnerSettings.tickRate) Tick();
        }
    }
}
