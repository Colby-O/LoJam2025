using UnityEngine;

namespace LoJam
{
    public class Hud : View
    {
        [SerializeField] private InventorySlots _inventorySlots;
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
    }
}