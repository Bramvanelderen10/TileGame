using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public Transform mainMenu, optionMenu;

    public void LoadScene(string name){
        SceneManager.LoadScene(name);
    }
}