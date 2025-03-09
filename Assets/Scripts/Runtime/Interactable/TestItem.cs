using UnityEngine;

namespace LoJam.Interactable
{
    public class TestItem : IInteractable
    {
        public void OnPlayerEnter() {
            Debug.Log("Get off of me YO!");
        }
        public void OnPlayerAdjancent() {
            Debug.Log("Don't come any closer!");
        }
    }
}
