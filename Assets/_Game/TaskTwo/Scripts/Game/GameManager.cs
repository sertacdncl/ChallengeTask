using TaskTwo.Level;
using UnityEngine;
using Zenject;

namespace TaskTwo.Game
{
	public class GameManager : MonoBehaviour
	{
		[Inject] private LevelCreateService _levelCreateService;
		
		private void Start()
		{
			_levelCreateService.SetupLevel();
			
		}
	}
}