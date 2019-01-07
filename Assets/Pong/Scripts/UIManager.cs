using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    [SerializeField]
    GameManager gameManager;
    [SerializeField]
    GameObject mainMenuGO;
    [SerializeField]
    GameObject gameUIGO;
    [SerializeField]
    ScoreUI northScore;
    [SerializeField]
    ScoreUI southScore;
    [SerializeField]
    RectTransform arrow;

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

    public void StartGame() {
        mainMenuGO.SetActive(false);
        gameUIGO.SetActive(true);
    }

    public void EndGame() {
        mainMenuGO.SetActive(true);
        gameUIGO.SetActive(false);
    }
}
