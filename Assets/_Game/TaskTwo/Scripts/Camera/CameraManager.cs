using System.Collections;
using Cinemachine;
using DG.Tweening;
using TaskTwo.Level.Utils;
using UnityEngine;

namespace TaskTwo.Camera
{
	public class CameraManager : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera _gamePlayCamera;
		[SerializeField] private CinemachineVirtualCamera _finishCamera;
		private Coroutine _rotateCoroutine;
		private float _rotationSpeed = 100f;

		private void OnEnable()
		{
			LevelEvents.OnLevelFinished += OnLevelFinished;
			LevelEvents.OnNextLevel += OnNextLevelStart;
		}

		private void OnDisable()
		{
			LevelEvents.OnLevelFinished -= OnLevelFinished;
			LevelEvents.OnNextLevel -= OnNextLevelStart;
		}

		public void SetFollowTarget(Transform target)
		{
			_gamePlayCamera.Follow = target;
		}

		private void OnLevelFinished()
		{
			_finishCamera.Priority = _gamePlayCamera.Priority + 1;
			_rotateCoroutine = StartCoroutine(StartRotateCamera());
		}

		private void OnNextLevelStart()
		{
			_finishCamera.Priority = _gamePlayCamera.Priority - 1;
			if (_rotateCoroutine != null)
			{
				StopCoroutine(_rotateCoroutine);
				var cinemachineOrbitalTransposer = _finishCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
				cinemachineOrbitalTransposer.m_XAxis.Value = -180f;
			}
		}

		private IEnumerator StartRotateCamera()
		{
			var cinemachineOrbitalTransposer = _finishCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
			while (true)
			{
				cinemachineOrbitalTransposer.m_XAxis.Value += _rotationSpeed * Time.deltaTime;
				yield return null;
			}
		}
	}
}