using DG.Tweening;
using Shared.Extensions;
using UnityEngine;

namespace TaskOne.Grid.Components
{
	public class CellMarkerController : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;
		
		private Tween _fadeTween;
		private Color _baseColor;
		private void Awake()
		{
			_baseColor = _spriteRenderer.color;
		}

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

		public void MatchEffect()
		{
			_spriteRenderer.DOColor(Color.red,0.2f);
		}

		public void Reset()
		{
			_spriteRenderer.color = _baseColor;
			_fadeTween?.Kill();
			_spriteRenderer.color = _spriteRenderer.color.With(a: 1);
		}
	}
}