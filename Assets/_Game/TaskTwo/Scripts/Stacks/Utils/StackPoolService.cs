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
		private bool _isPoolInitialized;
		private Transform _poolParentTransform;
		private const int InitialPoolSize = 10;
		
		private Stack<StackController> _objectPool = new();
		private Vector3 _originalScale = Vector3.zero;
		
		private AddressableLoader _addressableLoader;
		private StackSettings _stackSettings;

		[Inject]
		private StackPoolService(AddressableLoader addressableLoader
			, StackSettingsData stackSettingsData)
		{
			_addressableLoader = addressableLoader;
			_stackSettings = stackSettingsData.StackSettings;
		}


		public async void SetupPool(Transform poolParent = null)
		{
			if (!_isPoolInitialized)
			{
				_poolParentTransform = poolParent;
				var stackLoadTask = _addressableLoader.LoadAsset(_stackSettings.stackAssetRef);
				await stackLoadTask;

				if (stackLoadTask.IsCompletedSuccessfully)
				{
					CreatePool();
				}
			}
			else
				CreatePool();
		}

		private void CreatePool()
		{
			for (int i = 0; i < InitialPoolSize; i++)
			{
				Addressables.InstantiateAsync(_stackSettings.stackAssetRef, _poolParentTransform).Completed += OnObjectInstantiated;
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

				if (_objectPool.Count >= InitialPoolSize && !_isPoolInitialized)
				{
					_isPoolInitialized = true;
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

		public void ReturnStackToPool(StackController stackController)
		{
			stackController.gameObject.SetActive(false);
			stackController.moveTween?.Kill();
			stackController.transform.SetParent(_poolParentTransform);
			stackController.transform.localScale = _originalScale;
			stackController.transform.localRotation = Quaternion.identity;
			stackController.ToggleRigidbodyKinematic(true);
			stackController.rigidbody.mass = 1;
			stackController.collider.isTrigger = false;
			
			if(_objectPool.Contains(stackController))
				Debug.Log("Found same stack in pool");
			_objectPool.Push(stackController);
		}
	}
}