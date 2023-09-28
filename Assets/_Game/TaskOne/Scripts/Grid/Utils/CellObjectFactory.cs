using TaskOne.Addressable.Utils;
using TaskOne.Grid.Components;
using TaskOne.Grid.Config;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TaskOne.Grid.Utils
{
	public class CellObjectFactory : PlaceholderFactory<CellController>
	{
		private DiContainer _container;
		private GridManager _gridManager;
		private GridManager.Settings _gridSettings;
		private AddressableLoader _addressableLoader;

		private Transform _gridParent;
		private GameObject _loadedCellPrefab;
		private bool _isInitialized;

		[Inject]
		public void Construct(DiContainer container, AddressableLoader addressableLoader, GridManager gridManager,
			GridSettingsData gridSettingsData)
		{
			_container = container;
			_addressableLoader = addressableLoader;
			_gridManager = gridManager;
			_gridSettings = gridSettingsData.GridSettings;
		}

		public async void LoadAndCreateGridElements()
		{
			if (!_isInitialized)
			{
				_gridParent = _gridManager.gridParent;
				var cellLoad = _addressableLoader.LoadAsset(_gridSettings.cellPrefab);
				await cellLoad;
				if (cellLoad.IsCompletedSuccessfully)
				{
					_loadedCellPrefab = cellLoad.Result;
					CreateGrid();
				}
			}
			else
				CreateGrid();
		}

		private void CreateGrid()
		{
			_gridManager.CellControllers = new CellController[_gridSettings.width, _gridSettings.height];
			Vector3 cellPos = Vector3.zero;
			for (int y = 0; y < _gridSettings.height; y++)
			{
				cellPos.y = y * _gridSettings.distance;
				for (int x = 0; x < _gridSettings.width; x++)
				{
					cellPos.x = x * _gridSettings.distance;
					var cellCoordinate = new Vector2Int(x, y);
					CreateCell(cellCoordinate, cellPos);
				}
			}
		}

		public void CreateCell(Vector2Int coordinate, Vector3 pos)
		{
			Addressables.InstantiateAsync(_gridSettings.cellPrefab, _gridParent).Completed += objHandle =>
			{
				var result = objHandle.Result;
				var cellController = result.GetComponent<CellController>();
				if (ReferenceEquals(cellController, null)) return;
				_container.Inject(cellController);
				_gridManager.CellControllers[coordinate.x, coordinate.y] = cellController;
				
				cellController.Setup(pos, coordinate);

				if (_gridManager.CellControllers[_gridSettings.width - 1, _gridSettings.height - 1] != null)
				{
					GridEvents.OnGridSetupComplete?.Invoke();
					_isInitialized = true;
				}
			};
			
			// Another version of instantiate without Addressables
			// var instantiatedGameObject = _container.InstantiatePrefab(_loadedCellPrefab, _gridParent);
			// var cellController = instantiatedGameObject.GetComponent<CellController>();
			// if (ReferenceEquals(cellController, null)) return;
			// _gridManager.CellControllers[coordinate.x, coordinate.y] = cellController;
			//
			// cellController.Setup(pos, coordinate);
			//
			// if (_gridManager.CellControllers[_gridSettings.width - 1, _gridSettings.height - 1] != null)
			// {
			// 	GridEvents.OnGridSetupComplete?.Invoke();
			// 	_isInitialized = true;
			// }
		}
	}
}