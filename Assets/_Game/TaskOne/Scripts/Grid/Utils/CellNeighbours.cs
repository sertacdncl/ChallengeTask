using System;
using System.Collections.Generic;
using TaskOne.Grid.Components;

namespace TaskOne.Grid.Utils
{
	public class CellNeighbours
	{
		protected virtual CellController Up { get; set; }
		protected virtual CellController Down { get; set; }
		protected virtual CellController Right { get; set; }
		protected virtual CellController Left { get; set; }

		private List<CellController> GetNeighbourList()
		{
			var neighbours = new List<CellController>();
			if (!ReferenceEquals(Up, null))
				neighbours.Add(Up);
			if (!ReferenceEquals(Down, null))
				neighbours.Add(Down);
			if (!ReferenceEquals(Right, null))
				neighbours.Add(Right);
			if (!ReferenceEquals(Left, null))
				neighbours.Add(Left);
			return neighbours;
		}


		public enum Direction
		{
			Up,
			Down,
			Right,
			Left,
		}

		public void Set(Direction direction, CellController cell)
		{
			var directionMap = new Dictionary<Direction, Action<CellController>>
			{
				{ Direction.Up, c => Up = c },
				{ Direction.Down, c => Down = c },
				{ Direction.Right, c => Right = c },
				{ Direction.Left, c => Left = c },
			};

			if (directionMap.TryGetValue(direction, out var setAction))
			{
				setAction(cell);
			}
		}

		public List<CellController> GetMarkedNeighbours(CellController baseCell
			, List<CellController> markedNeighbors = null)
		{
			markedNeighbors ??= new List<CellController>();
			foreach (var neighbor in GetNeighbourList())
			{
				if (markedNeighbors.Contains(neighbor) || ReferenceEquals(neighbor, baseCell) || !neighbor.IsInteractable)
					continue;
				if (neighbor is { IsMarked: true })
				{
					markedNeighbors.Add(neighbor);
					neighbor.Neighbours.GetMarkedNeighbours(baseCell, markedNeighbors);
				}
			}

			return markedNeighbors;
		}
	}
}