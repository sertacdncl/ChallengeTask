using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TaskOne.Addressable.Utils
{
	[UsedImplicitly]
	public class AddressableLoader
	{
		public async Task<GameObject> LoadAsset(AssetReference assetReference)
		{
			var loadOperation = assetReference.LoadAssetAsync<GameObject>();
			await loadOperation.Task; 

			if (loadOperation.Status == AsyncOperationStatus.Succeeded)
			{
				return loadOperation.Result;
			}

			Debug.LogError("Asset Reference Load Failed");
			return null;
		}
		
		
	}
}