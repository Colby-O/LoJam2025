using LoJam.Core;
using LoJam.MonoSystem;
using UnityEngine;

namespace LoJam.Logic
{
    public class StaticController : MonoBehaviour
    {
        [SerializeField] private float _lifeSpan = 15;

        private Side _side;

        private float _life;

        public void SetSide(Side side) => _side = side;

        private Vector3 GetCenter(Side side)
        {
            Vector2 bounds = GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds();
            Vector2 firewallPos = GameManager.GetMonoSystem<IGridMonoSystem>().GetFirewallPos();

            float offset = ((bounds.x / 2f) - firewallPos.x) * GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().x;

            return (side == Side.Left) ? new Vector3(firewallPos.x - (bounds.x / 4f) + offset, bounds.y / 2f, 0f) : new Vector3((bounds.x / 4f) + firewallPos.x + offset, bounds.y / 2f, 0f);
        }

        private Vector3 GetScale(Side side)
        {
            Vector2 bounds = GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds();
            Vector2 firewallPos = GameManager.GetMonoSystem<IGridMonoSystem>().GetFirewallPos();
            return (side == Side.Left) ? 
                new Vector3(firewallPos.x + GameManager.GetMonoSystem<IGridMonoSystem>().GetTileSize().x, 2f * bounds.y, 1f) : 
                new Vector3(bounds.x - firewallPos.x, 2f * bounds.y, 1f);
        }

        private void Update()
        {
            _life += Time.deltaTime;

            if (_life > _lifeSpan) Destroy(gameObject);
            else
            {
                transform.position = GetCenter(_side);
                transform.localScale = GetScale(_side);
            }
        }
    }
}
