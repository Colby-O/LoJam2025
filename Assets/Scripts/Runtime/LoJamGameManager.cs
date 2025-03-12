using LoJam.Core;
using LoJam.MonoSystem;
using LoJam.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoJam
{
    public sealed class LoJamGameManager : GameManager
    {

        [Header("MonoSystem Holder")]
        [SerializeField] private GameObject _msHolder;

        [Header("MonoSystems")]
        [SerializeField] private GridMonoSystem _gridSystem;
        [SerializeField] private UIMonoSystem _uiSystem;
        [SerializeField] private CraftingMonoSystem _craftingSystem;

        public static List<Interactor> players;

        private void AddEvents() {
            //AddEvent<GameEvents.TestEvent>(DeleteMe);
        }

        private void AttachMonoSystems() {

            AddMonoSystem<GridMonoSystem, IGridMonoSystem>(_gridSystem);
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiSystem);
            AddMonoSystem<CraftingMonoSystem, ICraftingMonoSystem>(_craftingSystem);
        }

        protected override void OnLoad() {
            // Attaches MonoSystem
            AttachMonoSystems();

            // Added Events
            AddEvents();

            // Ensure all MonoSystems call Awake at the same time
            _msHolder.SetActive(true);
        }

        private void Start()
        {
            players = FindObjectsByType<Interactor>(FindObjectsSortMode.None).ToList();

            _uiSystem.PushView(FindObjectsByType<MainMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault());
        }
    }
}
