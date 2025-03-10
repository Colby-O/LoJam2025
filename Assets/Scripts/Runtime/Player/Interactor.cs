using LoJam.Interactable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LoJam.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;

        public IInteractable Item { get; set; }

        public CraftingStation NearbyCraftingStation { get; set; }

        public bool HasAnyItem() => Item != null;

        public bool HasCraftingMaterial() => HasAnyItem() && Item is CraftingMaterial;

        private void OpenCraftingStation(InputAction.CallbackContext e)
        {
            if (NearbyCraftingStation == null) return;
            NearbyCraftingStation.UseCraftingStation(this);
        }

        private void Craft(InputAction.CallbackContext e)
        {
            if (NearbyCraftingStation == null || HasAnyItem()) return;
            NearbyCraftingStation.Craft(this);
        }

        private void Awake()
        {
            if (_input == null) _input = GetComponent<PlayerInput>();

            _input.actions["Interact"].performed += OpenCraftingStation;
            _input.actions["Craft"].performed += Craft;
        }

        private void Update()
        {
            Debug.Log(Item);
        }
    }
}
