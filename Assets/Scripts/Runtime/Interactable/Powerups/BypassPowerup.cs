using LoJam.Interactable;
using LoJam.Logic;
using LoJam.Player;
using UnityEngine;
using LoJam.Core;
using LoJam.MonoSystem;

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
            base.OnPlayerEnter(player);
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(4);
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlaySfX(13);
            FindFirstObjectByType<FirewallController>().Open(player, _duration);
            RemovePowerup();
        }
    }
}
