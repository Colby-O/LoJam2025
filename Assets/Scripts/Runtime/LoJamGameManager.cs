using LoJam.Core;
using LoJam.MonoSystem;
using UnityEngine;

namespace LoJam
{
    public class LoJamGameManager : GameManager
    {

        [Header("MonoSystem Holder")]
        [SerializeField] private GameObject _msHolder;

        [Header("MonoSystems")]
        [SerializeField] private GridMonoSystem _grid;
        [SerializeField] private UIMonoSystem _uiSystem;

        private void AddEvents() {
            //AddEvent<GameEvents.TestEvent>(DeleteMe);
        }

        private void AttachMonoSystems() {

            AddMonoSystem<GridMonoSystem, IGridMonoSystem>(_grid);
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiSystem);
        }

        protected override void OnLoad() {
            // Attaches MonoSystem
            AttachMonoSystems();

            // Added Events
            AddEvents();

            // Ensure all MonoSystems call Awake at the same time
            _msHolder.SetActive(true);
        }
    }
}
