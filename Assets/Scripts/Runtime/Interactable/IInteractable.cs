using LoJam.Grid;
using LoJam.Player;
using System.Collections.Generic;
using UnityEngine;

namespace LoJam.Interactable
{
    public interface IInteractable
    {
        public List<Tile> Tiles { get; set; }
        public Sprite GetSprite();
        public Transform GetTransform();
        public Vector2Int GetGridSize();
        public void OnPlayerEnter(Interactor player);
        public void OnPlayerAdjancent(Interactor player);
        public void OnPlayerExit(Interactor player);
        public void OnPlayerAdjancentExit(Interactor player);
    }
}
