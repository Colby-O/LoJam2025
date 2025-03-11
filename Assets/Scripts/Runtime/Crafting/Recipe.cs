using LoJam.Core;
using LoJam.Interactable;
using LoJam.Player;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LoJam.Crafting
{
    public class Recipe
    {
        public UnityEvent<Interactor> OnCraft { get; set; }

        public string Label { get; set; }

        private List<MaterialType> _recipe;

        private List<CraftingMaterial> _progress;

        private bool _isStatic;
        private int _size;

        public Recipe(int size, bool isStatic = false, string label = "")
        {
            _isStatic = isStatic;
            _size = size;

            Label = label;

            _progress = new List<CraftingMaterial>();

            OnCraft = new UnityEvent<Interactor>();

            GnerateNewRecipe(_size);

            OnCraft.AddListener(Refresh);
        }

        public List<MaterialType> GetMaterials() => _recipe;

        public bool CanCraft(List<CraftingMaterial> materials)
        {
            return _recipe.All(mat => materials.FirstOrDefault(m => m.GetMaterialType() == mat) != null) && _progress.Count == _recipe.Count;
        }

        public List<CraftingMaterial> GetProgress()
        {
            return _progress;
        }

        public bool Craft(Interactor player)
        {
            if (CanCraft(_progress))
            {
                OnCraft.Invoke(player);
                return true;
            }
            return false;
        }

        public void Refresh(Interactor _ = null)
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
        }

        private MaterialType GetRandomMaterial()
        {
            return (MaterialType)Random.Range(0, (int)System.Enum.GetValues(typeof(MaterialType)).Cast<MaterialType>().Max());
        }
    }
}
