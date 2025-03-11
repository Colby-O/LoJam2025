using LoJam.Core;
using LoJam.Crafting;
using LoJam.Grid;
using LoJam.MonoSystem;
using LoJam.Player;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LoJam.Interactable
{
    public class CraftingStation : MonoBehaviour, IInteractable
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        public Tile Tile { get; set; }

        public Transform GetTransform() => transform;

        public Sprite GetSprite() => _spriteRenderer.sprite;

        [SerializeField] private List<CraftingMaterial> _materials;
        [SerializeField] private List<SpriteRenderer> _itemsUI;
        [SerializeField] private SpriteRenderer _resUI;

        private List<Recipe> _possibleResults;

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
            if (_possibleResults.Count > 0 && _possibleResults[0].CanCraft(_materials)) 
            {
                foreach (SpriteRenderer sr in _itemsUI) sr.sprite = null;
                _resUI.sprite = null;

                for (int i = 0; i < _materials.Count; i++)
                {
                    Destroy(_materials[i].gameObject);
                }

                _materials.Clear();
            }
        }

        public void UseCraftingStation(Interactor player)
        {
            Debug.Log("Uisng Crafting Bench!");
            //if (player.HasCraftingMaterial())
            //{
            //    if (_materials.Count < 3)
            //    {
            //        AddCraftingMaterial(player.Item as CraftingMaterial);
            //        player.Item = null;
            //    }
            //}
            //else
            //{
            //    if (!player.HasAnyItem() && _materials.Count > 0)
            //    {
            //        player.Item = _materials[0];
            //        RemoveCraftingMaterial(0);
            //    }
            //}
        }

        private void AddCraftingMaterial(CraftingMaterial mat)
        {
            //_materials.Add(mat);

            //_itemsUI[_materials.Count - 1].sprite = mat.GetSprite();

            //_possibleResults = GameManager.GetMonoSystem<ICraftingMonoSystem>().FetchPossibleRecipes(_materials);
            //_resUI.sprite = _possibleResults[0].result.GetSprite();
            //_resUI.color = new Color(_resUI.color.r, _resUI.color.g, _resUI.color.b, (_possibleResults[0].CanCraft(_materials)) ? 1 : 0.5f);
        }

        private void RemoveCraftingMaterial(int index)
        {
            //_materials.RemoveAt(index);

            //foreach (SpriteRenderer sr in _itemsUI) sr.sprite = null;
            //for (int i = 0; i < _materials.Count; i++) _itemsUI[i].sprite = _materials[i].GetSprite();
            
            //_possibleResults = GameManager.GetMonoSystem<ICraftingMonoSystem>().FetchPossibleRecipes(_materials);
            //if (_possibleResults.Count > 0)
            //{
            //    _resUI.sprite = _possibleResults[0].result.GetSprite();
            //    _resUI.color = new Color(_resUI.color.r, _resUI.color.g, _resUI.color.b, (_possibleResults[0].CanCraft(_materials)) ? 1f : 0.5f);
            //}
            //else _resUI.sprite = null;
        }

        private void Awake()
        {
            //if (_spriteRenderer == null) _spriteRenderer  = GetComponent<SpriteRenderer>();

            //foreach (SpriteRenderer renderer in _itemsUI) renderer.sprite = null;
            //_resUI.sprite = null;
        }

    }
}
