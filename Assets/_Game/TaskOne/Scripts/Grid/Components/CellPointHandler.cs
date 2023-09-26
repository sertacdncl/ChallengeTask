using TaskOne.Grid.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace TaskOne.Grid.Components
{
	public class CellPointHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[Inject] private CellMarkerPoolService _cellMarkerPoolService;
		
		public void OnPointerDown(PointerEventData eventData)
		{
			
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			
		}
	}
}