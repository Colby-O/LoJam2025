using UnityEngine;

namespace LoJam
{
    public class UIBounce : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _amplitude = 0.1f;
        [SerializeField] private float _speed = 1.5f;

        private Vector3 _initialPosition;

        private void Start()
        {
            _initialPosition = transform.position;
        }

        private void Update()
        {
            Vector3 localPosition = _initialPosition;
            localPosition.y += _amplitude * Mathf.Cos(Time.time * _speed);
            transform.position = localPosition;
        }
    }
}
