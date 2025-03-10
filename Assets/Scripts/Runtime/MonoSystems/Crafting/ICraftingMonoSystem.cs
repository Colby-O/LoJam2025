using LoJam.Crafting;
using LoJam.Interactable;
using System.Collections.Generic;
using UnityEngine;

namespace LoJam.MonoSystem
{
    public interface ICraftingMonoSystem : IUIMonoSystem
    {
        public List<RecipeSO> FetchPossibleRecipes(List<CraftingMaterial> materials);
        public PowerupBase CraftRequest(List<CraftingMaterial> materials);
    }
}
