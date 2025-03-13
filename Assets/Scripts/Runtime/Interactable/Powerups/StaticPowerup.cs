using LoJam.Core;
using LoJam.Logic;
using LoJam.MonoSystem;
using LoJam.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace LoJam.Interactable
{
    public class StaticPowerup : PowerupBase
    {
        public override void OnPlayerAdjancent(Interactor player) { }
        public override void OnPlayerAdjancentExit(Interactor player) { }

        public override void OnPlayerExit(Interactor player) { }

        public override void OnPlayerEnter(Interactor player)
        {
            Interactor other = LoJamGameManager.players.Find(p => p != player);

            SpawnStatic(other.GetSide());

            RemovePowerup();
        }

        private void SpawnStatic(Side side)
        {
            Debug.Log("SpawningStatic");
            StaticController frame = Instantiate(
                Resources.Load<StaticController>("Static"),
                Vector3.zero,
                Quaternion.identity
            );

            frame.SetSide(side);
        }
    }
}
