using LoJam.Logic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LoJam
{
    public class player_movement : MonoBehaviour
    {

        [SerializeField] private PlayerInput _input;

        public Rigidbody2D body;
        float horizontalMovement;
        float verticalMovement;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (_input == null) _input = GetComponent<PlayerInput>();

            _input.actions["Move"].performed += Move;
        }

        // Update is called once per frame
        void Update()
        {
            body.linearVelocityY = verticalMovement;
            body.linearVelocityX = horizontalMovement;
        }

        public void Move(InputAction.CallbackContext context)
        {

            horizontalMovement = context.ReadValue<Vector2>().x;
            verticalMovement = context.ReadValue<Vector2>().y;
        }
    }
}
