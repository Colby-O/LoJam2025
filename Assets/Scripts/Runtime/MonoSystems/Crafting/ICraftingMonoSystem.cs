using LoJam.Core;
using LoJam.Crafting;
using LoJam.Interactable;
using System.Collections.Generic;
using UnityEngine;

namespace LoJam.MonoSystem
{
    public interface ICraftingMonoSystem : IMonoSystem
    {
        public List<RecipeSO> FetchPossibleRecipes(List<CraftingMaterial> materials);
        public PowerupBase CraftRequest(List<CraftingMaterial> materials);
    }
}
