using LoJam.Core;
using LoJam.MonoSystem;
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
