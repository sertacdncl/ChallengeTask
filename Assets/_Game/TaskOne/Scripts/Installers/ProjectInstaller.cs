using TaskOne.Addressable.Utils;
using Zenject;

namespace TaskOne.Installers
{
	public class ProjectInstaller : MonoInstaller<ProjectInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<AddressableLoader>().AsSingle().NonLazy();
		}
	}
}
