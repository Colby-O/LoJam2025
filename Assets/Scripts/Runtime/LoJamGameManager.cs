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

        private void AddEvents() {
            //AddEvent<GameEvents.TestEvent>(DeleteMe);
        }

        private void AttachMonoSystems() {
            AddMonoSystem<GridMonoSystem, IGridMonoSystem>(_grid);
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
