using UnityEngine;

namespace LoJam
{
    public class Hud : View
    {
        [SerializeField] private InventorySlots _inventorySlots;
        [SerializeField] private UIRecipes _recipes;
        [SerializeField] private UIPusher _pusher;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public InventorySlots GetInventory()
        {
            return _inventorySlots;
        }

        public UIRecipes GetRecipes()
        {
            return _recipes;
        }

        public UIPusher GetPusher()
        {
            return _pusher;
        }
    }
}