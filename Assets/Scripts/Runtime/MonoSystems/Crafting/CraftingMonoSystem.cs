using LoJam.Crafting;
using LoJam.Interactable;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoJam.MonoSystem
{
    public class CraftingMonoSystem : MonoBehaviour, ICraftingMonoSystem
    {
        [SerializeField] private RecipeDatabase _db;

        public List<RecipeSO> FetchPossibleRecipes(List<CraftingMaterial> materials)
        {
            List<RecipeSO> recipes = _db.GetAllEntries();

            List<RecipeSO> possibleRecipes = recipes.FindAll(recipe => materials.All(mat => recipe.recipe.Contains(mat.GetMaterialType())));

            return possibleRecipes;
        }

        public PowerupBase CraftRequest(List<CraftingMaterial> materials)
        {
            List<RecipeSO> recipes = _db.GetAllEntries();

            RecipeSO recipe = recipes.Find(recipe => recipe.CanCraft(materials));

            return (recipe != null) ? recipe.result : null;
        }

        private void Awake()
        {
            _db.InitDatabase();
        }
    }
}
