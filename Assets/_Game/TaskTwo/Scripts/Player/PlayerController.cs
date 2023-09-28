using UnityEngine;

namespace TaskTwo.Player
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private float _playerSpeed = 5f;
		private const float TimeMultiplier = 3;
		private Rigidbody _playerRigidbody;
		public PlayerState playerState;
	
		private void Start()
		{
			_playerRigidbody = GetComponent<Rigidbody>();
		}
		
		private void FixedUpdate()
		{
			var myTransform = transform;
			Vector3 drawPosition = myTransform.position + Vector3.up + (Vector3.forward * .1f);
			var ray = new Ray(drawPosition, -myTransform.up);

			if (Physics.Raycast(ray, out var hit, 10))
			{
				MoveForward();
			}
			else
			{
				_playerSpeed = 0;
			}
		}
	
		private void MoveForward()
		{
			_playerRigidbody.velocity = (Vector3.forward * _playerSpeed);

		}

		public enum PlayerState
		{
			Idle,
			Run,
			Dance
		}
	}
}
