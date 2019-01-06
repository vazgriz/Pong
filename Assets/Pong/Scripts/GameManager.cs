using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField]
    GameObject ui;

    Game currentGame;

    void Start() {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(ui.gameObject);
    }

    void UnloadGame() {
        currentGame?.Unload();
    }

    public void LoadSinglePlayer() {
        UnloadGame();
        currentGame = GetComponent<SinglePlayer>();
        StartCoroutine(Load(1));
    }

    public void LoadMultiPlayer() {
        UnloadGame();
        var async = SceneManager.LoadSceneAsync(1);
    }

    public void Quit() {
        Application.Quit();
    }

    IEnumerator Load(int index) {
        yield return SceneManager.LoadSceneAsync(index);
        currentGame?.Load();
    }
}

public abstract class Game : MonoBehaviour {
    public abstract void Load();
    public abstract void Unload();
}
