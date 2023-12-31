using TaskTwo.Game;
using UnityEngine;

namespace TaskTwo.UI
{
	public class UIManager : MonoBehaviour
	{
		[SerializeField] private Canvas _mainCanvas;
		[SerializeField] private Canvas _successCanvas;
		[SerializeField] private Canvas _failCanvas;

		public void ToggleFailCanvas(bool active)
		{
			_failCanvas.enabled = active;
		}

		public void ToggleSuccessCanvas(bool active)
		{
			_successCanvas.enabled = active;
		}

		public void ToggleMainCanvas(bool active)
		{
			_mainCanvas.enabled = active;
		}

		public void OnClick_TryAgain()
		{
			ToggleFailCanvas(false);
			GameEvents.OnTryAgain?.Invoke();
			ToggleMainCanvas(true);
		}
	}
}