using LoJam.Grid;
using LoJam.Player;
using UnityEngine;

namespace LoJam.Interactable
{
    public enum MaterialType
    {
        Square,
        Circle,
        Triangle
    }

    public class CraftingMaterial : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MaterialType _type;

        public Tile Tile { get; set; }

        public MaterialType GetMaterialType() => _type;

        public Sprite GetSprite() => _spriteRenderer.sprite;

        public void OnPlayerAdjancent(Interactor player)
        {

        }

        public void OnPlayerAdjancentExit(Interactor player)
        {
            if (player.Item != null) return;
            player.Item = this;
            Tile.SetInteractable(null);
            gameObject.SetActive(false);
        }

        public void OnPlayerEnter(Interactor player)
        {

        }

        public void OnPlayerExit(Interactor player)
        {

        }

        protected void Awake()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
