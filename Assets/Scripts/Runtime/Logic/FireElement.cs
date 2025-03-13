using System.Collections.Generic;
using UnityEngine;

namespace LoJam.Logic
{
    public class FireElement : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider2D _collider;

        [Header("Idle")]
        [SerializeField] private List<Sprite> _idleFrames;
        [SerializeField] private float _idleAnimSpeed = 0.2f;

        private int _ptr;
        private float _time;

        public Collider2D Collider { get { return _collider; } }

        public SpriteRenderer SpriteRenderer { get { return _spriteRenderer; } }

        private void Awake()
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_collider == null) _collider = _spriteRenderer.GetComponent<Collider2D>();

            _ptr = Random.Range(0, _idleFrames.Count);
            if (_idleFrames != null && _idleFrames.Count > 0 ) _spriteRenderer.sprite = _idleFrames[_ptr];
        }

        private void Update()
        {
            _time += Time.deltaTime;

            if (_time > _idleAnimSpeed)
            {
                _spriteRenderer.sprite = _idleFrames[++_ptr % _idleFrames.Count];
                _time = 0;
            }
        }
    }
}
