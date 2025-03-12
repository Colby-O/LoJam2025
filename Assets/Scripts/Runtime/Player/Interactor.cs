using LoJam.Interactable;
using LoJam.Logic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

namespace LoJam.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Side _side;

        public IInteractable Item
        {
            get
            {
                return _item;
            }
            set
            {
                if (_item != null)
                {
                    GarbageCollecter gc = _item.GetTransform().GetComponent<GarbageCollecter>();
                    if (gc != null) gc.Pause = false;
                }
                _item = value;

                if (_item != null)
                {
                    GarbageCollecter gc = _item.GetTransform().GetComponent<GarbageCollecter>();
                    if (gc != null) gc.Pause = true;
                }
            }
        }

        private IInteractable _item;

        public CraftingStation NearbyCraftingStation { get; set; }

        public Side GetSide() => _side;

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
            NearbyCraftingStation.CompleteRecipe(this);
        }

        private void NextReecipe(InputAction.CallbackContext e)
        {
            if (NearbyCraftingStation == null) return;
            NearbyCraftingStation.SwitchRecipe();
        }

        private void Awake()
        {
            if (_input == null) _input = GetComponent<PlayerInput>();

            if (_side == Side.Left)
            {
                _input.actions["Interact"].performed += OpenCraftingStation;
                _input.actions["Craft"].performed += Craft;
                _input.actions["NextRecipe"].performed += NextReecipe;
            }
            else
            {
                _input.actions["Interact2"].performed += OpenCraftingStation;
                _input.actions["Craft2"].performed += Craft;
                _input.actions["NextRecipe2"].performed += NextReecipe;
            }
        }

        private void Update()
        {

        }
    }
}
