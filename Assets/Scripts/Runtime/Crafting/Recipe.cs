using LoJam.Core;
using LoJam.Interactable;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace LoJam.Crafting
{
    public class Recipe
    {
        public UnityEvent<Inspector> OnCraft { get; set; }

        private List<MaterialType> _recipe;

        private bool _isStatic;
        private int _size;

        public Recipe(int size, bool isStatic = false)
        {
            _isStatic = isStatic;
            _size = size;

            OnCraft = new UnityEvent<Inspector>();

            GnerateNewRecipe(_size);

            OnCraft.AddListener(Refresh);
        }

        public List<MaterialType> GetMaterials() => _recipe;

        public bool CanCraft(List<CraftingMaterial> materials)
        {
            return _recipe.All(mat => materials.FirstOrDefault(m => m.GetMaterialType() == mat) != null);
        }

        private void Refresh(Inspector _)
        {
            if (!_isStatic)
            {
                GnerateNewRecipe(_size);
            }
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
