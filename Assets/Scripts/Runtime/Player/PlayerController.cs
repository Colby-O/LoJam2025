using LoJam.Logic;
using LoJam.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LoJam
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Interactor _interator;

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
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_interator == null) _interator = GetComponent<Interactor>();

            if (_interator.GetSide() == Side.Left) _input.actions["Move"].performed += Move;
            else _input.actions["Move2"].performed += Move;
        }

        private void Update()
        {
            
        }
    }
}
