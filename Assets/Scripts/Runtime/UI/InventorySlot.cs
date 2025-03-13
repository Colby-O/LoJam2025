using UnityEngine;
using UnityEngine.UI;

namespace LoJam
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Image _slotItem;
        [SerializeField] private Image _slotContainer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void SetItemSprite(Sprite sprite)
        {
            _slotItem.sprite = sprite;

            if (SlotEmpty)
            {
                _slotItem.enabled = false;
            }
            else
            {
                _slotItem.enabled = true;
            }
        }

        public bool SlotEmpty => _slotItem.sprite == null;
    }
}
