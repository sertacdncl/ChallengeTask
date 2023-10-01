using TaskTwo.Audio;
using TaskTwo.Camera;
using TaskTwo.Level;
using TaskTwo.Level.Components;
using TaskTwo.Level.Utils;
using TaskTwo.Stacks;
using TaskTwo.Stacks.Config;
using TaskTwo.Stacks.Utils;
using TaskTwo.UI;
using UnityEngine;
using Zenject;

namespace TaskTwo.Installers
{
	public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
	{
		[SerializeField] private LevelSetupService _levelSetupService;
		[SerializeField] private LevelManager _levelManager;
		[SerializeField] private StacksManager _stacksManager;
		[SerializeField] private AudioManager _audioManager;
		[SerializeField] private UIManager _uiManager;
		[SerializeField] private CameraManager _cameraManager;
		
		public override void InstallBindings()
		{
			Container.BindInstance((StackSettingsData)Resources.Load("Data/StackSettingsData")).AsSingle().NonLazy();
			Container.BindInstance(_levelSetupService).AsSingle().NonLazy();
			Container.BindInstance(_levelManager).AsSingle().NonLazy();
			Container.BindInstance(_stacksManager).AsSingle().NonLazy();
			Container.BindInstance(_audioManager).AsSingle().NonLazy();
			Container.BindInstance(_uiManager).AsSingle().NonLazy();
			Container.BindInstance(_cameraManager).AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<StackPoolService>().AsSingle().NonLazy();
			Container.BindFactory<LevelController, LevelFactory>().AsSingle().NonLazy();
		}
	}
}