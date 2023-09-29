using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace TaskTwo.Level.Utils
{
	public static class LevelEvents
	{
		public static UnityAction<AssetReference> OnLevelAssetLoaded;
	}
}