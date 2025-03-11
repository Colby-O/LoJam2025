using LoJam.Grid;
using LoJam.Player;
using System.Collections.Generic;
using UnityEngine;

namespace LoJam.Interactable
{
    public abstract class PowerupBase : MonoBehaviour, IInteractable
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        public List<Tile> Tiles { get; set; }

        public Vector2Int GetGridSize() => new Vector2Int(1, 1);

        public Transform GetTransform() => transform;

        public Sprite GetSprite() => _spriteRenderer.sprite;

        public abstract void OnPlayerAdjancent(Interactor player);

        public abstract void OnPlayerAdjancentExit(Interactor player);

        public abstract void OnPlayerEnter(Interactor player);

        public abstract void OnPlayerExit(Interactor player);

        protected virtual void Awake()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
