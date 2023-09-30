using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Shared.Addressable.Utils;
using TaskTwo.Level.Utils;
using TaskTwo.Stacks.Components;
using TaskTwo.Stacks.Config;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace TaskTwo.Stacks.Utils
{
	[UsedImplicitly]
	public class StackPoolService
	{
		private AddressableLoader _addressableLoader;
		private StackSettings _stackSettings;

		public Transform poolParentTransform;
		private const int InitialPoolSize = 10;
		private bool _initialized;
		private bool _isPoolReady;
		private Stack<StackController> _objectPool = new();
		private Vector3 _originalScale = Vector3.zero;

		[Inject]
		private StackPoolService(AddressableLoader addressableLoader
			, StackSettingsData stackSettingsData
			, StacksManager stacksManager)
		{
			_addressableLoader = addressableLoader;
			_stackSettings = stackSettingsData.StackSettings;
		}


		public async void SetupPool(Transform poolParent = null)
		{
			if (!_initialized)
			{
				poolParentTransform = poolParent;
				var stackLoadTask = _addressableLoader.LoadAsset(_stackSettings.stackAssetRef);
				await stackLoadTask;
				if (stackLoadTask.IsCompletedSuccessfully)
				{
					CreatePool();
					_initialized = true;
				}
			}
			else
				CreatePool();
		}

		private void CreatePool()
		{
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Addressables.InstantiateAsync(_stackSettings.stackAssetRef, poolParentTransform).Completed +=
					OnObjectInstantiated;
			}
		}

		private void OnObjectInstantiated(AsyncOperationHandle<GameObject> objHandle)
		{
			if (objHandle.Status == AsyncOperationStatus.Succeeded)
			{
				GameObject obj = objHandle.Result;
				if(_originalScale == Vector3.zero)
					_originalScale = obj.transform.localScale;
				obj.SetActive(false);
				if (obj.TryGetComponent<StackController>(out var stackController))
					_objectPool.Push(stackController);

				if (_objectPool.Count >= 3 && !_isPoolReady)
				{
					_isPoolReady = true;
					LevelEvents.OnStackPoolReady?.Invoke();
				}
			}
		}

		public StackController GetStackFromPool()
		{
			if (_objectPool.Count <= 3)
				SetupPool();

			StackController stackController = _objectPool.Pop();

			return stackController;
		}

		public void ReturnCellMarkerToPool(StackController stackController)
		{
			stackController.gameObject.SetActive(false);
			stackController.moveTween?.Kill();
			stackController.transform.localScale = _originalScale;
			stackController.collider.enabled = true;
			stackController.transform.SetParent(poolParentTransform);
			_objectPool.Push(stackController);
		}
	}
}