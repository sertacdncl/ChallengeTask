﻿using System.Collections.Generic;
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
		[Inject] public StackPoolService _stackPool;
		[Inject] private LevelManager _levelManager;
		[Inject] private StackSettingsData _stackSettingsData;
		[Inject] private AudioManager _audioManager;
		private StackSettings Settings => _stackSettingsData.StackSettings;
		private StackController CurrentStack => activeStacksList[^1];

		public Transform stacksPoolParent;
		public Transform lastPlacedStackTransform { get; private set; }
		public List<StackController> activeStacksList = new();

		private Tween _stackMoveTween;
		private int _comboCounter = 0;
		private const float StackStartOffset = 10f;
		private float GetRandomStackOffset => Random.Range(0, 1) == 1 ? -StackStartOffset : StackStartOffset;
		private Transform _firstSpawnTransform;

		//TODO: this part needs to be more clean and readable i guess make it better

		private void OnEnable()
		{
			LevelEvents.OnLevelReady += Setup;
			LevelEvents.OnNextLevel += OnNextLevel;
			GameEvents.OnGameStarted += OnGameStart;
			GameEvents.OnTouchToCut += CutStack;
			GameEvents.OnTryAgain += OnTryAgain;
		}

		private void OnDisable()
		{
			LevelEvents.OnLevelReady -= Setup;
			LevelEvents.OnNextLevel -= OnNextLevel;
			GameEvents.OnGameStarted -= OnGameStart;
			GameEvents.OnTouchToCut -= CutStack;
			GameEvents.OnTryAgain -= OnTryAgain;
		}

		private void Setup()
		{
			CreateNextStack(true);
		}

		private void OnGameStart()
		{
			CreateNextStack();
		}

		private void OnNextLevel()
		{
			CleanOldLevel();
		}

		private void OnTryAgain()
		{
			CleanOldLevel();
			CreateNextStack(true);
		}

		private void CleanOldLevel()
		{
			foreach (var stackController in activeStacksList)
			{
				_stackPool.ReturnStackToPool(stackController);
			}

			activeStacksList.Clear();
			_comboCounter = 0;
		}

		#region Stack Creation

		private StackController CreateStackObject(bool setMaterialFromList = true)
		{
			StackController stackController = _stackPool.GetStackFromPool();
			stackController.SetParent(_levelManager.currentLevelController.stacksParent);
			if (setMaterialFromList)
				stackController.SetMeshMaterial(Settings.stackMats[activeStacksList.Count]);
			return stackController;
		}

		private void CreateNextStack(bool isStart = false)
		{
			var stackController = CreateStackObject();
			activeStacksList.Add(stackController);

			if (isStart)
			{
				_firstSpawnTransform = _levelManager.currentLevelController.firstStackSpawnPos;
				lastPlacedStackTransform = stackController.transform;
				SetFirstStackPosition(stackController);
			}
			else
			{
				SetStackScaleFromLastStack(stackController);
				SetStackNextSpawnPosition(stackController);
				StartMoveStackForNext(stackController);
			}

			stackController.gameObject.SetActive(true);
		}

		private void SetFirstStackPosition(StackController stackController)
		{
			var stackTransform = stackController.transform;
			var randomOffset = GetRandomStackOffset;
			var firstSpawnPos = _firstSpawnTransform.position;
			stackTransform.position = firstSpawnPos.With(x: firstSpawnPos.x + randomOffset, z: firstSpawnPos.z);
			stackTransform.DOMove(firstSpawnPos, 0.5f).SetEase(Ease.OutBack);
		}

		private void SetStackScaleFromLastStack(StackController stackController)
		{
			stackController.transform.localScale = lastPlacedStackTransform.localScale;
		}

		private void SetStackNextSpawnPosition(StackController stackController)
		{
			var lastStackPos = lastPlacedStackTransform.position;
			var stackTransform = stackController.transform;

			Vector3 spawnPos = new()
			{
				z = lastStackPos.z + lastPlacedStackTransform.localScale.z,
				y = lastStackPos.y,
				x = _firstSpawnTransform.position.x +
					(activeStacksList.Count % 2 == 1 ? 1.5f : -1.5f) * _firstSpawnTransform.localScale.x
			};

			stackTransform.position = spawnPos;
		}

		private void StartMoveStackForNext(StackController stackController)
		{
			var firstStackPosX = _firstSpawnTransform.position.x;
			var firstStackScaleX = _firstSpawnTransform.localScale.x;
			var targetPosX = activeStacksList.Count % 2 == 0
				? firstStackPosX + firstStackScaleX * 1.5f
				: firstStackPosX - firstStackScaleX * 1.5f;

			stackController.StartMove(targetPosX, StackStartOffset, Settings);
		}

		#endregion

		#region Cut

		private void CutStack()
		{
			StackController tempCurrentStack = CurrentStack;
			StackController cutStack = CreateStackObject(false);

			SetCutStackPosScale(tempCurrentStack, cutStack);
			MakeFallStack(tempCurrentStack);
			activeStacksList.Add(cutStack);
			cutStack.gameObject.SetActive(true);

			CheckCutStackSuccess(cutStack, tempCurrentStack);
		}

		private void SetCutStackPosScale(StackController tempCurrentStack, StackController cutStack)
		{
			Transform tempCurrentStackTransform = tempCurrentStack.transform;
			Transform cutStackTransform = cutStack.transform;

			cutStack.SetParent(_levelManager.currentLevelController.stacksParent);
			cutStack.SetMeshMaterial(tempCurrentStack.meshRenderer.material);

			//Cut Stack
			cutStackTransform.position = new Vector3(
				(lastPlacedStackTransform.position.x + tempCurrentStackTransform.position.x) / 2f
				, tempCurrentStackTransform.position.y
				, tempCurrentStackTransform.position.z);

			cutStackTransform.localScale = new Vector3(
				tempCurrentStackTransform.localScale.x -
				Mathf.Abs(lastPlacedStackTransform.position.x - tempCurrentStackTransform.position.x)
				, tempCurrentStackTransform.localScale.y
				, tempCurrentStackTransform.localScale.z);
			cutStack.gameObject.SetActive(true);

			PlayComboSound(cutStack, tempCurrentStack);

			//Current Stack
			float horizontalDirection =
				tempCurrentStackTransform.position.x - lastPlacedStackTransform.position.x > 0 ? 1 : -1;
			tempCurrentStackTransform.position = new Vector3(
				(lastPlacedStackTransform.position.x + tempCurrentStackTransform.position.x) / 2f +
				tempCurrentStackTransform.localScale.x * horizontalDirection / 2f
				, tempCurrentStackTransform.position.y
				, tempCurrentStackTransform.position.z);

			tempCurrentStackTransform.localScale =
				new Vector3(
					lastPlacedStackTransform.localScale.x - cutStackTransform.localScale.x
					, lastPlacedStackTransform.localScale.y
					, lastPlacedStackTransform.localScale.z);
		}

		private void CheckCutStackSuccess(StackController cutStack, StackController tempCurrentStack)
		{
			Transform tempCurrentStackTransform = tempCurrentStack.transform;
			bool isStackPlaced = cutStack.transform.localScale.x > .05f;
			if (isStackPlaced)
			{
				lastPlacedStackTransform = cutStack.transform;
				if (activeStacksList.Count < _levelManager.currentLevelController.levelLength)
					CreateNextStack();

				CreateStackPlaceEffect();
			}
			else
			{
				LevelEvents.OnLevelFailed?.Invoke();
				cutStack.transform.position = tempCurrentStackTransform.position;
				cutStack.transform.localScale = lastPlacedStackTransform.localScale;
				tempCurrentStack.gameObject.SetActive(false);
				MakeFallStack(cutStack);
			}
		}

		private void MakeFallStack(StackController stackController)
		{
			stackController.moveTween?.Kill();
			stackController.rigidbody.mass = 100f;
			stackController.collider.isTrigger = true;
			stackController.ToggleRigidbodyKinematic(false);
			activeStacksList.Remove(stackController);
			DOVirtual.DelayedCall(1f, () => _stackPool.ReturnStackToPool(stackController));
		}

		private void CreateStackPlaceEffect()
		{
			var fakeStack = CreateStackObject(false);
			var fakeStackTransform = fakeStack.transform;
			fakeStack.collider.isTrigger = true;
			fakeStackTransform.localScale = lastPlacedStackTransform.localScale;
			fakeStackTransform.position = lastPlacedStackTransform.position;
			fakeStack.SetMeshMaterial(fakeStack.meshRenderer.material);

			fakeStack.meshRenderer.material.color = _comboCounter > 1 ? Color.green : Color.white;
			fakeStack.meshRenderer.material.DOFade(0, .5f);
			fakeStack.transform.DOScale(fakeStack.transform.localScale * 1.15f, .3f).SetLoops(2, LoopType.Yoyo).OnComplete(
				() =>
				{
					fakeStack.meshRenderer.material.color.With(a: 1);
					_stackPool.ReturnStackToPool(fakeStack);
				}
			);

			fakeStack.gameObject.SetActive(true);
		}

		#endregion

		private void PlayComboSound(StackController cutStack, StackController tempCurrentStack)
		{
			bool failThreshold = cutStack.transform.localScale.x > .05f;
			var isGoodCut = cutStack.transform.localScale.x / tempCurrentStack.transform.localScale.x > .9f;
			if (isGoodCut)
			{
				_comboCounter++;
				_audioManager.PlayNote(0.5f + (_comboCounter * .1f));
			}
			else
			{
				_comboCounter = 1;
				if (failThreshold)
					_audioManager.PlayNote(0.5f);
				else
					_audioManager.PlayFail();
			}
		}
	}
}