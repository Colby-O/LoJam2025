using UnityEngine;

namespace LoJam.Interactable
{
    public class TestPowerup : BasePowerup
    {
        public override void OnPlayerAdjancent()
        {
            Debug.Log("Near Powerup!");
        }

        public override void OnPlayerEnter()
        {
            Debug.Log("Picking Up Powerup!");
            Tile.SetInteractable(null);
            Destroy(gameObject);
        }
    }
}
