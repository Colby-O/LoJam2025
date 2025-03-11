using LoJam.Grid;
using LoJam.Interactable;
using UnityEngine;

namespace LoJam
{
    public class GarbageCollecter : MonoBehaviour
    {
        [SerializeField] private float _lifeSpan;

        private float _life;

        public bool Pause 
        { 
            get 
            {
                return _isPaused;
            } 
            set 
            {
                _life = 0;
                _isPaused = value;
            } 
        }

        private bool _isPaused;

        private void HandleCollection()
        {
            Component comp = gameObject.GetComponent(typeof(IInteractable));
            if (comp != null)
            {
                IInteractable interactable = comp as IInteractable;
                foreach (Tile tile in interactable.Tiles)
                {
                    tile.SetInteractable(null);
                }

                interactable.Tiles.Clear();
            }

            Destroy(gameObject);
        }

        private void Awake()
        {
            _life = 0;
            _lifeSpan += Random.Range(-10, 11);
            Pause = false;
        }

        private void Update()
        {
            if (Pause) return;
            
            _life += Time.deltaTime;

            if (_life > _lifeSpan) HandleCollection();
        }
    }
}