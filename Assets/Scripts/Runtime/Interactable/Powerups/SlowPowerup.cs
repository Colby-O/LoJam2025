using LoJam.Interactable;
using LoJam.Player;
using UnityEngine;

namespace LoJam.Interactable
{
    public class SlowPowerup : PowerupBase
    {
        [SerializeField] private float _length;
        [SerializeField, Range(0, 1)] private float _amount;

        public override void OnPlayerAdjancent(Interactor player) { }
        public override void OnPlayerAdjancentExit(Interactor player) { }

        public override void OnPlayerExit(Interactor player) { }

        public override void OnPlayerEnter(Interactor player)
        {
            LoJamGameManager.players.Find(p => p != player).GetComponent<PlayerController>().AddSpeedEffector(_amount, _length);
            RemovePowerup();
        }
    }
}
