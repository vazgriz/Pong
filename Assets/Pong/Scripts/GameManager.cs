﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField]
    GameObject uiGO;
    UIManager ui;

    public Game CurrentGame { get; private set; }
    public UIManager UI {
        get {
            return ui;
        }
    }

    void Start() {
        if (ServiceLocator<GameManager>.Instance != null) {
            Destroy(gameObject);
            Destroy(uiGO);
            return;
        }

        ServiceLocator<GameManager>.Instance = this;

        ui = uiGO.GetComponent<UIManager>();
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(uiGO.gameObject);
    }

    void UnloadGame() {
        CurrentGame?.Unload();
        if (CurrentGame != null) CurrentGame.enabled = false;
    }

    public void LoadSinglePlayer() {
        UnloadGame();
        CurrentGame = GetComponent<SinglePlayer>();
        StartCoroutine(Load(1));
    }

    public void LoadMultiPlayer() {
        UnloadGame();
        var async = SceneManager.LoadSceneAsync(1);
    }

    public void OpenMainMenu() {
        StartCoroutine(LoadMainMenu());
    }

    public void Quit() {
        Application.Quit();
    }

    IEnumerator Load(int index) {
        yield return SceneManager.LoadSceneAsync(index);
        ui.StartGame();
        CurrentGame?.Load();
        if (CurrentGame != null) CurrentGame.enabled = true;
    }

    IEnumerator LoadMainMenu() {
        UnloadGame();
        yield return SceneManager.LoadSceneAsync(0);
        UI.EndGame();
    }
}

public abstract class Game : MonoBehaviour {
    protected GameManager Manager { get; private set; }

    void Awake() {
        Manager = GetComponent<GameManager>();
    }

    protected virtual void Update() { }

    public abstract void Load();
    public abstract void Unload();
    public abstract void NotifyGoal(GoalPosition position);
}
