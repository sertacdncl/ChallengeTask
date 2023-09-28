using System;
using JetBrains.Annotations;
using Shared.Addressable.Utils;
using TaskOne.Grid.Components;
using TaskOne.Grid.Config;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TaskOne.Grid.Utils
{
	[UsedImplicitly]
	public class GridSetupService
	{
		private GridManager _gridManager;
		private GridManager.Settings _gridSettings;
		private CellObjectFactory _cellObjectFactory;

		private Transform _gridParent;

		private CellController GetTile(Vector2Int coordinate) =>
			_gridManager.CellControllers[coordinate.x, coordinate.y];

		public CellNeighbours.Direction GetDirection(int xOffset, int yOffset)
		{
			return xOffset switch
			{
				0 when yOffset == -1 => CellNeighbours.Direction.Up,
				0 when yOffset == 1 => CellNeighbours.Direction.Down,
				-1 when yOffset == 0 => CellNeighbours.Direction.Left,
				1 when yOffset == 0 => CellNeighbours.Direction.Right,
				_ => throw new ArgumentException("Invalid xOffset or yOffset")
			};
		}

		private GridSetupService(GridSettingsData gridSettingsData
			, GridManager gridManager
			, AddressableLoader addressableLoader, CellObjectFactory cellObjectFactory)
		{
			_gridSettings = gridSettingsData.GridSettings;
			_gridManager = gridManager;
			_cellObjectFactory = cellObjectFactory;
			GridEvents.OnGridSetupComplete += OnSetupComplete;
		}

		public void RebuildGrid()
		{
			foreach (var cellController in _gridManager.CellControllers)
			{
				Addressables.Release(cellController.gameObject);
				// Object.Destroy(cellController.gameObject);
			}
			SetupGrid();
		}
		
		public void SetupGrid()
		{
			_cellObjectFactory.LoadAndCreateGridElements();
		}

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
				
				Vector2Int[] directions = {
					new(0, 1),   // Up
					new(0, -1),  // Down
					new(-1, 0),  // Left
					new(1, 0)    // Right
				};
				
				foreach (var direction in directions)
				{
					Vector2Int neighborCoordinate = coordinate + direction;

					if (neighborCoordinate.x < 0 || neighborCoordinate.x >= _gridSettings.width ||
						neighborCoordinate.y < 0 || neighborCoordinate.y >= _gridSettings.height)
						continue; // Skip if the neighbor is out of bounds

					CellNeighbours.Direction neighborDirection = GetDirection(direction.x, direction.y);
					cellController.Neighbours.Set(neighborDirection, GetTile(neighborCoordinate));
				}
			}
		}
	}
}