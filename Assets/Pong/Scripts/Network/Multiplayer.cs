using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player {
    Server,
    Client
}

public class Multiplayer : Game {
    [SerializeField]
    GameObject localPaddlePrefab = null;
    [SerializeField]
    GameObject remotePaddlePrefab = null;
    [SerializeField]
    GameObject ballPrefab = null;
    [SerializeField]
    float timeLimit = 180f;

    GameObject localPaddleGO;
    Paddle localPaddle;
    GameObject remotePaddleGO;
    Paddle remotePaddle;
    GameObject ballGO;
    Ball ball;
    RectTransform arrow;

    bool authoritative;
    float clock;
    bool clockRunning;
    bool gameStarted;
    int localScore;
    int remoteScore;
    ScoreUI localScoreUI;
    ScoreUI remoteScoreUI;

    public override void Load() {
        localPaddleGO = Instantiate(localPaddlePrefab);
        localPaddle = localPaddleGO.GetComponent<Paddle>();
        remotePaddleGO = Instantiate(remotePaddlePrefab);
        remotePaddle = remotePaddleGO.GetComponent<Paddle>();
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

    public void InitGame(bool authoritative) {
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

    public void StartGame() {
        gameStarted = true;
        Manager.UI.Message.Hide();

        if (authoritative) {
            StartMessage start = new StartMessage();
            Manager.Network.Engine.Send(start, LiteNetLib.SendOptions.ReliableOrdered);
        }
    }

    protected override void Update() {
        if (!gameStarted) return;

        if (clockRunning) {
            clock -= Time.deltaTime;
        }
        
        PaddleUpdateMessage update = new PaddleUpdateMessage();
        update.Position = localPaddle.Position;
        update.Velocity = localPaddle.Velocity;
        update.Input = localPaddle.MoveDir;

        Manager.Network.Engine.Send(update, LiteNetLib.SendOptions.Sequenced);
    }

    public override void NotifyGoal(GoalPosition position) {
        if (authoritative) {

        }
    }

    public void HandlePaddleUpdate(PaddleUpdateMessage update) {
        float input = -update.Input;
        remotePaddle.Move(input);
    }
}
