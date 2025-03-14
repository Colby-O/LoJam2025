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

		private Vector2 _rawMovement;

		private float _movementMul = 1;

		private float _effectDuration = 0;
		private float _effectTime = 0;

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
		}

		private void Move(InputAction.CallbackContext e)
		{
			InputDevice device = e.control.device;
			if (device != null)
			{
				if (device is Gamepad)
				{
					if (_interator.myId == -1) _interator.RegisterController(device.deviceId);
					if (device.deviceId == _interator.myId)
					{
						_rawMovement = e.ReadValue<Vector2>();
						_rb.linearVelocity = _rawMovement * _movementSpeed * _movementMul;
					}
				}
				else if (device is Keyboard)
				{
					_rawMovement = e.ReadValue<Vector2>();
					_rb.linearVelocity = _rawMovement * _movementSpeed * _movementMul;
				}
			}
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
			ProcessMovement();

			if (_effectDuration != 0)
			{
				_effectTime += Time.deltaTime;
				if (_effectTime > _effectDuration) RemoveSpeedEffector();
			}
		}
	}
}
