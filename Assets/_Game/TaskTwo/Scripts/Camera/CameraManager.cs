using Cinemachine;
using UnityEngine;

namespace TaskTwo.Camera
{
	public class CameraManager : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera _gamePlayCamera;
		
		public void SetFollowTarget(Transform target)
		{
			_gamePlayCamera.Follow = target;
		}
	}
}