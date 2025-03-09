using LoJam.Core;
using UnityEngine;

namespace LoJam
{
    public class LoJamGameManager : GameManager
    {

        [Header("MonoSystem Holder")]
        [SerializeField] private GameObject _msHolder;

        //[Header("MonoSystems")]

        private void AddEvents() {
            //AddEvent<GameEvents.TestEvent>(DeleteMe);
        }

        private void AttachMonoSystems() {
            // Add MonoSystems Here
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
