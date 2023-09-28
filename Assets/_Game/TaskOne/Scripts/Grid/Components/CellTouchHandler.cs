using Shared.Finger;
using TaskOne.Grid.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TaskOne.Grid.Components
{
	public class CellTouchHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		private CellMarkerPoolService _cellMarkerPoolService; 
		[SerializeField] private CellController _cellController;
		public void OnPointerDown(PointerEventData eventData)
		{
			if (FingerManager.CanTouch)
			{
				FingerManager.FingerCount++;
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (FingerManager.IsFingerDown)
			{
				_cellController.OnTouch();
				FingerManager.FingerCount--;
			}
		}
	}
}