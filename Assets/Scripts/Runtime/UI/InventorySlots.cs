using LoJam.Interactable;
using LoJam.Logic;
using System.Collections;
using UnityEngine;

namespace LoJam
{
    public class InventorySlots : MonoBehaviour
    {
        [SerializeField] private InventorySlot _slotL;
        [SerializeField] private InventorySlot _slotR;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateInventorySlot(Side side, IHoldable holdable)
        {
            UpdateInventorySlot(side, holdable.GetInventorySprite());
        }

        public void UpdateInventorySlot(Side side, Sprite sprite)
        {
            InventorySlot slot = side == Side.Left ? _slotL : _slotR;
            slot.SetItemSprite(sprite);
        }
    }
}