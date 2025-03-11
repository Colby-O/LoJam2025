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

        private Recipe _firewallRecipe;
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

        public List<Recipe> GetAllRecipes(Interactor player) => new List<Recipe> { _firewallRecipe, _powerupRecipe[(int)player.GetSide()]};

        public Recipe GetFirewallRecipe() => _firewallRecipe;

        public void RefreshPowerupRecipe(Interactor player)
        {
            _powerupRecipe[(int)player.GetSide()].Refresh();
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
            _firewallRecipe = new Recipe(RECIPE_SIZE, true, "Firewall");
            _firewallRecipe.OnCraft.AddListener(AddPushingStrength);

            _powerupRecipe = new List<Recipe>();
            for (int i = 0; i < LoJamGameManager.players.Count; i++)
            {
                Recipe r = new Recipe(RECIPE_SIZE, false, "Powerup");
                r.OnCraft.AddListener(SpawnPowerup);
                _powerupRecipe.Add(r);
            }

            OnInit?.Invoke();
        }
    }
}
