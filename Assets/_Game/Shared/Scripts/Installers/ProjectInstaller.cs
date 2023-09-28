using Shared.Addressable.Utils;
using Zenject;

namespace Shared.Installers
{
	public class ProjectInstaller : MonoInstaller<ProjectInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<AddressableLoader>().AsSingle().NonLazy();
		}
	}
}
