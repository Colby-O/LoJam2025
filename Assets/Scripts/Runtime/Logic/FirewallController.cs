using LoJam.Core;
using LoJam.MonoSystem;
using System.Collections.Generic;
using UnityEngine;

namespace LoJam.Logic
{
    // Shouldn't be here :<
    public enum Side
    {
        Left,
        Right
    }

    public class FirewallController : MonoBehaviour
    {
        [SerializeField] private float _movementUnit = 0.001f;

        private int _leftDaemonCount;
        private int _rightDaemonCount;

        private float _netMovement;

        private FireElement _firewallTile;

        private List<FireElement> _tiles;

        public void AddDaemon(Side side)
        {
            if (side == Side.Left) _leftDaemonCount++;
            else _rightDaemonCount++;
        }

        public void RemoveDaemon(Side side)
        {
            if (side == Side.Left) _leftDaemonCount--;
            else _rightDaemonCount--;
        }

        public int GetDaemonCount(Side side)
        {
            return (side == Side.Left) ? _leftDaemonCount : _rightDaemonCount;
        }

        public void ProcessMovement()
        {
            Vector2Int bounds = GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds();

            if (transform.position.x > 0 && _netMovement < 0 || transform.position.x < bounds.x - 1 && _netMovement > 0)
            {
                transform.position = transform.position.SetX(transform.position.x + _netMovement * _movementUnit);
            }
        }

        private void ConstructSprite(Vector2Int bounds, Vector2 tileSize)
        {
            _tiles = new List<FireElement>();

            for (int i = 0; i < Mathf.RoundToInt(bounds.y / tileSize.y); i++)
            {
                FireElement tile = Instantiate<FireElement>(
                    _firewallTile,
                    new Vector3(
                         bounds.x / 2f,
                        i * tileSize.y,
                        0
                    ),
                    Quaternion.identity,
                    transform
                );

                tile.transform.localScale = Vector3.one.SetX(tileSize.x).SetY(tileSize.y);
            }
        }

        private void Start()
        {
            Vector2 tileSize = GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize();
            Vector2Int bounds = GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds();

            _firewallTile = Resources.Load<FireElement>("Tiles/Firewall");

            ConstructSprite(bounds, tileSize);
        }

        private void Update()
        {
            _netMovement = _leftDaemonCount - _rightDaemonCount;
        }

        private void FixedUpdate()
        {
            ProcessMovement();
        }
    }
}
