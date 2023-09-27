using TaskOne.Grid.Utils;
using UnityEngine;
using Zenject;

namespace TaskOne.Grid.Components
{
	public class CellController : MonoBehaviour
	{
		[Inject] private CellMarkerPoolService _cellMarkerPoolService;
		private CellMarkerController CellMarkerController { get; set; }
		public CellNeighbours Neighbours;
		public Vector2Int coordinate;


		public bool IsMarked => !ReferenceEquals(CellMarkerController, null);

		public void Setup(Vector3 basePos, Vector2Int coords)
		{
			name = $"Cell [{coords.x},{coords.y}]";
			transform.localPosition = basePos;
			coordinate = coords;
		}

		public void OnTouch()
		{
			if (ReferenceEquals(CellMarkerController, null))
			{
				CellMarkerController = _cellMarkerPoolService.GetCellMarkerFromPool();
				CellMarkerController.Mark(transform);
				CheckMarkMatches();
			}
			else
			{
				UnMark();
			}
		}

		private void UnMark()
		{
			var tempCellMarkerController = CellMarkerController;
			CellMarkerController = null;
			tempCellMarkerController.UnMarkFade(() =>
			{
				_cellMarkerPoolService.ReturnCellMarkerToPool(tempCellMarkerController);
				
			});
		}

		private void CheckMarkMatches()
		{
			var markedNeighbours = Neighbours.GetMarkedNeighbours(this);
			if (IsMarked && markedNeighbours.Count >= 2)
			{
				foreach (var markedCell in markedNeighbours)
				{
					markedCell.UnMark();
				}

				UnMark();
				Debug.Log("Match found!");
			}
		}
	}
}