using UnityEngine;

namespace LoJam.Interactable
{
    public interface IHoldable : IInteractable
    {
        public Sprite GetInventorySprite();
    }
}
