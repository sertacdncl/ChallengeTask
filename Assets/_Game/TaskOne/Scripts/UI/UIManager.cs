using System;
using TaskOne.Grid;
using TaskOne.Grid.Config;
using TaskOne.Grid.Utils;
using TMPro;
using UnityEngine;
using Zenject;

namespace TaskOne.UI
{
	public class UIManager : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI matchCountText;
		[SerializeField] private TMP_InputField _inputFieldX;
		[SerializeField] private TMP_InputField _inputFieldY;

		[Inject] private GridSetupService _gridSetupService;
		[Inject] private GridSettingsData _gridSettingsData;
		private GridManager.Settings _gridSettings;
		private int _matchCount;

		private void OnEnable()
		{
			GridEvents.OnCellMatch += OnMatchCountChanged;
		}

		private void OnDisable()
		{
			GridEvents.OnCellMatch -= OnMatchCountChanged;
		}

		private void Start()
		{
			_gridSettings = _gridSettingsData.GridSettings;
			_inputFieldX.text = _gridSettings.width.ToString();
			_inputFieldY.text = _gridSettings.height.ToString();
		}

		public void OnClickBuildGrid()
		{
			var gridWidth = _gridSettings.width;
			var gridHeight = _gridSettings.height;
			
			var newGridWidth = int.TryParse(_inputFieldX.text, out var x) ? x : gridWidth;
			var newGridHeight = int.TryParse(_inputFieldY.text, out var y) ? y : gridHeight;
			
			if(gridWidth == newGridWidth && gridHeight == newGridHeight)
				return;
			_gridSettings.width = newGridWidth;
			_gridSettings.height = newGridHeight;
			_gridSetupService.RebuildGrid();
		}
		
		private void OnMatchCountChanged()
		{
			_matchCount++;
			matchCountText.text = $"Match Count: {_matchCount}";
		}
	}
}