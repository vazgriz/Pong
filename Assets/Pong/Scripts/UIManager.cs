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

    public void StartGame() {
        mainMenuGO.SetActive(false);
        gameUIGO.SetActive(true);
    }

    public void EndGame() {
        mainMenuGO.SetActive(true);
        gameUIGO.SetActive(false);
    }
}
