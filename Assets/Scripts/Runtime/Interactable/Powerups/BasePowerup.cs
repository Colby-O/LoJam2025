using LoJam.Grid;
using UnityEngine;

namespace LoJam.Interactable
{
    public abstract class BasePowerup : MonoBehaviour, IInteractable
    {
        public Tile Tile { get; set; }

        public abstract void OnPlayerAdjancent();

        public abstract void OnPlayerEnter();
    }
}
