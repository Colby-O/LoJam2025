using UnityEngine;

namespace LoJam.Interactable
{
    public interface IInteractable
    {
        public void OnPlayerEnter();
        public void OnPlayerAdjacent(); 
    }
}
