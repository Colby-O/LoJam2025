using LoJam.Core;
using LoJam.Logic;
using LoJam.MonoSystem;
using LoJam.Player;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LoJam
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private PlayerInput _input;
		[SerializeField] private Rigidbody2D _rb;
		[SerializeField] private Interactor _interator;
		[SerializeField] private SpriteRenderer _sr;

		[SerializeField] private float _movementSpeed;
		[SerializeField] private float _dissolveRate = 0.05f;

        [SerializeField] private Vector2 _rawMovement;

		private float _movementMul = 1;

		private float _effectDuration = 0;
		private float _effectTime = 0;

		private bool _startedReturn;
		private float _returnTimer = 0;

		public void AddSpeedEffector(float boost, float time)
		{
			if (_movementMul != 1) RemoveSpeedEffector();

			_effectDuration = time;
			_movementMul = boost;
			_effectTime = 0;
		}

		public void RemoveSpeedEffector()
		{
			_effectDuration = 0;
			_movementMul = 1;
		}

		public void ReturnPlayer()
		{
			StartCoroutine(ReturnAnimation());
		}

		private IEnumerator ReturnAnimation()
		{
			_startedReturn = true; 
			_returnTimer = 0;

			float prog = 0;
			while (prog < 1.2)
			{
				prog += _dissolveRate;
				_sr.material.SetFloat("_DissolveAmount", prog);
				yield return new WaitForNextFrameUnit();
			}

			if (_interator.GetSide() == Side.Left)
			{
				transform.position = new Vector3(GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x / 4f, GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().y / 2f, 0);
			}
			else
			{
				transform.position = new Vector3(3f * GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x / 4f, GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().y / 2f, 0);
			}

			while (prog > 0)
			{
				prog -= _dissolveRate;
				_sr.material.SetFloat("_DissolveAmount", prog);
				yield return new WaitForNextFrameUnit();
			}

			_sr.material.SetFloat("_DissolveAmount", 0);

			_startedReturn = false;
		}

		private void Move(InputAction.CallbackContext e)
		{
            if (_startedReturn || LoJamGameManager.isPaused) return;

			InputDevice device = e.control.device;
			if (device != null)
			{
				if (device is Gamepad)
				{
					if (_interator.myId == -1) _interator.RegisterController(device.deviceId);
					if (device.deviceId == _interator.myId)
					{
						_rawMovement = e.ReadValue<Vector2>();
						//_rb.linearVelocity = _rawMovement * _movementSpeed * _movementMul;
					}
					else
					{
						// Note: Really we should be instacing the input system. This is Scuffed but due to time is unaviodable
						// This this called when someone is using a controller and the game thinks it's a gamepad since only one 
						// device can be binded at once. Two controller might have some issues might not idk can't test.

                        int x = 0;
                        int y = 0;

						if (_interator.GetSide() == Side.Left)
						{
                            if (Keyboard.current.wKey.IsPressed()) y += 1;
                            if (Keyboard.current.sKey.IsPressed()) y -= 1;
                            if (Keyboard.current.dKey.IsPressed()) x += 1;
                            if (Keyboard.current.aKey.IsPressed()) x -= 1;
                        }
						else
						{
                            if (Keyboard.current.upArrowKey.IsPressed()) y += 1;
                            if (Keyboard.current.downArrowKey.IsPressed()) y -= 1;
                            if (Keyboard.current.rightArrowKey.IsPressed()) x += 1;
                            if (Keyboard.current.leftArrowKey.IsPressed()) x -= 1;
                        }

                        _rawMovement = (new Vector2(x, y)).normalized;
                    }
				}
				else if (device is Keyboard)
				{
                    _rawMovement = e.ReadValue<Vector2>();
					//_rb.linearVelocity = _rawMovement * _movementSpeed * _movementMul;
				}
			}

            _rb.linearVelocity = _rawMovement * _movementSpeed * _movementMul;
        }

		private void ProcessMovement()
		{
			//_rb.linearVelocity = _rawMovement * _movementSpeed * _movementMul;
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
            if (_startedReturn) 
			{
				_returnTimer += Time.deltaTime;
				
				if (_returnTimer > 3f) 
				{

					if (_interator.GetSide() == Side.Left)
					{
						transform.position = new Vector3(GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x / 4f, GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().y / 2f, 0);
					}
					else
					{
						transform.position = new Vector3(3f * GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().x / 4f, GameManager.GetMonoSystem<IGridMonoSystem>().GetBounds().y / 2f, 0);
					}

					_sr.material.SetFloat("_DissolveAmount", 0);

					_startedReturn = false;
					_returnTimer = 0;
				}
			}

			ProcessMovement();

			if (_effectDuration != 0)
			{
				_effectTime += Time.deltaTime;
				if (_effectTime > _effectDuration) RemoveSpeedEffector();
			}
		}
    }
}
