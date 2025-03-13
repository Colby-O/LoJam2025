using LoJam.Player;
using UnityEngine;

namespace LoJam.Interactable
{
    public class HackPowerup : PowerupBase
    {
        [SerializeField] private float _duration = 15f;

        public override void OnPlayerAdjancent(Interactor player) { }
        public override void OnPlayerAdjancentExit(Interactor player) { }

        public override void OnPlayerExit(Interactor player) { }

        public override void OnPlayerEnter(Interactor player)
        {
            base.OnPlayerEnter(player);
            Interactor other = LoJamGameManager.players.Find(p => p != player);
            LoJamGameManager.craftingStations.Find(cs => cs.GetSide() == other.GetSide()).Hack(true, _duration);
            RemovePowerup();
        }
    }
}
