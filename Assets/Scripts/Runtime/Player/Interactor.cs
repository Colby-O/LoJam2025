using LoJam.Interactable;
using LoJam.Logic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace LoJam.Player
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Side _side;

        static List<int> _registeredControllers = new();
        public int myId = -1;

        static List<int> _registeredControllers = new();
        public int myId = -1;

        public IHoldable Item
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

                bool found = LoJamGameManager.GetMonoSystem<IUIMonoSystem>().GetViews().TryGetValue("HUD", out View view);
                if (found && view is Hud hud)
                {
                    hud.GetInventory().UpdateInventorySlot(GetSide(), _item);
                }
            }
        }

        private IHoldable _item;

        private CraftingStation _lastNearbyCraftingStation;

        public CraftingStation NearbyCraftingStation { get; set; }

        public Side GetSide() => _side;

        public bool HasAnyItem() => Item != null;

        public bool HasCraftingMaterial() => HasAnyItem() && Item is CraftingMaterial;

        public void RegisterController(int id)
        {
            if (_registeredControllers.Contains(id)) return;
            _registeredControllers.Add(id);
            myId = id;
        }

        public static void ResetRegisteredControllerList()
        {
            _registeredControllers.Clear();
        }

        private void NextReecipe(InputAction.CallbackContext e)
        {
            if (NearbyCraftingStation == null) return;
            InputDevice device = e.control.device;
            if (device != null)
            {
                if (device is Gamepad)
                {
                    if (this.myId == -1) RegisterController(device.deviceId);
                    if (device.deviceId == this.myId)
                    {
                        NearbyCraftingStation.SwitchRecipe();
                    }
                }
                else if (device is Keyboard)
                {
                    NearbyCraftingStation.SwitchRecipe();
                }
            }
        }

        private void Awake()
        {
            if (_input == null) _input = GetComponent<PlayerInput>();

            if (_side == Side.Left)
            {
                _input.actions["NextRecipe"].performed += NextReecipe;
            }
            else
            {
                _input.actions["NextRecipe2"].performed += NextReecipe;
            }
        }

        private void LateUpdate()
        {
            if (NearbyCraftingStation != null && _lastNearbyCraftingStation != NearbyCraftingStation) NearbyCraftingStation.UseCraftingStation(this);
            _lastNearbyCraftingStation = NearbyCraftingStation;
        }
    }
}
