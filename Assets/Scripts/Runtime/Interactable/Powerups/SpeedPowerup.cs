using LoJam.Grid;
using LoJam.Player;
using UnityEngine;
using LoJam.Core;
using LoJam.MonoSystem;

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
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(7);
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(10);
            player.GetComponent<PlayerController>().AddSpeedEffector(_amount, _length);
            RemovePowerup();
        }
    }
}
