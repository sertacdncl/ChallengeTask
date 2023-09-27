using System;
using Cinemachine;
using TaskOne.Grid;
using TaskOne.Grid.Config;
using TaskOne.Grid.Utils;
using UnityEngine;
using Zenject;

namespace TaskOne.Camera
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private CinemachineVirtualCamera _virtualCamera;
		[Inject] private readonly GridManager _gridManager;
		[Inject] private readonly GridSettingsData _gridSettingsData;
		private GridManager.Settings _gridSettings => _gridSettingsData.GridSettings;
		private const float CameraSizeOffset = 0.75f;

		private void OnEnable()
		{
			GridEvents.OnGridSetupComplete += Setup;
		}

		private void OnDisable()
		{
			GridEvents.OnGridSetupComplete -= Setup;
		}

		private void Setup()
		{
			var gridWidth = _gridSettings.width;
			var gridHeight = _gridSettings.height;
			var cellDistance = _gridSettings.distance;
			
			float gridWidthWorld = gridWidth * cellDistance;
			float gridHeightWorld = gridHeight * cellDistance;
			float cameraOrthoSize = Mathf.Max(gridWidthWorld / (2f * _virtualCamera.m_Lens.Aspect), gridHeightWorld / 2f);
			
			_virtualCamera.m_Lens.OrthographicSize = cameraOrthoSize + CameraSizeOffset;
			_virtualCamera.transform.position = new Vector3(gridWidthWorld / 2f, gridHeightWorld / 2f, -10f);
		}
	}
}
