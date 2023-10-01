using TaskOne.Grid.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
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

#if UNITY_EDITOR
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
				SceneManager.LoadScene(0);
		}
#endif
	}
}