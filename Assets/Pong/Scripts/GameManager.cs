using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [SerializeField]
    GameObject uiGO;
    UIManager ui;
    [SerializeField]
    NetworkManager network;

    public Game CurrentGame { get; private set; }

    public UIManager UI {
        get {
            return ui;
        }
    }


    public NetworkManager Network {
        get {
            return network;
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
        CurrentGame = GetComponent<SinglePlayer>();
        StartCoroutine(Load(1));
    }

    public void OpenMultiPlayerMenu() {
        ui.OpenMultiplayerMenu();
        network.StartClient();
    }

    public void CloseMultiplayerMenu() {
        ui.CloseMultiplayerMenu();
        network.EndNetworkConnection();
    }

    public void HostMultiplayer() {
        CurrentGame = GetComponent<Multiplayer>();
        network.StartServer();
        StartCoroutine(Load(1, true));
    }

    public void ConnectMultiplayer() {
        CurrentGame = GetComponent<Multiplayer>();
        network.StartClient();
        StartCoroutine(Load(1, false));
    }

    public void OpenMainMenu() {
        UnloadGame();
        network.EndNetworkConnection();
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

    IEnumerator Load(int index, bool authoritative) {
        yield return Load(index);
        (CurrentGame as Multiplayer).InitGame(authoritative);

        if (!authoritative) {
            try {
                (network.Engine as ClientEngine).Connect(UI.ConnectIP);
            } catch (System.Net.Sockets.SocketException e) {
                UI.Message.SetText("Could not connect");
                UI.Message.Show();
                UnloadGame();
                network.EndNetworkConnection();
            }
        }
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
