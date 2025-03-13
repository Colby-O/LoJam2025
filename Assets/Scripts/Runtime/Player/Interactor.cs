using LoJam.Interactable;
using LoJam.Logic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

namespace LoJam.Player
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Side _side;

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

        public CraftingStation NearbyCraftingStation { get; set; }

        public Side GetSide() => _side;

        public bool HasAnyItem() => Item != null;

        public bool HasCraftingMaterial() => HasAnyItem() && Item is CraftingMaterial;

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
                _input.actions["NextRecipe"].performed += NextReecipe;
            }
            else
            {
                _input.actions["NextRecipe2"].performed += NextReecipe;
            }
        }
    }
}
