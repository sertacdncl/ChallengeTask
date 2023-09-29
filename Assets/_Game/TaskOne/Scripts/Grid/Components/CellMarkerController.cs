using DG.Tweening;
using Shared.Extensions;
using UnityEngine;

namespace TaskOne.Grid.Components
{
	public class CellMarkerController : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;
		
		private Tween _fadeTween;
		public void Mark(Transform parent)
		{
			var cellMarkerTransform = transform;
			cellMarkerTransform.SetParent(parent);
			cellMarkerTransform.localPosition = Vector3.zero;
			_spriteRenderer.color = _spriteRenderer.color.With(a: 0);
			_spriteRenderer.DOFade(1, 0.5f);
			gameObject.SetActive(true);
		}

		public void UnMarkFade(TweenCallback onComplete)
		{
			_fadeTween?.Kill();
			_fadeTween = _spriteRenderer.DOFade(0, 0.5f).OnComplete(onComplete);
		}

		public void Reset()
		{
			_fadeTween?.Kill();
			_spriteRenderer.color = _spriteRenderer.color.With(a: 1);
		}
	}
}