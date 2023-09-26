using TaskOne.Grid.Utils;
using UnityEngine;
using Zenject;

namespace TaskOne.Game
{
	public class GameManager : MonoBehaviour
	{
		private GridSetupService _gridSetupService;
		
		[Inject]
		private void Construct(GridSetupService gridSetupService)
		{
			_gridSetupService = gridSetupService;
		}
		
		private void Start()
		{
			_gridSetupService.SetupGrid();
		}
	}
}