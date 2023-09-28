using UnityEngine;

namespace TaskTwo.Data
{
	public static class GameDataService
	{
		private static string _currentLevelKey = "CurrentLevel";
		
		public static int CurrentLevel
		{
			get => PlayerPrefs.GetInt(_currentLevelKey, 0);
			set => PlayerPrefs.SetInt(_currentLevelKey, value);
		}
		
	}
}