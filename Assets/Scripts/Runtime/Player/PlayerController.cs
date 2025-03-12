using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace LoJam
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Rigidbody2D _rb;

        [SerializeField] private float _movementSpeed;

        private Vector2 _rawMovement;

        private void Move(InputAction.CallbackContext e)
        {
            _rawMovement = e.ReadValue<Vector2>();

            _rb.linearVelocity = _rawMovement * _movementSpeed;
        }

        private void Awake()
        {
            if (_input == null) _input = GetComponent<PlayerInput>();
            if (_rb != null) _rb = GetComponent<Rigidbody2D>();

            _input.actions["Move"].performed += Move;
        }

        private void Update()
        {
            
        }
    }
}
