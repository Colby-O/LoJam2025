using LoJam.Core;
using LoJam.Crafting;
using LoJam.Interactable;
using LoJam.Logic;
using LoJam.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LoJam.MonoSystem
{
    public interface ICraftingMonoSystem : IMonoSystem
    {
        public UnityEvent OnInit { get; set; }
        public void Restart();
        public List<Recipe> GetAllRecipes(Side side, StationType type);
        public Recipe GetFirewallRecipe(Side side);
        public void RefreshPowerupRecipe(Side side);
    }
}
