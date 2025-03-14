using LoJam.Core;
using LoJam.Grid;
using LoJam.Interactable;
using LoJam.Logic;
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
        public bool IsNearEdge(Vector2Int gridPos);
        public bool IsNearEdge(Vector2 worldPos);
        public Side GetSide(Vector2Int gridPos);
        public Side GetSide(Vector2 worldPos);
        public Vector2Int WorldToGrid(Vector2 pos);
        public Vector2 GridToWorld(Vector2Int pos);
        public void AddFirewallDaemon(Side side);
        public void RemoveFirewallDaemon(Side side);
        public int GetDaemonCount(Side side);
        public bool IsNearFirewall(Vector2 worldPos);
        public bool IsNearFirewall(Vector2Int gridPos);
        public bool IsOnFirewall(Vector2 worldPos);
        public bool IsOnFirewall(Vector2Int gridPos);
        public Side GetFirewallSide(Vector2 worldPos);
        public Side GetFirewallSide(Vector2Int gridPos);
        public Vector3 GetFirewallPos();
        public void AddToGrid(int x, int y, IInteractable obj);
        public bool Spawn<T>(Side side, T obj) where T : MonoBehaviour, IInteractable;
        public bool Spawn<T>(Side side, T obj, out T cm) where T : MonoBehaviour, IInteractable;
        public void RemoveItemReference(CraftingMaterial cm);
        public void FillBackground(Camera camera);
    }
}
