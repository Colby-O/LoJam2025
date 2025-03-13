using LoJam.Core;
using LoJam.Grid;
using LoJam.MonoSystem;
using LoJam.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LoJam.Interactable
{
    public enum MaterialType
    {
        Square,
        Circle,
        Triangle,
        Cross,
        None
    }

    public class CraftingMaterial : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MaterialType _type;

        [Header("Animation")]
        [SerializeField] private float _animationSpeed = 2f;
        [SerializeField] private List<Sprite> _frames;
        [SerializeField] private float _dissolveRate = 0.01f;

        private float _time;
        private int _ptr = 0;

        public List<Tile> Tiles { get; set; }

        public Transform GetTransform() => transform;

        public MaterialType GetMaterialType() => _type;

        public Sprite GetSprite() => _spriteRenderer.sprite;

        public Vector2Int GetGridSize() => new Vector2Int(1, 1);

        public void OnPlayerAdjancent(Interactor player) { }

        public void OnPlayerAdjancentExit(Interactor player) { }

        public void OnPlayerExit(Interactor player) { }

        public void OnPlayerEnter(Interactor player)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(0);

            if (player.Item != null)
            {
                Vector2Int playerPos = GameManager.GetMonoSystem<IGridMonoSystem>().WorldToGrid(player.transform.position);
                player.Item.GetTransform().position = new Vector3(
                    GameManager.GetMonoSystem<IGridMonoSystem>().GridToWorld(playerPos).x, 
                    GameManager.GetMonoSystem<IGridMonoSystem>().GridToWorld(playerPos).y, 
                    0
                );

                player.Item.GetTransform().gameObject.SetActive(true);

                foreach (Tile tile in Tiles)
                {
                    tile.SetInteractable(player.Item);
                    player.Item.Tiles.Add(tile);
                }

                Tiles.Clear();
            }
            else
            {
                foreach (Tile tile in Tiles)
                {
                    tile.SetInteractable(null);
                }

                Tiles.Clear();
            }

            player.Item = this;
            gameObject.SetActive(false);
        }

        public void Remove()
        {
            SoftRemove();
            Destroy(gameObject);
        }

        public void SoftRemove()
        {
            foreach (Tile tile in Tiles) tile.SetInteractable(null);
            Tiles.Clear();
            GameManager.GetMonoSystem<IGridMonoSystem>().RemoveItemReference(this);
        }

        private IEnumerator Burn()
        {
            SoftRemove();

            float prog = 0;
            while (prog < 2)
            {
                prog += _dissolveRate;
                _spriteRenderer.material.SetFloat("_DissolveAmount", prog);
                yield return new WaitForNextFrameUnit();
            }
            Remove();
        }

        private void Awake()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();

            _ptr = 0;
            if (_frames != null && _frames.Count >= 1) _spriteRenderer.sprite = _frames[_ptr];
        }

        private void Update()
        {
            if (_frames == null || _frames.Count < 1) return;

            _time += Time.deltaTime;

            if (_time >= _animationSpeed)
            {
                _spriteRenderer.sprite = _frames[++_ptr % _frames.Count];
                _time = 0;
            }

            if (GameManager.GetMonoSystem<IGridMonoSystem>().IsNearFirewall(transform.position))
            {
                StartCoroutine(Burn());
            }

        }
    }
}
