using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public int sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void NewGame() {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
