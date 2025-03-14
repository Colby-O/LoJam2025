using LoJam.Core;
using LoJam.Grid;
using LoJam.MonoSystem;
using LoJam.Player;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace LoJam.Logic
{
    public enum Side
    {
        Left,
        Right
    }

    public static class SideExtension 
    {
        public static Side Opposide(this Side side)
        {
            return (side == Side.Left) ? Side.Right : Side.Left;
        }
    }

    public class FirewallController : MonoBehaviour
    {
        [SerializeField] private float _movementUnit = 0.001f;
        [SerializeField] private float _movementStep = 0.25f;

        [SerializeField, ColorUsage(true, true)] private Color _baseColor;
        [SerializeField, ColorUsage(true, true)] private Color _leftColor;
        [SerializeField, ColorUsage(true, true)] private Color _rightColor;

        private int _leftDaemonCount = 0;
        private int _rightDaemonCount = 0;

        private float _netMovement;

        private FireElement _firewallTile;

        private List<FireElement> _tiles;

        private Collider2D _currentPlayer;
        private float _openDuration;
        private float _openTime;
        private bool _isOpen;

       [SerializeField]  private float _virtualPosition;

        public UnityEvent OnMove = new UnityEvent();
        public UnityEvent OnDameonChange = new UnityEvent();

        public void Restart()
        {
            _leftDaemonCount = 0;
            _rightDaemonCount = 0;
            foreach (FireElement f in _tiles)
            {
                Physics2D.IgnoreCollision(LoJamGameManager.players[0].GetComponent<Collider2D>(), f.Collider, false);
                Physics2D.IgnoreCollision(LoJamGameManager.players[1].GetComponent<Collider2D>(), f.Collider, false);
                f.SpriteRenderer.material.SetColor("_Intensity", _baseColor);
            }

            _virtualPosition = GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x / 2f;
            ProcessMovement(false);
        }

        public void AddDaemon(Side side)
        {
            if (side == Side.Left) _leftDaemonCount++;
            else _rightDaemonCount++;

            OnDameonChange.Invoke();
        }

        public void RemoveDaemon(Side side)
        {
            if (side == Side.Left) _leftDaemonCount--;
            else _rightDaemonCount--;

            OnDameonChange.Invoke();
        }

        public int GetDaemonCount(Side side)
        {
            return (side == Side.Left) ? _leftDaemonCount : _rightDaemonCount;
        }

        public void Open(Interactor player, float duration)
        {
            if (_isOpen) Close();

            _openDuration = duration;
            _openTime = 0;
            _isOpen = true;
            _currentPlayer = player.GetComponent<Collider2D>();

            foreach (FireElement f in _tiles)
            {
                Physics2D.IgnoreCollision(_currentPlayer, f.Collider, true);
                f.SpriteRenderer.material.SetColor("_Intensity", (player.GetSide() == Side.Left) ? _leftColor : _rightColor);
            }
        }

        public void Close()
        {
            _openDuration = 0;
            _openTime = 0;
            _isOpen = false;

            foreach (FireElement f in _tiles)
            {
                Physics2D.IgnoreCollision(_currentPlayer, f.Collider, false);
                f.SpriteRenderer.material.SetColor("_Intensity", _baseColor);
            }

            if (
                GameManager.GetMonoSystem<IGridMonoSystem>().GetSide(_currentPlayer.transform.position) != _currentPlayer.GetComponent<Interactor>().GetSide()
            ) _currentPlayer.GetComponent<PlayerController>().ReturnPlayer();
            _currentPlayer = null;
        }

        public void ProcessMovement(bool playSound = true)
        {
            Vector2Int bounds = GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds();

            if (transform.position.x > 0 && _netMovement < 0 || transform.position.x < bounds.x - 1 && _netMovement > 0)
            {
                // Discrete movement
                _virtualPosition = _virtualPosition + _netMovement * _movementUnit;

                // Continuous movement
                //transform.position = transform.position.SetX(transform.position.x + _netMovement * _movementUnit);
            }

            if (Mathf.Abs(_virtualPosition - transform.position.x) >= _movementStep)
            {
                if (playSound) GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(3);
                transform.position = transform.position.SetX(_virtualPosition);
                OnMove.Invoke();
            }
        }

        public void SetPositon(Vector3 pos)
        {
            transform.position = pos;
            _virtualPosition = transform.position.x;
        }

        private void ConstructSprite(Vector2Int bounds, Vector2 tileSize)
        {
            _tiles = new List<FireElement>();

            for (int i = -10; i < Mathf.RoundToInt(bounds.y / tileSize.y) + 10; i++)
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

                tile.SpriteRenderer.material.SetColor("_Intensity", _baseColor);

                tile.transform.localScale = Vector3.one.SetX(tileSize.x).SetY(tileSize.y);

                _tiles.Add(tile);
            }
        }

        private void Start()
        {
            Vector2 tileSize = GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize();
            Vector2Int bounds = GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds();

            _firewallTile = Resources.Load<FireElement>("Tiles/Firewall");

            ConstructSprite(bounds, tileSize);

            _virtualPosition = transform.position.x;
        }

        private void Update()
        {
            _netMovement = _leftDaemonCount - _rightDaemonCount;

            if (_isOpen)
            {
                _openTime += Time.deltaTime;

                if (_openTime > _openDuration) Close();
            }

            if 
            (
                Mathf.Abs(transform.position.x - GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x) >= GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x - 3f || 
                Mathf.Abs(transform.position.x - GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x) <= 3f 
            )
            {
                LoJamGameManager.EndGame(GameManager.GetMonoSystem<IGridMonoSystem>().GetSide(transform.position).Opposide());
            }
        }

        private void FixedUpdate()
        {
            ProcessMovement();
        }
    }
}
