using LoJam.Grid;
using UnityEngine;

namespace LoJam.Interactable
{
    public interface IInteractable
    {
        public Tile Tile { get; set; }

        // NOTE: Will take in a reference to player script
        public void OnPlayerEnter();
        // NOTE: Will take in a reference to player script
        public void OnPlayerAdjancent(); 
    }
}
