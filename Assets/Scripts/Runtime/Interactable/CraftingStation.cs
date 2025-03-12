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

        private Recipe _selectedRecipe;

        private int _ptr;

        public List<Tile> Tiles { get; set; }

        public Transform GetTransform() => transform;

        public Sprite GetSprite() => _spriteRenderer.sprite;

        public Vector2Int GetGridSize() => new Vector2Int(3, 3);

        public void OnPlayerAdjancent(Interactor player)
        {
            player.NearbyCraftingStation = this;
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
            _selectedRecipe = GameManager.GetMonoSystem<ICraftingMonoSystem>().GetAllRecipes(_side)[++_ptr % GameManager.GetMonoSystem<ICraftingMonoSystem>().GetAllRecipes(_side).Count];
            ShowRecipe(_selectedRecipe);
        }

        public void UseCraftingStation(Interactor player)
        {
            Debug.Log("Uisng Crafting Bench!");
            if (player.HasCraftingMaterial())
            {
                CraftingMaterial cm = player.Item as CraftingMaterial;

                if (
                    _selectedRecipe.GetMaterials().Contains(cm.GetMaterialType()) && 
                    _selectedRecipe.GetProgress().Select(
                        m => m.GetMaterialType() == cm.GetMaterialType()
                    ).Count() != 
                    _selectedRecipe.GetMaterials().Select(
                        m => m == cm.GetMaterialType()
                    ).Count()
                )
                {
                    _selectedRecipe.GetProgress().Add(cm);
                    ShowRecipe(_selectedRecipe);
                    player.Item = null;
                }
            }
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
                Debug.Log(recipe.GetMaterials()[i]);
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

            if (recipe.CanCraft(recipe.GetProgress())) _spriteRenderer.sprite = _computerSprites[1];
            else _spriteRenderer.sprite = _computerSprites[0];
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
        }
    }
}
