using UnityEngine;

namespace TaskOne.Grid.Config
{
	[CreateAssetMenu(menuName = "Config/Create GridSettingsData", fileName = "GridSettingsData", order = 0)]
	public class GridSettingsData : ScriptableObject
	{
		[SerializeField] private GridManager.Settings _gridSettings;
		public GridManager.Settings GridSettings => _gridSettings;
	}
}