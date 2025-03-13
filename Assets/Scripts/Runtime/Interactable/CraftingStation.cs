using LoJam.Core;
using LoJam.Crafting;
using LoJam.Grid;
using LoJam.Logic;
using LoJam.MonoSystem;
using LoJam.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LoJam.Interactable
{
    public class CraftingStation : MonoBehaviour, IInteractable
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        [SerializeField] private TMP_Text _label;

        [SerializeField] private List<SpriteRenderer> _itemsUI;

        [SerializeField] private GameObject _lighting;

        [SerializeField] private List<Sprite> _computerSprites;

        [SerializeField] private SerializableDictionary<MaterialType, List<Sprite>> _sprites;

        [SerializeField] float _craftingAnimSpeed = 1f;

        [SerializeField] Side _side;

        [SerializeField] private Recipe _selectedRecipe;

        private int _ptr;

        private float _hackDuration;
        private float _hackedTIme = 0;
        private bool _hacked = false;

        public List<Tile> Tiles { get; set; }

        public Transform GetTransform() => transform;

        public Sprite GetSprite() => _spriteRenderer.sprite;

        public Vector2Int GetGridSize() => new Vector2Int(4, 5);

        public Side GetSide() => _side;

        public void Hack(bool state, float duration)
        {
            _hackedTIme = 0;
            _hackDuration = duration;
            _hacked = state;
            _spriteRenderer.sprite = state ? _computerSprites[0] : _computerSprites[1];
        }

        public void OnPlayerAdjancent(Interactor player)
        {
            player.NearbyCraftingStation = this;
            UseCraftingStation(player);
        }

        public void OnPlayerEnter(Interactor player) { }

        public void OnPlayerExit(Interactor player) { }

        public void OnPlayerAdjancentExit(Interactor player)
        {
            player.NearbyCraftingStation = null;
        }

        public void CompleteRecipe(Interactor player)
        {
            if (_selectedRecipe.Craft(player)) 
            {
                StartCoroutine(CraftingAnimation());
            }
        }

        public void SwitchRecipe()
        {
            if (_hacked) return;

            _selectedRecipe = GameManager.GetMonoSystem<ICraftingMonoSystem>().GetAllRecipes(_side)[++_ptr % GameManager.GetMonoSystem<ICraftingMonoSystem>().GetAllRecipes(_side).Count];
            ShowRecipe(_selectedRecipe);
        }

        public void UseCraftingStation(Interactor player)
        {
            if (_hacked) return;

            if (player.HasCraftingMaterial())
            {
                CraftingMaterial cm = player.Item as CraftingMaterial;

                if (
                    _selectedRecipe.GetMaterials().Contains(cm.GetMaterialType()) && 
                    _selectedRecipe.GetProgress().Where(
                        m => m.GetMaterialType() == cm.GetMaterialType()
                    ).Count() != 
                    _selectedRecipe.GetMaterials().Where(
                        m => m == cm.GetMaterialType()
                    ).Count()
                )
                {
                    _selectedRecipe.GetProgress().Add(cm);
                    ShowRecipe(_selectedRecipe);
                    GameManager.GetMonoSystem<IGridMonoSystem>().RemoveItemReference(cm);
                    player.Item = null;
                }
            }

            CompleteRecipe(player);
        }

        private IEnumerator CraftingAnimation()
        {
            foreach (SpriteRenderer sr in _itemsUI) sr.gameObject.SetActive(false);
            _lighting.SetActive(true);

            yield return new WaitForSeconds(_craftingAnimSpeed);

            _lighting.SetActive(false);
            foreach (SpriteRenderer sr in _itemsUI) sr.gameObject.SetActive(true);

            ShowRecipe(_selectedRecipe);
        }

        private void ShowRecipe(Recipe recipe)
        {
            _label.text = $"Crafting: {recipe.Label}";

            List<MaterialType> copyRecipe = new List<MaterialType>(recipe.GetMaterials());

            for (int i = 0; i < _itemsUI.Count; i++)
            {
                List<Sprite> cs = _sprites[recipe.GetMaterials()[i]];

                _itemsUI[i].sprite = cs[0];
            }

            for (int i = 0; i < _itemsUI.Count; i++)
            {
                if (i < recipe.GetProgress().Count && copyRecipe.Contains(recipe.GetProgress()[i].GetMaterialType()))
                {
                    int index = copyRecipe.IndexOf(recipe.GetProgress()[i].GetMaterialType());
                    List<Sprite> cs = _sprites[recipe.GetMaterials()[index]];
                    copyRecipe[index] = MaterialType.None;
                    _itemsUI[index].sprite = cs[1];
                }
            }
        }

        private void Init()
        {
            _selectedRecipe = GameManager.GetMonoSystem<ICraftingMonoSystem>().GetFirewallRecipe(_side);
            ShowRecipe(_selectedRecipe);
        }

        private void Awake()
        {
            GameManager.GetMonoSystem<ICraftingMonoSystem>().OnInit.AddListener(Init);
            _lighting.SetActive(false);
            _spriteRenderer.sprite = _computerSprites[1];
        }

        private void Update()
        {
            if (_hacked)
            {
                _hackedTIme += Time.deltaTime;
                if (_hackedTIme > +_hackDuration) Hack(false, 0);
            }
        }
    }
}
