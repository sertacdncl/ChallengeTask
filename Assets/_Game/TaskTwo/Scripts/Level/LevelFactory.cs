using UnityEngine;
using Zenject;

namespace TaskTwo.Level
{
	public class LevelFactory : PlaceholderFactory<GameObject>
	{
		private DiContainer _container;
		
		[Inject]
		public void Construct(DiContainer container)
		{
			_container = container;
		}
		
		
	}
}