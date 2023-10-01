using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskLoader : MonoBehaviour
{
    public void OnClick_TaskOne()
	{
		SceneManager.LoadScene(1);
	}
	
	public void OnClick_TaskTwo()
	{
		SceneManager.LoadScene(2);
	}
}
