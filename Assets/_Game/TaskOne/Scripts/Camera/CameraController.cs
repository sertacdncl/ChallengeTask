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
			
			float aspectRatio = (float)Screen.width / Screen.height;
			float gridWidthWorld = gridWidth * cellDistance;
			float gridHeightWorld = gridHeight * cellDistance;
			float cameraOrthoSize = Mathf.Max(gridWidthWorld / (2 * aspectRatio), gridHeightWorld / 2);

			var heightOffset = gridHeight > gridWidth ? 1f : 0f;
			_virtualCamera.m_Lens.OrthographicSize = cameraOrthoSize + CameraSizeOffset + heightOffset;
			_virtualCamera.transform.position = new Vector3(gridWidthWorld / 2f, gridHeightWorld / 2f, -10f);
		}
	}
}
