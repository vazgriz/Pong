using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject mainMenuGO;
    [SerializeField]
    GameObject multiplayerMenuGO;
    [SerializeField]
    GameObject gameUIGO;
    [SerializeField]
    ScoreUI northScore;
    [SerializeField]
    ScoreUI southScore;
    [SerializeField]
    RectTransform arrow;
    [SerializeField]
    MessageUI message;
    [SerializeField]
    Text clockText;
    [SerializeField]
    InputField playerName;
    [SerializeField]
    InputField connectIP;

    public ScoreUI NorthScore {
        get {
            return northScore;
        }
    }

    public ScoreUI SouthScore {
        get {
            return southScore;
        }
    }

    public RectTransform Arrow {
        get {
            return arrow;
        }
    }

    public MessageUI Message {
        get {
            return message;
        }
    }

    public Text Clock {
        get {
            return clockText;
        }
    }

    public string PlayerName {
        get {
            return playerName.text;
        }
    }

    public string ConnectIP {
        get {
            return connectIP.text;
        }
    }

    public void StartGame() {
        mainMenuGO.SetActive(false);
        gameUIGO.SetActive(true);
    }

    public void EndGame() {
        mainMenuGO.SetActive(true);
        gameUIGO.SetActive(false);
    }

    public void OpenMultiplayerMenu() {
        multiplayerMenuGO.SetActive(true);
    }

    public void CloseMultiplayerMenu() {
        multiplayerMenuGO.SetActive(false);
    }

    public void ResetServers() {
        Debug.Log("Reset");
    }

    public void AddServer(LiteNetLib.NetEndPoint endpoint, string serverName) {
        Debug.Log(string.Format("{1} ({0})", endpoint, serverName));
    }
}
