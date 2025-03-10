using LoJam.Core;
using LoJam.Interactable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoJam.Crafting
{
    [CreateAssetMenu(fileName = "DefaultRecipe", menuName = "Crafting/Recipe")]
    public class RecipeSO : BaseSO
    {
        public List<MaterialType> recipe;
        public PowerupBase result;

        public bool CanCraft(List<CraftingMaterial> materials)
        {
            return recipe.All(mat => materials.FirstOrDefault(m => m.GetMaterialType() == mat) != null);
        }
    }

}
