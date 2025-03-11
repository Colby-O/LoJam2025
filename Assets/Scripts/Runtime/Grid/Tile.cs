using UnityEngine;
using LoJam.Interactable;
using LoJam.Core;
using System.Collections.Generic;

namespace LoJam.Grid
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _sptire;

        private IInteractable _interactable;
        private bool _isEdge = false;

        public GameObject GetSpriteGameObject() => _spriteRenderer.gameObject;

        public bool IsEdge() => _isEdge;

        public void SetIsEdge(bool isEdge) => _isEdge = isEdge;

        public bool HasInteractable() => _interactable != null;

        public IInteractable GetInteractable() => _interactable;

        public void SetInteractable(IInteractable interactable) 
        {
            _interactable = interactable;
            if (_interactable != null && _interactable.Tiles == null) _interactable.Tiles = new List<Tile>();
            if (_interactable != null) _interactable.Tiles.Add(this);
        }

        public void Refresh()
        {
            if (_sptire != null) _spriteRenderer.sprite = _sptire;
        }

        public void Awake()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            Refresh();
        }
    }
}
