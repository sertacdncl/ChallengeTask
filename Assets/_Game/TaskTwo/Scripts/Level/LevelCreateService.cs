using System.Collections.Generic;
using TaskTwo.Data;
using TaskTwo.Level.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TaskTwo.Level
{
	public class LevelCreateService : MonoBehaviour
	{
		[SerializeField] private List<AssetReference> _levels;

		public void SetupLevel()
		{
			var level = GameDataService.CurrentLevel;
			_levels[level].LoadAssetAsync<GameObject>().Completed += handle =>
			{
				LevelEvents.OnLevelAssetLoaded?.Invoke(handle.Result);
			};
		}
	}
}
