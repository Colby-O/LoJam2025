using LoJam.Core;
using LoJam.Interactable;
using LoJam.Player;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LoJam.Crafting
{
    [System.Serializable]
    public class Recipe
    {
        public UnityEvent<Interactor, CraftingStation> OnCraft { get; set; }

        public string Label { get; set; }

        private List<MaterialType> _recipe;

        public List<CraftingMaterial> _progress;

        private bool _isStatic;
        private int _size;

        public Recipe(int size, bool isStatic = false, string label = "")
        {
            _isStatic = isStatic;
            _size = size;

            Label = label;

            _progress = new List<CraftingMaterial>();

            OnCraft = new UnityEvent<Interactor, CraftingStation>();

            GnerateNewRecipe(_size);

            OnCraft.AddListener(Refresh);
        }

        public Recipe(Recipe recipe)
        {
            _isStatic = recipe.GetIsStatic();
            _size = recipe.GetSize();
            Label = recipe.Label;

            _recipe = new List<MaterialType>(recipe.GetMaterials());

            _progress = new List<CraftingMaterial>();
            OnCraft = new UnityEvent<Interactor, CraftingStation>();
            OnCraft.AddListener(Refresh);
        }

        public Recipe(List<MaterialType> recipe, bool isStatic = false, string label = "")
        {
            _isStatic = isStatic;
            _size = recipe.Count;

            Label = label;

            _progress = new List<CraftingMaterial>();

            OnCraft = new UnityEvent<Interactor, CraftingStation>();

            _recipe = new List<MaterialType>(recipe);

            OnCraft.AddListener(Refresh);
        }

        public bool GetIsStatic() => _isStatic;

        public int GetSize() => _size;

        public List<MaterialType> GetMaterials() => _recipe;

        public bool CanCraft(List<CraftingMaterial> materials)
        {
            return _recipe.All(mat => materials.FirstOrDefault(m => m.GetMaterialType() == mat) != null) && _progress.Count == _recipe.Count;
        }

        public List<CraftingMaterial> GetProgress()
        {
            return _progress;
        }

        public bool Craft(Interactor player, CraftingStation station)
        {
            if (CanCraft(_progress))
            {
                OnCraft.Invoke(player, station);
                return true;
            }
            return false;
        }

        public void Refresh(Interactor _ = null, CraftingStation __ = null)
        {
            if (!_isStatic)
            {
                GnerateNewRecipe(_size);
            }

            _progress.Clear();
        }

        private void GnerateNewRecipe(int size)
        {
            if (_recipe != null) _recipe.Clear();
            else _recipe = new List<MaterialType>();

            for (int i = 0; i < size; i++)
            {
                _recipe.Add(GetRandomMaterial());
            }

            while (_recipe.Where(r => r == MaterialType.Cross).Count() > 1)
            {
                _recipe[_recipe.IndexOf(MaterialType.Cross)] = (MaterialType)UnityEngine.Random.Range(0, 3);
            }

            while (_recipe.Where(r => r == MaterialType.Square).Count() > 2)
            {
                _recipe[_recipe.IndexOf(MaterialType.Square)] = (MaterialType)UnityEngine.Random.Range(0, 2);
            }
        }

        private MaterialType GetRandomMaterial()
        {
            return (MaterialType)UnityEngine.Random.Range(0, (int)System.Enum.GetValues(typeof(MaterialType)).Cast<MaterialType>().Max());
        }
    }
}
