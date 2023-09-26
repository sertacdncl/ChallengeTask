using TaskOne.Grid.Utils;
using UnityEngine;
using Zenject;

namespace TaskOne.Grid.Components
{
	public class CellController : MonoBehaviour
	{
		public CellNeighbours Neighbours;
		public Vector2Int coordinate;
		public CellMarkerController CellMarkerController { get; set; }
		[Inject] private CellMarkerPoolService _cellMarkerPoolService;

		private void OnEnable()
		{
			GridEvents.OnTouchCellComplete += OnTouch;
		}

		private void OnDisable()
		{
			GridEvents.OnTouchCellComplete -= OnTouch;
		}

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
				var cellMarkerTransform = CellMarkerController.transform;
				cellMarkerTransform.SetParent(transform);
				cellMarkerTransform.localPosition = Vector3.zero;
				CellMarkerController.gameObject.SetActive(true);
			}
		}
	}
}