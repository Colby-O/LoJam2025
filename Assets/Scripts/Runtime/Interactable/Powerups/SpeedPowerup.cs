using LoJam.Grid;
using LoJam.Player;
using UnityEngine;

namespace LoJam.Interactable
{
    public class SpeedPowerup : PowerupBase
    {
        [SerializeField] private float _length;
        [SerializeField, Min(1)] private float _amount;

        public override void OnPlayerAdjancent(Interactor player) { }
        public override void OnPlayerAdjancentExit(Interactor player) { }

        public override void OnPlayerExit(Interactor player) { }

        public override void OnPlayerEnter(Interactor player)
        {
            base.OnPlayerEnter(player);
            player.GetComponent<PlayerController>().AddSpeedEffector(_amount, _length);
            RemovePowerup();
        }
    }
}
