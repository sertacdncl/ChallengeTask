using TaskTwo.Level.Utils;
using UnityEngine;
using Zenject;

namespace TaskTwo.Game
{
	public class GameManager : MonoBehaviour
	{
		[Inject] private LevelSetupService _levelSetupService;
		
		private void Start()
		{
			_levelSetupService.SetupLevel();
			
		}
	}
}