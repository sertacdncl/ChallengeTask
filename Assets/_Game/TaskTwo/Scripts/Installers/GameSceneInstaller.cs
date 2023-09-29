using TaskTwo.Level;
using TaskTwo.Level.Components;
using TaskTwo.Level.Utils;
using TaskTwo.Stacks;
using TaskTwo.Stacks.Config;
using TaskTwo.Stacks.Utils;
using UnityEngine;
using Zenject;

namespace TaskTwo.Installers
{
	public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
	{
		[SerializeField] private LevelSetupService _levelSetupService;
		[SerializeField] private LevelManager _levelManager;
		[SerializeField] private StacksManager _stacksManager;
		
		public override void InstallBindings()
		{
			Container.BindInstance((StackSettingsData)Resources.Load("Data/StackSettingsData")).AsSingle().NonLazy();
			Container.BindInstance(_levelSetupService).AsSingle().NonLazy();
			Container.BindInstance(_levelManager).AsSingle().NonLazy();
			Container.BindInstance(_stacksManager).AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<StackPoolService>().AsSingle().NonLazy();
			Container.BindFactory<LevelController, LevelFactory>().AsSingle().NonLazy();
		}
	}
}