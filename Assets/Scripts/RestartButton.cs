using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour {

    public string scene;

	public void Restart()
    {
        SceneManager.LoadScene(scene);
    }
}
