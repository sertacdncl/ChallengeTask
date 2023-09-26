using TaskOne.Grid.Utils;
using UnityEngine;

namespace TaskOne.Grid.Components
{
	public class CellController : MonoBehaviour
	{
		public CellNeighbours Neighbours;
		public Vector2Int coordinate;
		
		private GridManager _gridManager;

		public void Setup(Vector3 basePos, Vector2Int coords)
		{
			name = $"Cell [{coords.x},{coords.y}]";
			transform.localPosition = basePos;
			coordinate = coords;
		}
	}
}