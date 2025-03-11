using LoJam.Core;
using LoJam.Grid;
using LoJam.MonoSystem;
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

        public Transform GetTransform() => transform;

        public MaterialType GetMaterialType() => _type;

        public Sprite GetSprite() => _spriteRenderer.sprite;

        public void OnPlayerAdjancent(Interactor player) { }

        public void OnPlayerAdjancentExit(Interactor player) { }

        public void OnPlayerExit(Interactor player) { }

        public void OnPlayerEnter(Interactor player)
        {
            if (player.Item != null)
            {
                Vector2Int playerPos = GameManager.GetMonoSystem<IGridMonoSystem>().WorldToGrid(player.transform.position);
                player.Item.GetTransform().position = new Vector3(
                    GameManager.GetMonoSystem<IGridMonoSystem>().GridToWorld(playerPos).x, 
                    GameManager.GetMonoSystem<IGridMonoSystem>().GridToWorld(playerPos).y, 
                    0
                );

                player.Item.GetTransform().gameObject.SetActive(true);

                Tile.SetInteractable(player.Item); 
            }
            else
            {
                Tile.SetInteractable(null);
            }

            player.Item = this;
            gameObject.SetActive(false);
        }

        protected void Awake()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
