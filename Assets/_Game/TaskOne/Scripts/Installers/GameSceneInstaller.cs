using TaskOne.Grid;
using TaskOne.Grid.Config;
using TaskOne.Grid.Utils;
using UnityEngine;
using Zenject;

namespace TaskOne.Installers
{
	public class GameSceneInstaller : MonoInstaller<GameSceneInstaller>
	{
		[SerializeField] private GridManager _gridManager;
		public override void InstallBindings()
		{
			Container.BindInstance((GridSettingsData)Resources.Load("Data/GridSettingsData")).AsSingle();
			Container.BindInstance(_gridManager).AsSingle().NonLazy();
			Container.Bind<GridSetupService>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<CellMarkerPoolService>().AsSingle().NonLazy();
		}
	}
}