using System.Collections.Generic;
using UnityEngine;

namespace TaskTwo.Effects
{
	public class ConfettiController : MonoBehaviour
	{
		[SerializeField] private Transform confettiLeft;
		[SerializeField] private Transform confettiRight;
	
		[SerializeField] private List<ParticleSystem> _confettiLeftParticles;

		private UnityEngine.Camera _camera;
		private void Start()
		{
			_camera = UnityEngine.Camera.main;
			var isCameraOrthographic = _camera.orthographic;
		
			SetupConfetti(isCameraOrthographic);
		}

		void SetupConfetti(bool orthoGraphic = false)
		{
			if (!orthoGraphic)
				return;

			var cameraSize = _camera.orthographicSize;
			var horizontalDistance = cameraSize / 2;
			var particleScale = Vector3.one * horizontalDistance;

			confettiLeft.localPosition = new Vector3(-horizontalDistance, -cameraSize, 2);
			confettiRight.localPosition = new Vector3(horizontalDistance, -cameraSize, 2);

			confettiLeft.localScale = particleScale;
			confettiRight.localScale = particleScale;
		}

		[ContextMenu("PlayParticle")]
		public void PlayParticle()
		{
			foreach (var confettiLeftParticle in _confettiLeftParticles)
			{
				confettiLeftParticle.Play();
				// confettiLeftParticle.Emit(1);
			}		
		}
	}
}