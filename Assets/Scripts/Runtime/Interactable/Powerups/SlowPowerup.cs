using LoJam.Interactable;
using LoJam.Player;
using UnityEngine;
using LoJam.Core;
using LoJam.MonoSystem;

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
            base.OnPlayerEnter(player);
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(6);
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(9);
            PlayerController other = LoJamGameManager.players.Find(p => p != player).GetComponent<PlayerController>();
            other.AddSpeedEffector(_amount, _length);
            RemovePowerup();
        }
    }
}
