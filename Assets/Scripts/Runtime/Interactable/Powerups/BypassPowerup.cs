using LoJam.Interactable;
using LoJam.Logic;
using LoJam.Player;
using UnityEngine;

namespace LoJam
{
    public class BypassPowerup : PowerupBase
    {
        [SerializeField] private float _duration;

        public override void OnPlayerAdjancent(Interactor player) { }
        public override void OnPlayerAdjancentExit(Interactor player) { }

        public override void OnPlayerExit(Interactor player) { }

        public override void OnPlayerEnter(Interactor player)
        {
            FindFirstObjectByType<FirewallController>().Open(player, _duration);
            RemovePowerup();
        }
    }
}
