using UnityEngine;

namespace TaskTwo.Stacks.Config
{
	[CreateAssetMenu(menuName = "Config/TaskTwo/Create StackSettingsData", fileName = "StackSettingsData", order = 0)]
	public class StackSettingsData : ScriptableObject
	{
		[SerializeField] private StackSettings _stackSettings;
		public StackSettings StackSettings => _stackSettings;
	}
}