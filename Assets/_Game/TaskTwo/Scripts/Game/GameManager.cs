using DG.Tweening;
using Shared.Finger;
using TaskTwo.Camera;
using TaskTwo.Level;
using TaskTwo.Level.Utils;
using TaskTwo.Stacks;
using UnityEngine;
using Zenject;

namespace TaskTwo.Game
{
	public class GameManager : MonoBehaviour
	{
		[Inject] private LevelSetupService _levelSetupService;
		[Inject] private LevelManager _levelManager;
		
		[Inject] private StacksManager _stacksManager;
		[Inject] private UIManager _uiManager;
		[Inject] private CameraManager _cameraManager;
		private bool _isGameStarted;

		private void OnEnable()
		{
			LevelEvents.OnStackPoolReady += OnStackPoolReady;
			LevelEvents.OnLevelFailed += OnLevelFailed;
			LevelEvents.OnLevelFinished += OnLevelFinish;
			GameEvents.OnTryAgain += OnTryAgain;
		}

		private void OnDisable()
		{
			LevelEvents.OnStackPoolReady -= OnStackPoolReady;
			LevelEvents.OnLevelFailed -= OnLevelFailed;
			LevelEvents.OnLevelFinished -= OnLevelFinish;
			GameEvents.OnTryAgain -= OnTryAgain;
		}

		private void OnStackPoolReady()
		{
			_levelSetupService.SetupLevel();
		}
		
		private void OnTryAgain()
		{
			
		}


		public void OnClick_StartGame()
		{
			FingerManager.MaxFingerCount = 1;
			_isGameStarted = true;
			GameEvents.OnGameStarted?.Invoke();
		}

		private void Update()
		{
			if (!_isGameStarted)
				return;
			OnFingerDown();
			OnFingerUp();
		}

		private static void OnFingerDown()
		{
			if (FingerManager.CanTouch && Input.GetMouseButtonDown(0))
			{
				FingerManager.FingerCount++;
				GameEvents.OnTouchToCut?.Invoke();
			}
		}

		private static void OnFingerUp()
		{
			if (!FingerManager.CanTouch && Input.GetMouseButtonUp(0))
				FingerManager.FingerCount--;
		}

		private void OnLevelFailed()
		{
			FingerManager.CanUseFinger = false;
			DOVirtual.DelayedCall(2, () =>
			{
				_uiManager.ToggleFailCanvas(true);
				_cameraManager.SetFollowTarget(null);
			});
		}

		private void OnLevelFinish()
		{
			FingerManager.CanUseFinger = false;
		}
	}
}