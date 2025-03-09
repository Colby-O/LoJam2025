using UnityEngine;

namespace LoJam.Interactable
{
    public interface IInteractable
    {
        // NOTE: Will take in a reference to player script
        public void OnPlayerEnter();
        // NOTE: Will take in a reference to player script
        public void OnPlayerAdjancent(); 
    }
}
