using LoJam.Crafting;
using LoJam.Interactable;
using LoJam.Logic;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace LoJam.MonoSystem
{
    public class CraftingMonoSystem : MonoBehaviour, ICraftingMonoSystem
    {
        private const int RECIPE_SIZE = 3;

        private Recipe _firewallRecipe;
        private List<Recipe> _powerupRecipe;

        public List<Recipe> GetAllRecipes(Side side) => new List<Recipe> { _firewallRecipe, _powerupRecipe[(int)side]};

        private void Start()
        {
            _firewallRecipe = new Recipe(RECIPE_SIZE, true);
            _powerupRecipe = new List<Recipe>();
            for (int i = 0; i < LoJamGameManager.players.Count; i++)
            {
                Recipe r = new Recipe(RECIPE_SIZE, false);
                //r.OnCraft.AddListener(/*Spawn Powerup */);
                _powerupRecipe.Add(r);
            }
        }
    }
}
