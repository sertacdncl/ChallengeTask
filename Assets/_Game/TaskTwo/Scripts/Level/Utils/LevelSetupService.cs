using System.Collections.Generic;
using Shared.Addressable.Utils;
using TaskTwo.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TaskTwo.Level.Utils
{
	public class LevelSetupService : MonoBehaviour
	{
		[SerializeField] private List<AssetReference> _levels;
		[Inject] private AddressableLoader _addressableLoader;
		private int CurrentLevel => GameDataService.CurrentLevel;
		private AssetReference CurrentLevelAsset => _levels[CurrentLevel];

		public async void SetupLevel()
		{
			var load = _addressableLoader.LoadAsset(CurrentLevelAsset);
			await load;
			if (load.Result != null)
			{
				LevelEvents.OnLevelAssetLoaded?.Invoke(CurrentLevelAsset);
			}
		}
	}
}