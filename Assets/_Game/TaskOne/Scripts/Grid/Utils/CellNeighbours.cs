using System;
using System.Collections.Generic;
using TaskOne.Grid.Components;

namespace TaskOne.Grid.Utils
{
	public class CellNeighbours
	{
		protected virtual CellController Up{get; set;}
		protected virtual CellController Down{get; set;}
		protected virtual CellController Right{get; set;}
		protected virtual CellController Left{get; set;}
		protected virtual CellController UpRight{get; set;}
		protected virtual CellController UpLeft{get; set;}
		protected virtual CellController DownRight{get; set;}
		protected virtual CellController DownLeft{get; set;}
		
		public enum Direction
		{
			Up,
			Down,
			Right,
			Left,
			UpRight,
			UpLeft,
			DownRight,
			DownLeft
		}

		public void Set(Direction direction, CellController cell)
		{
			var directionMap = new Dictionary<Direction, Action<CellController>>
			{
				{ Direction.Up, c => Up = c },
				{ Direction.Down, c => Down = c },
				{ Direction.Right, c => Right = c },
				{ Direction.Left, c => Left = c },
				{ Direction.UpRight, c => UpRight = c },
				{ Direction.UpLeft, c => UpLeft = c },
				{ Direction.DownRight, c => DownRight = c },
				{ Direction.DownLeft, c => DownLeft = c },
			};

			if (directionMap.TryGetValue(direction, out var setAction))
			{
				setAction(cell);
			}
		}

	}
}