using System.Collections.Generic;
using JetBrains.Annotations;
using Shared.Addressable.Utils;
using TaskTwo.Stacks.Components;
using TaskTwo.Stacks.Config;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace TaskTwo.Stacks.Utils
{
	[UsedImplicitly]
	public class StackPoolService : IInitializable
	{
		private AddressableLoader _addressableLoader;
		private StacksManager _stacksManager;
		private StackSettings _stackSettings;

		private const int InitialPoolSize = 10;
		private bool _initialized;
		private Stack<StackController> _objectPool = new();


		private StackPoolService(AddressableLoader addressableLoader
			, StackSettingsData stackSettingsData
			, StacksManager stacksManager)
		{
			_addressableLoader = addressableLoader;
			_stackSettings = stackSettingsData.StackSettings;
			_stacksManager = stacksManager;
		}

		public void Initialize()
		{
			SetupPool();
		}

		private async void SetupPool()
		{
			if (!_initialized)
			{
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
				Addressables.InstantiateAsync(_stackSettings.stackAssetRef, _stacksManager.stacksPoolParent).Completed +=
					OnObjectInstantiated;
			}
		}

		private void OnObjectInstantiated(AsyncOperationHandle<GameObject> objHandle)
		{
			if (objHandle.Status == AsyncOperationStatus.Succeeded)
			{
				GameObject obj = objHandle.Result;
				obj.SetActive(false);
				if (obj.TryGetComponent<StackController>(out var stackController))
					_objectPool.Push(stackController);
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
			stackController.transform.SetParent(_stacksManager.stacksPoolParent);
			_objectPool.Push(stackController);
		}
	}
}