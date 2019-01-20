using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multiplayer : Game {
    [SerializeField]
    GameObject ballPrefab = null;
    [SerializeField]
    float timeLimit = 180f;
    
    GameObject ballGO;
    Ball ball;
    RectTransform arrow;

    bool authoritative;
    float clock;
    bool clockRunning;
    int localScore;
    int remoteScore;
    ScoreUI localScoreUI;
    ScoreUI remoteScoreUI;

    public override void Load() {
        ballGO = Instantiate(ballPrefab);
        ball = ballGO.GetComponent<Ball>();
        ballGO.SetActive(false);

        arrow = Manager.UI.Arrow;
        arrow.gameObject.SetActive(false);
        localScoreUI = Manager.UI.SouthScore;
        remoteScoreUI = Manager.UI.NorthScore;
        localScoreUI.SetScore(0);
        remoteScoreUI.SetPlayerName("Player 2");
        remoteScoreUI.SetScore(0);

        clock = timeLimit;
        clockRunning = false;
        localScore = 0;
        remoteScore = 0;

        int time = Mathf.RoundToInt(clock);
        Manager.UI.Clock.text = string.Format("{0}:{1:00}", time / 60, time % 60);
    }

    public override void Unload() {
        Destroy(ballGO);
    }

    public void StartGame(bool authoritative) {
        this.authoritative = authoritative;
        string localName = Manager.UI.PlayerName;
        if (localName == "") {
            if (authoritative) {
                localName = "Player 1";
            } else {
                localName = "Player 2";
            }
        }

        localScoreUI.SetPlayerName(localName);

        if (authoritative) {
            Manager.UI.Message.SetText("Waiting for player...");
            remoteScoreUI.SetPlayerName("Player 2");
        } else {
            Manager.UI.Message.SetText("Connecting...");
            remoteScoreUI.SetPlayerName("Player 1");
        }

        Manager.UI.Message.Show();
    }

    public void SetRemoteName(string remoteName) {
        if (remoteName == "") {
            if (authoritative) {
                remoteName = "Player 2";
            } else {
                remoteName = "Player 1";
            }
        }

        remoteScoreUI.SetPlayerName(remoteName);
    }

    protected override void Update() {
        if (clockRunning) {
            clock -= Time.deltaTime;
        }
    }

    public override void NotifyGoal(GoalPosition position) {
        if (authoritative) {

        }
    }
}
