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
using static UnityEditor.Progress;

namespace LoJam.Interactable
{
    public enum StationType
    {
        Main,
        Circle,
        Triangle,
        Square,
        Cross
    }

    public class CraftingStation : MonoBehaviour, IInteractable
    {
        [SerializeField] protected SpriteRenderer _spriteRenderer;

        [SerializeField] private TMP_Text _label;

        [SerializeField] private List<SpriteRenderer> _itemsUI;

        [SerializeField] private GameObject _lighting;

        [SerializeField] private List<Sprite> _computerSprites;

        [SerializeField] protected SpriteRenderer _center;
        [SerializeField] private List<Sprite> _centerSprites;


        [SerializeField] private SerializableDictionary<MaterialType, List<Sprite>> _sprites;

        [SerializeField] float _craftingAnimSpeed = 1f;

        [SerializeField] Vector2Int _bounds = new Vector2Int(3, 4);

        [SerializeField] Side _side;

        [SerializeField] StationType _stationType;

        [SerializeField] private Recipe _selectedRecipe;

        [SerializeField, ColorUsage(true, true)] Color _wireOffColor;
        [SerializeField, ColorUsage(true, true)] Color _wireOnColor;

        private bool _isFire;
        [SerializeField] private GameObject _fireIcon;
        [SerializeField] private GameObject _powerupIcon;
        [SerializeField] private List<GameObject> _icon;

        private int _ptr;

        private float _hackDuration;
        private float _hackedTIme = 0;
        private bool _hacked = false;

        public List<Tile> Tiles { get; set; }

        public void SetStationType(StationType t) => _stationType = t;

        public StationType GetStationType() => _stationType;

        public Transform GetTransform() => transform;

        public Sprite GetSprite() => _spriteRenderer.sprite;

        public Vector2Int GetGridSize() => _bounds;

        public Side GetSide() => _side;

        public void Hack(bool state, float duration)
        {
            _hackedTIme = 0;
            _hackDuration = duration;
            _hacked = state;

            _fireIcon.SetActive(!state && _isFire);
            _powerupIcon.SetActive(!state && !_isFire);
        }

        public void OnPlayerAdjancent(Interactor player)
        {
            player.NearbyCraftingStation = this;
            //UseCraftingStation(player);
        }

        public void OnPlayerEnter(Interactor player) { }

        public void OnPlayerExit(Interactor player) { }

        public void OnPlayerAdjancentExit(Interactor player)
        {
            player.NearbyCraftingStation = null;
        }

        public void CompleteRecipe(Interactor player)
        {
            if (_selectedRecipe.Craft(player, this)) 
            {
                StartCoroutine(CraftingAnimation());
            }
        }

        public void SwitchRecipe()
        {
            if (_hacked) return;
            _isFire = !_isFire;
            _selectedRecipe = GameManager.GetMonoSystem<ICraftingMonoSystem>().GetAllRecipes(_side, _stationType)[++_ptr % GameManager.GetMonoSystem<ICraftingMonoSystem>().GetAllRecipes(_side, _stationType).Count];
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
                    GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(2);
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
                if (recipe.GetMaterials().Count <= i)
                {
                    _itemsUI[i].gameObject.SetActive(false);
                }
                else
                {
                    _itemsUI[i].gameObject.SetActive(true);
                    List<Sprite> cs = _sprites[recipe.GetMaterials()[i]];
                    _itemsUI[i].sprite = cs[0];
                }

                _itemsUI[i].transform.parent.GetComponent<SpriteRenderer>().material.SetColor("_Intensity", _wireOffColor);
                _itemsUI[i].transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_Intensity", _wireOffColor);
            }

            if (recipe.GetMaterials().Count == 1)
            {
                _itemsUI[0].gameObject.SetActive(false);
                _itemsUI[1].gameObject.SetActive(false);
                _itemsUI[2].gameObject.SetActive(true);

                _itemsUI[2].sprite = _sprites[recipe.GetMaterials()[0]][(recipe.GetProgress().Count == 0) ? 0 : 1];

                _itemsUI[2].transform.parent.GetComponent<SpriteRenderer>().material.SetColor("_Intensity", _wireOffColor);
                _itemsUI[2].transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_Intensity", _wireOffColor);
            }
            else
            {
                for (int i = 0; i < _itemsUI.Count; i++)
                {
                    if (i < recipe.GetProgress().Count && copyRecipe.Contains(recipe.GetProgress()[i].GetMaterialType()))
                    {
                        int index = copyRecipe.IndexOf(recipe.GetProgress()[i].GetMaterialType());
                        List<Sprite> cs = _sprites[recipe.GetMaterials()[index]];
                        copyRecipe[index] = MaterialType.None;
                        _itemsUI[index].sprite = cs[1];
                        _itemsUI[index].transform.parent.GetComponent<SpriteRenderer>().material.SetColor("_Intensity", _wireOnColor);
                        _itemsUI[index].transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_Intensity", _wireOnColor);
                    }
                }
            }

            if (_stationType == StationType.Main)
            {
                _fireIcon.SetActive(!recipe.CanCraft(recipe.GetProgress()) && !_hacked && _isFire);
                _powerupIcon.SetActive(!recipe.CanCraft(recipe.GetProgress()) && !_hacked && !_isFire);
            }
            else if (_stationType == StationType.Circle)
            {
                _icon[0].SetActive(!_hacked);
            }
            else if (_stationType == StationType.Triangle) 
            {
                _icon[1].SetActive(!recipe.CanCraft(recipe.GetProgress()) && !_hacked);
            }
            else if (_stationType == StationType.Square)
            {
                _icon[2].SetActive(!recipe.CanCraft(recipe.GetProgress()) && !_hacked);
            }
            else if (_stationType == StationType.Cross)
            {
                _icon[3].SetActive(!recipe.CanCraft(recipe.GetProgress()) && !_hacked);
            }

            if (recipe.CanCraft(recipe.GetProgress()) && recipe.GetMaterials().Count > 0) _spriteRenderer.sprite = _computerSprites[1];
            else _spriteRenderer.sprite = _computerSprites[0];

            bool found = LoJamGameManager.GetMonoSystem<IUIMonoSystem>().GetViews().TryGetValue("HUD", out View view);
            if (found && view is Hud hud)
            {
                hud.GetRecipes().SetRecipeText(GetSide(), recipe);
            }
        }

        private void Init()
        {
            if (_stationType == StationType.Main)
            {
                _center.sprite = _centerSprites[0];
            }
            else if (_stationType == StationType.Circle)
            {
                _center.sprite = _centerSprites[1];
            }
            else if (_stationType == StationType.Triangle)
            {
                _center.sprite = _centerSprites[2];
            }
            else if (_stationType == StationType.Square)
            {
                _center.sprite = _centerSprites[3];
            }
            else if (_stationType == StationType.Cross)
            {
                _center.sprite = _centerSprites[4];
            }

            _selectedRecipe = GameManager.GetMonoSystem<ICraftingMonoSystem>().GetAllRecipes(_side, _stationType)[0];
            _isFire = true;
            ShowRecipe(_selectedRecipe);
        }

        private void Awake()
        {
            GameManager.GetMonoSystem<ICraftingMonoSystem>().OnInit.AddListener(Init);
            _lighting.SetActive(false);
            _spriteRenderer.sprite = _computerSprites[1];

            _fireIcon.SetActive(false);
            _powerupIcon.SetActive(false);
            foreach (GameObject icon in _icon) icon.SetActive(false);
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
