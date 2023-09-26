using System.Collections.Generic;
using JetBrains.Annotations;
using TaskOne.Addressable.Utils;
using TaskOne.Grid.Components;
using TaskOne.Grid.Config;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace TaskOne.Grid.Utils
{
	[UsedImplicitly]
	public class CellMarkerPoolService : IInitializable
	{
		private GridManager.Settings _gridSettings;
		private GridManager _gridManager;
		private AddressableLoader _addressableLoader;

		private bool _initialized;
		private int _initialPoolSize = 10;
		private Stack<CellMarkerController> _objectPool = new();

		private CellMarkerPoolService(GridSettingsData gridSettingsData, GridManager gridManager,
			AddressableLoader addressableLoader)
		{
			_gridSettings = gridSettingsData.GridSettings;
			_gridManager = gridManager;
			_addressableLoader = addressableLoader;
		}

		public void Initialize()
		{
			SetupPool();
		}


		private async void SetupPool()
		{
			if (!_initialized)
			{
				var xObjectLoad = _addressableLoader.LoadAsset(_gridSettings.xObjectPrefab);
				await xObjectLoad;
				if (xObjectLoad.IsCompletedSuccessfully)
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
			for (int i = 0; i < _initialPoolSize; i++)
			{
				Addressables.InstantiateAsync(_gridSettings.xObjectPrefab, _gridManager.cellMarkerPoolParent).Completed +=
					OnObjectInstantiated;
			}
		}

		private void OnObjectInstantiated(AsyncOperationHandle<GameObject> objHandle)
		{
			if (objHandle.Status == AsyncOperationStatus.Succeeded)
			{
				GameObject obj = objHandle.Result;
				obj.SetActive(false);
				if (obj.TryGetComponent<CellMarkerController>(out var cellMarker))
					_objectPool.Push(cellMarker);
			}
		}

		public CellMarkerController GetCellMarkerFromPool()
		{
			if (_objectPool.Count <= 3)
				SetupPool();
			
			CellMarkerController marker = _objectPool.Pop();

			return marker;
		}

		public void ReturnCellMarkerToPool(CellMarkerController marker)
		{
			marker.gameObject.SetActive(false);
			_objectPool.Push(marker);
		}
	}
}