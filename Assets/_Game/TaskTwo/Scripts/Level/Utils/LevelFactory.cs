using TaskTwo.Level.Components;
using UnityEngine.AddressableAssets;
using Zenject;

namespace TaskTwo.Level.Utils
{
	public class LevelFactory : PlaceholderFactory<LevelController>
	{
		private DiContainer _container;
		private LevelManager _levelManager;

		[Inject]
		public void Construct(DiContainer container
			, LevelSetupService levelSetupService
			, LevelManager levelManager)
		{
			_container = container;
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
					if (!ReferenceEquals(_levelManager.currentLevelController, null))
					{
						var oldLevel = _levelManager.currentLevelController;
						_levelManager.currentLevelController = levelController;
						levelController.transform.position = oldLevel.finishArea.position;
						oldLevel.gameObject.SetActive(false);
					}
					else
						_levelManager.currentLevelController = levelController;
					
					
					_container.Inject(levelController);
					LevelEvents.OnLevelReady?.Invoke();
				}
			};
		}
	}
}