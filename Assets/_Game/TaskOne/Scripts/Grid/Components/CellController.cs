using DG.Tweening;
using TaskOne.Grid.Utils;
using UnityEditor;
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
		public bool IsInteractable { get; private set; } = true;

		public void Setup(Vector3 basePos, Vector2Int coords)
		{
			name = $"Cell [{coords.x},{coords.y}]";
			transform.localPosition = basePos;
			coordinate = coords;
		}

		public void OnTouch()
		{
			if (!IsInteractable)
				return;
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
			IsInteractable = false;
			DOVirtual.DelayedCall(0.2f, () =>
			{
				tempCellMarkerController.UnMarkFade(() =>
				{
					_cellMarkerPoolService.ReturnCellMarkerToPool(tempCellMarkerController);
					IsInteractable = true;
				});
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
				GridEvents.OnCellMatch?.Invoke();
			}
		}
	}
}