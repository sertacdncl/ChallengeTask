using System;
using JetBrains.Annotations;
using TaskOne.Addressable.Utils;
using TaskOne.Grid.Components;
using TaskOne.Grid.Config;
using UnityEngine;

namespace TaskOne.Grid.Utils
{
	[UsedImplicitly]
	public class GridSetupService
	{
		private GridManager _gridManager;
		private GridManager.Settings _gridSettings;
		private CellObjectFactory _cellObjectFactory;
		private AddressableLoader _addressableLoader;

		private Transform _gridParent;

		private CellController GetTile(Vector2Int coordinate) =>
			_gridManager.CellControllers[coordinate.x, coordinate.y];

		private CellNeighbours.Direction GetDirection(int xOffset, int yOffset)
		{
			if (xOffset == -1)
			{
				if (yOffset == -1) return CellNeighbours.Direction.DownLeft;
				if (yOffset == 0) return CellNeighbours.Direction.Left;
				if (yOffset == 1) return CellNeighbours.Direction.UpLeft;
			}
			else if (xOffset == 0)
			{
				if (yOffset == -1) return CellNeighbours.Direction.Down;
				if (yOffset == 1) return CellNeighbours.Direction.Up;
			}
			else if (xOffset == 1)
			{
				if (yOffset == -1) return CellNeighbours.Direction.DownRight;
				if (yOffset == 0) return CellNeighbours.Direction.Right;
				if (yOffset == 1) return CellNeighbours.Direction.UpRight;
			}

			throw new ArgumentException("Invalid xOffset or yOffset.");
		}

		private GridSetupService(GridSettingsData gridSettingsData
			, GridManager gridManager
			, AddressableLoader addressableLoader, CellObjectFactory cellObjectFactory)
		{
			_gridSettings = gridSettingsData.GridSettings;
			_gridManager = gridManager;
			_addressableLoader = addressableLoader;
			_cellObjectFactory = cellObjectFactory;
			GridEvents.OnGridSetupComplete += OnSetupComplete;
		}

		public async void SetupGrid()
		{
			_cellObjectFactory.LoadAndCreateGridElements();
		}

		// public async void SetupGrid()
		// {
		// 	_gridParent = _gridManager.gridParent;
		// 	var cellLoad = _addressableLoader.LoadAsset(_gridSettings.cellPrefab);
		// 	await cellLoad;
		// 	if (cellLoad.IsCompletedSuccessfully)
		// 		CreateGrid();
		// }
		//
		// private void CreateGrid()
		// {
		// 	_gridManager.CellControllers = new CellController[_gridSettings.width, _gridSettings.height];
		// 	Vector3 cellPos = Vector3.zero;
		// 	for (int y = 0; y < _gridSettings.height; y++)
		// 	{
		// 		cellPos.y = y * _gridSettings.distance;
		// 		for (int x = 0; x < _gridSettings.width; x++)
		// 		{
		// 			cellPos.x = x * _gridSettings.distance;
		// 			var cellCoordinate = new Vector2Int(x, y);
		// 			CreateCell(cellCoordinate, cellPos);
		// 		}
		// 	}
		// }
		//
		// private void CreateCell(Vector2Int coordinate, Vector3 pos)
		// {
		// 	Addressables.InstantiateAsync(_gridSettings.cellPrefab, _gridParent).Completed += handle =>
		// 	{
		// 		if (handle.Status == AsyncOperationStatus.Succeeded)
		// 		{
		// 			var cellController = handle.Result.GetComponent<CellController>();
		// 			if (ReferenceEquals(cellController, null)) return;
		// 			_gridManager.CellControllers[coordinate.x, coordinate.y] = cellController;
		//
		// 			cellController.Setup(pos, coordinate);
		// 		}
		//
		// 		if (_gridManager.CellControllers[_gridSettings.width - 1, _gridSettings.height - 1] != null)
		// 			OnGridSetupComplete?.Invoke();
		// 	};
		// }

		private void OnSetupComplete()
		{
			SetTileNeighbours();
		}

		private void SetTileNeighbours()
		{
			foreach (var cellController in _gridManager.CellControllers)
			{
				Vector2Int coordinate = cellController.coordinate;
				cellController.Neighbours = new CellNeighbours();

				for (int xOffset = -1; xOffset <= 1; xOffset++)
				{
					for (int yOffset = -1; yOffset <= 1; yOffset++)
					{
						if (xOffset == 0 && yOffset == 0)
							continue; // Skip the current cell

						Vector2Int neighborCoordinate = coordinate + new Vector2Int(xOffset, yOffset);
						CellNeighbours.Direction direction = GetDirection(xOffset, yOffset);

						if (neighborCoordinate.x < 0 || neighborCoordinate.x >= _gridSettings.width ||
							neighborCoordinate.y < 0 || neighborCoordinate.y >= _gridSettings.height)
							continue; // Skip if the neighbor is out of bounds (e.g. on the edge of the grid

						cellController.Neighbours.Set(direction, GetTile(neighborCoordinate));
					}
				}
			}
		}
	}
}