using LoJam.Core;
using LoJam.Crafting;
using LoJam.Interactable;
using LoJam.Logic;
using LoJam.Player;
using NUnit.Framework;
using System.Collections;
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
        [SerializeField] private List<CraftingMaterial> _materialPickups;

        private List<Recipe> _firewallRecipe;
        private List<Recipe> _powerupRecipe;

        private List<Recipe> _circleRecipe;
        private List<Recipe> _triangleRecipe;
        private List<Recipe> _squareRecipe;
        private List<Recipe> _crossRecipe;

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

        public List<Recipe> GetAllRecipes(Side side, StationType type) {

            if (type == StationType.Main) return new List<Recipe> { _firewallRecipe[(int)side], _powerupRecipe[(int)side] };
            else if (type == StationType.Circle) return new List<Recipe> { _circleRecipe[(int)side] };
            else if (type == StationType.Triangle) return new List<Recipe> { _triangleRecipe[(int)side] };
            else if (type == StationType.Square) return new List<Recipe> { _squareRecipe[(int)side] };
            else if (type == StationType.Cross) return new List<Recipe> { _crossRecipe[(int)side] };
            else
            {
                Debug.Log("Something is really wrong if this ever gets called. I'm in the CraftingMonoSystem.");
                return null;
            }
        }

        public Recipe GetFirewallRecipe(Side side) => _firewallRecipe[(int)side];

        public void RefreshPowerupRecipe(Side side)
        {
            _powerupRecipe[(int)side].Refresh();
        }

        private void AddPushingStrength(Interactor player, CraftingStation _)
        {
            GameManager.GetMonoSystem<IGridMonoSystem>().AddFirewallDaemon(player.GetSide());
        }

        private void SpawnPowerup(Interactor player, CraftingStation _)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(1);

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

        private IEnumerator ItemAnimation(CraftingMaterial mat,  Vector3 start, Vector3 end)
        {
            float prog = 0;

            while (prog < 1)
            {
                prog += 0.1f / (end - start).magnitude;
                mat.transform.position = Vector3.Lerp(start, end, prog);
                yield return new WaitForEndOfFrame();
            }
        }

        private void SpawnMaterial(Interactor player, CraftingStation cs)
        {
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(1);

            if (_materialPickups == null || _materialPickups.Count == 0)
            {
                Debug.LogWarning("Trying to smaterial but no prefabs are set.");
                return;
            }

            int maxTries = 100;
            int tries = 0;

            CraftingMaterial mat = null;

            if (cs.GetStationType() == StationType.Circle) mat = _materialPickups[0];
            else if (cs.GetStationType() == StationType.Triangle) mat = _materialPickups[1];
            else if (cs.GetStationType() == StationType.Square) mat = _materialPickups[2];
            else if (cs.GetStationType() == StationType.Cross) mat = _materialPickups[3];

            if (mat == null) 
            {
                Debug.LogWarning("I Guess the main craftng station os calling this for some reason.");
                return;
            }

            CraftingMaterial instance = null;
             while (!GameManager.GetMonoSystem<IGridMonoSystem>().Spawn(player.GetSide(), mat, out instance))
             {
                if (++tries > maxTries)
                {
                    Debug.LogWarning($"Failed to spawn material. no vaild spawn location found on side {player.GetSide()}.");
                    break;
                }
             }

            if (instance == null) return;

            Vector3 start = cs.transform.position;
            Vector3 end = instance.transform.position;
            StartCoroutine(ItemAnimation(instance, start, end));
        }

        private void Start()
        {
            Recipe firewallRecipe = new Recipe(RECIPE_SIZE, true, "Firewall");

            _powerupRecipe = new List<Recipe>();
            _firewallRecipe = new List<Recipe>();

            _circleRecipe = new List<Recipe>();
            _triangleRecipe = new List<Recipe>();
            _squareRecipe = new List<Recipe>();
            _crossRecipe = new List<Recipe>();

            for (int i = 0; i < LoJamGameManager.players.Count; i++)
            {
                Recipe r1 = new Recipe(firewallRecipe);
                r1.OnCraft.AddListener(AddPushingStrength);
                _firewallRecipe.Add(r1);

                Recipe r2 = new Recipe(RECIPE_SIZE, false, "Powerup");
                r2.OnCraft.AddListener(SpawnPowerup);
                _powerupRecipe.Add(r2);

                Recipe r3 = new Recipe(new List<MaterialType>() { }, true, "Cricle");
                r3.OnCraft.AddListener(SpawnMaterial);
                _circleRecipe.Add(r3);

                Recipe r4 = new Recipe(new List<MaterialType>() { }, true, "Cricle");
                r4.OnCraft.AddListener(SpawnMaterial);
                _triangleRecipe.Add(r4);

                Recipe r5 = new Recipe(new List<MaterialType>() { MaterialType.Circle, MaterialType.Triangle }, true, "Cricle");
                r5.OnCraft.AddListener(SpawnMaterial);
                _squareRecipe.Add(r5);

                Recipe r6 = new Recipe(new List<MaterialType>() { MaterialType.Square, MaterialType.Triangle }, true, "Cricle");
                r6.OnCraft.AddListener(SpawnMaterial);
                _crossRecipe.Add(r6);
            }

            OnInit?.Invoke();
        }
    }
}
