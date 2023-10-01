using TaskTwo.Game;
using UnityEngine;

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
		GameEvents.OnTryAgain?.Invoke();
		ToggleFailCanvas(false);
		ToggleMainCanvas(true);
	}
}