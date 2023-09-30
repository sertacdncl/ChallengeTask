using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace TaskTwo.Level.Utils
{
	public static class LevelEvents
	{
		public static UnityAction OnStackPoolReady;
		public static UnityAction<AssetReference> OnLevelAssetLoaded;
		public static UnityAction OnLevelCreated;
		public static UnityAction OnLevelFailed;
		public static UnityAction OnLevelFinished;
	}
}