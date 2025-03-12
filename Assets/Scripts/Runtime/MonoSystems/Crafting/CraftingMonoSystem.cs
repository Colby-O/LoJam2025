using LoJam.Core;
using LoJam.Crafting;
using LoJam.Interactable;
using LoJam.Logic;
using LoJam.Player;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace LoJam.MonoSystem
{
    public class CraftingMonoSystem : MonoBehaviour, ICraftingMonoSystem
    {
        private const int RECIPE_SIZE = 3;

        [SerializeField] private List<PowerupBase> _powerups;

        private List<Recipe> _firewallRecipe;
        private List<Recipe> _powerupRecipe;

        public UnityEvent OnInit
        {
            get
            {
                if (_0nInit == null)
                {
                    _0nInit = new UnityEvent();
                }

                return _0nInit;
            }
            set
            {
                _0nInit = value;
            }
        }

        private UnityEvent _0nInit;

        public List<Recipe> GetAllRecipes(Side side) => new List<Recipe> { _firewallRecipe[(int)side], _powerupRecipe[(int)side] };

        public Recipe GetFirewallRecipe(Side side) => _firewallRecipe[(int)side];

        public void RefreshPowerupRecipe(Side side)
        {
            _powerupRecipe[(int)side].Refresh();
        }

        private void AddPushingStrength(Interactor player)
        {
            GameManager.GetMonoSystem<IGridMonoSystem>().AddFirewallDaemon(player.GetSide());
        }

        private void SpawnPowerup(Interactor player)
        {
            if (_powerups == null || _powerups.Count == 0)
            {
                Debug.LogWarning("Trying to spawn powerup but no prefabs are set.");
                return;
            }

            int maxTries = 100;
            int tries = 0;

            while (!GameManager.GetMonoSystem<IGridMonoSystem>().Spawn(player.GetSide(), _powerups[Random.Range(0, _powerups.Count)]))
            {
                if (++tries > maxTries)
                {
                    Debug.LogWarning($"Failed to spawn powerup. no vaild spawn location found on side {player.GetSide()}.");
                    break;
                }
            }
        }

        private void Start()
        {
            Recipe firewallRecipe = new Recipe(RECIPE_SIZE, true, "Firewall");

            _powerupRecipe = new List<Recipe>();
            _firewallRecipe = new List<Recipe>();
            for (int i = 0; i < LoJamGameManager.players.Count; i++)
            {
                Recipe r1 = new Recipe(firewallRecipe);
                r1.OnCraft.AddListener(AddPushingStrength);
                _firewallRecipe.Add(r1);

                Recipe r2 = new Recipe(RECIPE_SIZE, false, "Powerup");
                r2.OnCraft.AddListener(SpawnPowerup);
                _powerupRecipe.Add(r2);
            }

            OnInit?.Invoke();
        }
    }
}
