using TaskTwo.Level.Components;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TaskTwo.Level.Utils
{
	public class LevelFactory : PlaceholderFactory<LevelController>
	{
		private DiContainer _container;
		private LevelSetupService _levelSetupService;
		private LevelManager _levelManager;
		
		[Inject]
		public void Construct(DiContainer container
			, LevelSetupService levelSetupService
			, LevelManager levelManager)
		{
			_container = container;
			_levelSetupService = levelSetupService;
			_levelManager = levelManager;
			LevelEvents.OnLevelAssetLoaded += OnLevelAssetLoaded;
			
		}


		public void OnLevelAssetLoaded(AssetReference assetReference)
		{
			Addressables.InstantiateAsync(assetReference).Completed += handle =>
			{
				var level = handle.Result;
				if (level.TryGetComponent<LevelController>(out var levelController))
				{
					_levelManager.currentLevelController = levelController;
					_container.Inject(levelController);
				}
				
			};
		}
	}
}