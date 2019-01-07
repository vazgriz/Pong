using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {
    [SerializeField]
    Text playerName;
    [SerializeField]
    Text score;

    public void SetPlayerName(string name) {
        playerName.text = name;
    }

    public void SetScore(int score) {
        this.score.text = score.ToString();
    }
}
