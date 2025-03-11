using LoJam.Interactable;
using UnityEngine;

namespace LoJam
{
    public class GarbageCollecter : MonoBehaviour
    {
        [SerializeField] private float _lifeSpan;

        private float _life;

        private void HandleCollection()
        {
            Component interactable = gameObject.GetComponent(typeof(IInteractable));
            if (interactable != null)
            {
                (interactable as IInteractable).Tile.SetInteractable(null);
            }

            Destroy(gameObject);
        }

        private void Awake()
        {
            _life = 0;
        }

        private void Update()
        {
            _life += Time.deltaTime;

            if (_life > _lifeSpan) HandleCollection();
        }
    }
}
