using LoJam.Grid;
using LoJam.Player;
using UnityEngine;

namespace LoJam.Interactable
{
    public interface IInteractable
    {
        public Tile Tile { get; set; }
        public Sprite GetSprite();
        public void OnPlayerEnter(Interactor player);
        public void OnPlayerAdjancent(Interactor player);
        public void OnPlayerExit(Interactor player);
        public void OnPlayerAdjancentExit(Interactor player);
    }
}
