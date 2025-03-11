using LoJam.Core;
using LoJam.Crafting;
using LoJam.Interactable;
using LoJam.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LoJam.MonoSystem
{
    public interface ICraftingMonoSystem : IMonoSystem
    {
        public UnityEvent OnInit { get; set; }
        public List<Recipe> GetAllRecipes(Interactor player);
        public Recipe GetFirewallRecipe();
        public void RefreshPowerupRecipe(Interactor player);
    }
}
