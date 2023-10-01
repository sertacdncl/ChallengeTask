using DG.Tweening;
using TaskTwo.Stacks.Config;
using UnityEngine;

namespace TaskTwo.Stacks.Components
{
	public class StackController : MonoBehaviour
	{
		public MeshRenderer meshRenderer;
		public new Collider collider;
		public new Rigidbody rigidbody;
		public Tween moveTween;
		public void StartMove(float targetPos, float offSet, StackSettings settings)
		{
			// transform.position.WithAddX(offSet);
			moveTween = transform.DOMoveX(targetPos, settings.moveTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		}
		
		public void ToggleRigidbodyKinematic(bool active)
		{
			rigidbody.isKinematic = active;
		}
		
		public void SetParent(Transform parent)
		{
			transform.SetParent(parent);
		}
		
		public void SetMeshMaterial(Material material)
		{
			meshRenderer.material = new Material(material);
		}
	}
}