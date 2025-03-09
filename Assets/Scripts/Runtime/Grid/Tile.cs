using UnityEngine;
using LoJam.Interactable;
using LoJam.Core;

namespace LoJam.Grid
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _sptire;

        private IInteractable _interactable;
        private bool _isEdge = false;

        public bool IsEdge() => _isEdge;
        public void SetIsEdge(bool isEdge) => _isEdge = isEdge;

        public bool HasInteractable() => _interactable != null;
        public IInteractable GetInteractable() => _interactable;
        public void SetInteractable(IInteractable interactable) 
        {
            if (_interactable != null) _interactable.Tile = null;
            _interactable = interactable;
            if (_interactable != null) _interactable.Tile = this;
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
