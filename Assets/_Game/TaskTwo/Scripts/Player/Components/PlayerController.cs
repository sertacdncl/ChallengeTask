using System;
using DG.Tweening;
using Shared.Extensions;
using TaskTwo.Camera;
using TaskTwo.Game;
using TaskTwo.Level;
using TaskTwo.Level.Utils;
using TaskTwo.Stacks;
using UnityEngine;
using Zenject;

namespace TaskTwo.Player.Components
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Animator _playerAnimator;
		[SerializeField] private float _playerSpeed = 5f;
		[Inject] private LevelManager _levelManager;
		[Inject] private StacksManager _stacksManager;
		[Inject] private CameraManager _cameraManager;

		public enum PlayerState
		{
			Idle,
			Run,
			Fall,
			Dance
		}

		public PlayerState State
		{
			get => _playerState;
			set
			{
				_playerState = value;
				switch (_playerState)
				{
					case PlayerState.Idle:
						_playerAnimator.SetBool(Run, false);
						_playerAnimator.SetBool(Dance, false);
						_playerAnimator.SetBool(Fall, false);

						break;
					case PlayerState.Run:
						_playerAnimator.SetBool(Dance, false);
						_playerAnimator.SetBool(Fall, false);
						_playerAnimator.SetBool(Run, true);
						break;
					case PlayerState.Fall:
						_playerAnimator.SetBool(Dance, false);
						_playerAnimator.SetBool(Run, false);
						_playerAnimator.SetBool(Fall, true);
						break;
					case PlayerState.Dance:
						_playerAnimator.SetBool(Run, false);
						_playerAnimator.SetBool(Fall, false);
						_playerAnimator.SetBool(Dance, true);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private PlayerState _playerState;
		private static readonly int Run = Animator.StringToHash("Run");
		private static readonly int Dance = Animator.StringToHash("Dance");
		private static readonly int Fall = Animator.StringToHash("Fall");

		private const float TimeMultiplier = 3;
		private Rigidbody _playerRigidbody;

		private bool _initialized;
		private bool _gameStarted;
		


		private void OnEnable()
		{
			LevelEvents.OnLevelCreated += OnLevelCreated;
			GameEvents.OnGameStarted += OnGameStart;
			GameEvents.OnTryAgain += OnTryAgain;
		}

		private void OnDisable()
		{
			LevelEvents.OnLevelCreated -= OnLevelCreated;
			GameEvents.OnGameStarted -= OnGameStart;
			GameEvents.OnTryAgain -= OnTryAgain;
		}

		private void Start()
		{
			_playerRigidbody = GetComponent<Rigidbody>();
		}

		private void OnLevelCreated()
		{
			transform.position = _levelManager.currentLevelController.startArea.position.With();
			_playerRigidbody.isKinematic = false;
			_initialized = true;
		}

		private void OnTryAgain()
		{
			State = PlayerState.Idle;
			_playerRigidbody.isKinematic = true;
			_playerRigidbody.velocity = Vector3.zero;
			transform.position = _levelManager.currentLevelController.startArea.position.With();
			_cameraManager.SetFollowTarget(transform);
			_playerRigidbody.isKinematic = false;
		}

		private void FixedUpdate()
		{
			if (!_initialized || !_gameStarted)
				return;
			
			if (State != PlayerState.Run) return;

			var myTransform = transform;
			Vector3 drawPosition = myTransform.position + Vector3.up + (Vector3.forward * .1f);
			var ray = new Ray(drawPosition, -myTransform.up);
			if (Physics.Raycast(ray, out var hit, 10))
			{
				MoveForward();
				
				bool isFinish = hit.collider.CompareTag("Finish");
				if (isFinish)
				{
					LevelEvents.OnLevelFinished?.Invoke();
					OnGameFinish();
				}
			}
			else
			{
				_playerSpeed = 0;
				State = PlayerState.Fall;
				LevelEvents.OnLevelFailed?.Invoke();
				DOVirtual.DelayedCall(2.2f, () =>
				{
					_playerRigidbody.isKinematic = true;
				});

				foreach (var stackController in _stacksManager.activeStacksList)
				{
					stackController.collider.isTrigger = true;
				}
			}
			
		}

		private void MoveForward()
		{
			_playerRigidbody.velocity = (Vector3.forward * _playerSpeed);
			var playerPos = _playerRigidbody.transform.position;
			playerPos = Vector3.Lerp(playerPos,
				new Vector3(_stacksManager.lastStackTransform.transform.position.x, playerPos.y,
					playerPos.z),
				TimeMultiplier * Time.deltaTime);
			_playerRigidbody.transform.position = playerPos;
		}

		private void OnGameStart()
		{
			State = PlayerState.Run;
			_gameStarted = true;
		}
		
		private void OnGameFinish()
		{
			State = PlayerState.Dance;
			_gameStarted = false;
		}
	}
}