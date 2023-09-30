using System.Collections.Generic;
using DG.Tweening;
using Shared.Extensions;
using TaskTwo.Audio;
using TaskTwo.Game;
using TaskTwo.Level;
using TaskTwo.Level.Utils;
using TaskTwo.Stacks.Components;
using TaskTwo.Stacks.Config;
using TaskTwo.Stacks.Utils;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace TaskTwo.Stacks
{
	public class StacksManager : MonoBehaviour
	{
		public Transform stacksPoolParent;
		public Transform lastStackTransform;
		public List<StackController> activeStacksList = new();

		[Inject] private LevelManager _levelManager;
		[Inject] public StackPoolService _stackPool;
		[Inject] private StackSettingsData _stackSettingsData;
		[Inject] private AudioManager _audioManager;
		private StackSettings Settings => _stackSettingsData.StackSettings;
		private StackController CurrentStack => activeStacksList[^1];
		private int _comboCounter = 0;
		private Tween _stackMoveTween;
		private const float StackStartOffset = 10f;

		private void OnEnable()
		{
			LevelEvents.OnLevelCreated += Setup;
			GameEvents.OnGameStarted += OnGameStart;
			GameEvents.OnTouchToCut += CutStack;
			GameEvents.OnTryAgain += OnTryAgain;
		}

		private void OnDisable()
		{
			LevelEvents.OnLevelCreated -= Setup;
			GameEvents.OnGameStarted -= OnGameStart;
			GameEvents.OnTouchToCut -= CutStack;
			GameEvents.OnTryAgain -= OnTryAgain;
		}

		private void Start() => _stackPool.SetupPool(stacksPoolParent);

		private void Setup() => CreateNextStack(true);

		private void OnGameStart() => CreateNextStack();

		private void OnTryAgain()
		{
			CleanOldLevel();
			CreateNextStack(true);
		}

		private void CleanOldLevel()
		{
			foreach (var stackController in activeStacksList)
			{
				_stackPool.ReturnCellMarkerToPool(stackController);
			}
		}

		private void CreateNextStack(bool isStart = false)
		{
			var stackController = CreateStackObject();

			if (isStart)
			{
				lastStackTransform = stackController.transform;
				SetFirstStackPosition(stackController);
			}
			else
			{
				SetStackScaleFromLastStack(stackController);
				SetSpawnPositionFromLastStack(stackController);
				MoveStack(stackController);
			}

			stackController.gameObject.SetActive(true);
		}

		private StackController CreateStackObject()
		{
			StackController stackController = _stackPool.GetStackFromPool();
			stackController.SetParent(_levelManager.currentLevelController.stacksParent);
			stackController.SetMeshMaterial(Settings.stackMats[activeStacksList.Count]);
			activeStacksList.Add(stackController);
			return stackController;
		}

		private void SetFirstStackPosition(StackController stackController)
		{
			var stackTransform = stackController.transform;
			var spawnPos = _levelManager.currentLevelController.firstStackSpawnPos.position;
			var randomOffset = Random.Range(0, 1) == 1 ? StackStartOffset : -StackStartOffset;
			stackTransform.position = spawnPos.With(x: spawnPos.x + randomOffset, z: spawnPos.z);
			stackTransform.DOMove(spawnPos, 0.5f).SetEase(Ease.OutBack);
		}

		private void SetSpawnPositionFromLastStack(StackController stackController)
		{
			var lastStackBasePos = lastStackTransform.position;
			var stackTransform = stackController.transform;

			Vector3 spawnPos = new()
			{
				z = lastStackBasePos.z + lastStackTransform.localScale.z,
				y = lastStackBasePos.y
			};
			var firstStackSpawnPos = _levelManager.currentLevelController.firstStackSpawnPos;
			spawnPos.x = firstStackSpawnPos.position.x +
						(activeStacksList.Count % 2 == 1 ? 1.5f : -1.5f) * firstStackSpawnPos.localScale.x;
			stackTransform.position = spawnPos;
		}

		private void SetStackScaleFromLastStack(StackController stackController)
		{
			stackController.transform.localScale = lastStackTransform.localScale;
		}

		private void MoveStack(StackController stackController)
		{
			var firstStackSpawnPos = _levelManager.currentLevelController.firstStackSpawnPos;
			var startPos = firstStackSpawnPos.position;
			var targetPosX = activeStacksList.Count % 2 == 0
				? (int)(startPos.x + (firstStackSpawnPos.transform.localScale.x * 1.5f))
				: (int)(startPos.x - (firstStackSpawnPos.transform.localScale.x * 1.5f));

			stackController.StartMove(targetPosX, StackStartOffset, Settings);
		}

		private void CutStack()
		{
			var tempCurrentStack = CurrentStack;
			var cutStack = CutAndPlaceStack();


			bool failThreshold = cutStack.transform.localScale.x > .05f;
			if (failThreshold)
			{
				lastStackTransform = cutStack.transform;
				if (activeStacksList.Count < _levelManager.currentLevelController.levelLength)
					CreateNextStack();
				else
					LevelEvents.OnLevelFinished?.Invoke();


				CreateStackPlaceEffect();
			}
			else
			{
				LevelEvents.OnLevelFailed?.Invoke();
				FallCutStack(cutStack, tempCurrentStack);
			}
		}

		private void FallCutStack(StackController cutStack, StackController tempCurrentStack)
		{
			cutStack.transform.position = tempCurrentStack.transform.position;
			cutStack.transform.localScale = lastStackTransform.transform.localScale;
			cutStack.ToggleRigidbodyKinematic(false);
			_stackPool.ReturnCellMarkerToPool(tempCurrentStack);
		}

		private StackController CutAndPlaceStack()
		{
			var cutStack = _stackPool.GetStackFromPool();
			var tempCurrentStack = CurrentStack;
			var currentStackTransform = tempCurrentStack.transform;
			var currentStackLocalScale = currentStackTransform.localScale;
			var cutStackTransform = cutStack.transform;

			cutStack.SetParent(_levelManager.currentLevelController.stacksParent);
			cutStack.SetMeshMaterial(tempCurrentStack.meshRenderer.material);


			Vector3 lastStackPos = lastStackTransform.position;
			var currentStackPos = currentStackTransform.position;
			Vector3 currentStackScale = currentStackLocalScale;


			//Cut Stack
			cutStackTransform.position = new Vector3(
				(lastStackPos.x + currentStackPos.x) / 2f
				, currentStackPos.y
				, currentStackPos.z);

			cutStackTransform.localScale = new Vector3(
				currentStackLocalScale.x - Mathf.Abs(lastStackPos.x - currentStackPos.x)
				, currentStackLocalScale.y
				, currentStackLocalScale.z);
			cutStack.gameObject.SetActive(true);

			PlayComboSound(cutStack, tempCurrentStack);

			//Current Stack
			float factor = currentStackPos.x - lastStackPos.x > 0 ? 1 : -1;
			currentStackTransform.position = new Vector3(
				(lastStackPos.x + currentStackPos.x) / 2f + currentStackScale.x * factor / 2f
				, currentStackPos.y
				, currentStackPos.z);

			currentStackTransform.localScale =
				new Vector3(
					lastStackTransform.localScale.x - cutStackTransform.localScale.x
					, lastStackTransform.localScale.y
					, lastStackTransform.localScale.z);

			activeStacksList.Remove(CurrentStack);
			activeStacksList.Add(cutStack);
			tempCurrentStack.ToggleRigidbodyKinematic(false);
			tempCurrentStack.moveTween?.Kill();
			cutStack.gameObject.SetActive(true);

			return cutStack;
		}

		private void PlayComboSound(StackController cutStack, StackController tempCurrentStack)
		{
			var isGoodCut = cutStack.transform.localScale.x / tempCurrentStack.transform.localScale.x > .9f;
			if (isGoodCut)
			{
				_comboCounter++;
				_audioManager.PlayNote(0.5f + (_comboCounter * .1f));
			}
			else
			{
				_comboCounter = 1;
				_audioManager.PlayNote(0.5f);
			}
		}

		private void CreateStackPlaceEffect()
		{
			var fakeStack = _stackPool.GetStackFromPool();
			fakeStack.collider.enabled = false;
			fakeStack.ToggleRigidbodyKinematic(true);
			fakeStack.transform.localScale = lastStackTransform.localScale;
			fakeStack.transform.position = lastStackTransform.position;
			fakeStack.gameObject.SetActive(true);
			fakeStack.transform.DOScale(fakeStack.transform.localScale * 1.15f, .3f).SetLoops(2, LoopType.Yoyo).OnComplete(
				() =>
				{
					fakeStack.ToggleRigidbodyKinematic(true);
					fakeStack.meshRenderer.material.color.With(a: 1);
					_stackPool.ReturnCellMarkerToPool(fakeStack);
				}
			);
			fakeStack.meshRenderer.material.color = _comboCounter > 1 ? Color.green : Color.white;
			fakeStack.meshRenderer.material.DOFade(0, .5f);
		}
	}
}