using DG.Tweening;
using Shared.Finger;
using TaskTwo.Audio;
using TaskTwo.Camera;
using TaskTwo.Data;
using TaskTwo.Level.Utils;
using TaskTwo.Stacks;
using UnityEngine;
using Zenject;

namespace TaskTwo.Game
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private ConfettiController _confettiController;
		[Inject] private LevelSetupService _levelSetupService;
		[Inject] private UIManager _uiManager;
		[Inject] private StacksManager _stacksManager;
		[Inject] private CameraManager _cameraManager;
		[Inject] private AudioManager _audioManager;
		private bool _isGameStarted;

		private void OnEnable()
		{
			LevelEvents.OnStackPoolReady += OnStackPoolReady;
			LevelEvents.OnLevelReady += OnLevelReady;
			LevelEvents.OnLevelFailed += OnLevelFailed;
			LevelEvents.OnLevelFinished += OnLevelFinish;
			GameEvents.OnTryAgain += OnTryAgain;
		}

		private void OnDisable()
		{
			LevelEvents.OnStackPoolReady -= OnStackPoolReady;
			LevelEvents.OnLevelReady -= OnLevelReady;
			LevelEvents.OnLevelFailed -= OnLevelFailed;
			LevelEvents.OnLevelFinished -= OnLevelFinish;
			GameEvents.OnTryAgain -= OnTryAgain;
		}

		private void Start()
		{
			FingerManager.CanUseFinger = false;
			_stacksManager._stackPool.SetupPool(_stacksManager.stacksPoolParent);
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

		private void OnStackPoolReady()
		{
			_levelSetupService.SetupLevel();
		}

		public void OnClick_StartGame()
		{
			if (!FingerManager.CanTouch)
				return;
			_uiManager.ToggleMainCanvas(false);
			_isGameStarted = true;
			GameEvents.OnGameStarted?.Invoke();
		}

		public void OnClick_NextLevel()
		{
			GameDataService.CurrentLevel++;
			LevelEvents.OnNextLevel?.Invoke();
			_uiManager.ToggleSuccessCanvas(false);
			_uiManager.ToggleMainCanvas(true);
			_levelSetupService.SetupLevel();
		}

		private void OnLevelReady()
		{
			FingerManager.CanUseFinger = true;
		}

		private void OnLevelFailed()
		{
			_isGameStarted = false;
			FingerManager.FingerCount--;
			FingerManager.CanUseFinger = false;
			DOVirtual.DelayedCall(2, () =>
			{
				_uiManager.ToggleFailCanvas(true);
				_cameraManager.SetFollowTarget(null);
			});
		}

		private void OnTryAgain()
		{
			DOVirtual.DelayedCall(1f, () => { FingerManager.CanUseFinger = true; });
		}

		private void OnLevelFinish()
		{
			_isGameStarted = false;
			FingerManager.FingerCount--;
			FingerManager.CanUseFinger = false;
			_audioManager.PlayConfetti();
			_confettiController.PlayParticle();
			DOVirtual.DelayedCall(1f, () => { _uiManager.ToggleSuccessCanvas(true); });
		}
	}
}