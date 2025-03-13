using LoJam.Grid;
using LoJam.Player;
using UnityEngine;

namespace LoJam.Interactable
{
    public class TestPowerup : PowerupBase
    {
        public override void OnPlayerAdjancent(Interactor player)
        {
            Debug.Log("Near Powerup!");
        }

        public override void OnPlayerAdjancentExit(Interactor player)
        {

        }

        public override void OnPlayerEnter(Interactor player)
        {
            Debug.Log("Picking Up Powerup!");
            RemovePowerup();
        }

        public override void OnPlayerExit(Interactor player)
        {

        }
    }
}
