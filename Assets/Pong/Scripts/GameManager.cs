using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField]
    GameObject ui;

    void Start() {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(ui.gameObject);
    }

    public void LoadSinglePlayer() {
        SceneManager.LoadScene(1);
    }

    public void LoadMultiPlayer() {
        SceneManager.LoadScene(1);
    }

    public void Quit() {
        Application.Quit();
    }
}
