using UnityEngine;

namespace TaskTwo.Data
{
	public static class GameDataService
	{
		private const string CurrentLevelKey = "CurrentLevel";

		public static int CurrentLevel
		{
			get
			{
				if(!PlayerPrefs.HasKey(CurrentLevelKey))
					PlayerPrefs.SetInt(CurrentLevelKey, 0);
				return PlayerPrefs.GetInt(CurrentLevelKey);
			}
			set => PlayerPrefs.SetInt(CurrentLevelKey, value);
		}
	}
}