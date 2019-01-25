using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player {
    None,
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
    Player localRole;
    float clock;
    bool clockRunning;
    int localScore;
    int remoteScore;
    ScoreUI localScoreUI;
    ScoreUI remoteScoreUI;

    public bool GameRunning { get; private set; }

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

        if (authoritative) {
            localRole = Player.Server;
        } else {
            localRole = Player.Client;
        }

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
        GameRunning = true;
        Manager.UI.Message.Hide();

        if (authoritative) {
            StartMessage start = new StartMessage();
            Manager.Network.Engine.Send(start, LiteNetLib.SendOptions.ReliableOrdered);

            LaunchBall(1);
        }
    }

    public void StopGame() {
        GameRunning = false;
        clockRunning = false;
        ball.Freeze();
    }

    protected override void Update() {
        if (!GameRunning) return;

        if (clockRunning) {
            clock -= Time.deltaTime;

            if (authoritative) {
                BallUpdateMessage ballUpdate = new BallUpdateMessage();
                ballUpdate.Position = ball.Position;
                ballUpdate.Velocity = ball.Velocity;

                Manager.Network.Engine.Send(ballUpdate, LiteNetLib.SendOptions.Sequenced);
            }
        }
        
        PaddleUpdateMessage paddleUpdate = new PaddleUpdateMessage();
        paddleUpdate.Position = localPaddle.Position;
        paddleUpdate.Velocity = localPaddle.Velocity;
        paddleUpdate.Input = localPaddle.MoveDir;

        Manager.Network.Engine.Send(paddleUpdate, LiteNetLib.SendOptions.Sequenced);

        if (clock <= 0f) {
            if (authoritative) {
                if (localScore == remoteScore) {
                    EndGame(Player.None);
                } else if (localScore > remoteScore) {
                    EndGame(Player.Server);
                } else if (remoteScore > localScore) {
                    EndGame(Player.Client);
                }
            }
        } else {
            int time = Mathf.Max(Mathf.RoundToInt(clock), 0);
            Manager.UI.Clock.text = string.Format("{0}:{1:00}", time / 60, time % 60);
        }
    }

    public override void NotifyGoal(GoalPosition position) {
        if (authoritative) {
            if (position == GoalPosition.North) {
                HandleGoal(Player.Server);
            } else if (position == GoalPosition.South) {
                HandleGoal(Player.Client);
            }
        } else {
            ball.Freeze();
            ballGO.SetActive(false);
        }
    }

    void HandleGoal(Player player) {
        int dir = UpdateScore(player);
        clockRunning = false;

        if (authoritative) {
            GoalMessage goal = new GoalMessage();
            goal.Player = player;
            goal.Time = clock;

            Manager.Network.Engine.Send(goal, LiteNetLib.SendOptions.ReliableOrdered);
        }

        if (localScore >= 5) {
            EndGame(Player.Server);
        } else if (remoteScore >= 5) {
            EndGame(Player.Client);
        } else {
            LaunchBall(dir);
        }
    }

    public void HandlePaddleUpdate(PaddleUpdateMessage update) {
        float input = -update.Input;
        float position = -update.Position;
        float velocity = -update.Velocity;

        remotePaddle.Move(input);

        if (!authoritative) {
            remotePaddle.Velocity = -velocity;

            if (Mathf.Abs(remotePaddle.Position - position) > 0.25f) {
                remotePaddle.Position = position;
            }
        }
    }

    public void HandleBallUpdate(BallUpdateMessage update) {
        if (!GameRunning) return;

        //transform into local space
        Vector2 position = Ball.RotatePoint(update.Position, 180);
        Vector2 velocity = Ball.RotatePoint(update.Velocity, 180);

        ball.Position = position;
        ball.Velocity = velocity;
        ballGO.SetActive(true);
    }

    public void HandleBallLaunch(LaunchBallMessage launch) {
        clock = launch.Time;
        StartCoroutine(LaunchBall((launch.Angle + 540) % 360f));
    }

    public void HandleGoal(GoalMessage goal) {
        clock = goal.Time;
        clockRunning = false;
        UpdateScore(goal.Player);
    }

    public void HandleEndGame(EndGameMessage endGame) {
        EndGame(endGame.Winner);
    }

    int UpdateScore(Player player) {
        ball.Freeze();
        ballGO.SetActive(false);

        if (player == localRole) {
            localScore++;
            Manager.UI.SouthScore.SetScore(localScore);
            return -1;
        } else {
            remoteScore++;
            Manager.UI.NorthScore.SetScore(remoteScore);
            return 1;
        }
    }

    void LaunchBall(int dir) {
        float angle = ball.SelectAngle(dir);
        StartCoroutine(LaunchBall(angle));
    }

    IEnumerator LaunchBall(float angle) {
        arrow.gameObject.SetActive(true);
        arrow.localEulerAngles = new Vector3(0, 0, angle);

        if (authoritative) {
            LaunchBallMessage launch = new LaunchBallMessage();
            launch.Angle = angle;
            launch.Time = clock;
            
            Manager.Network.Engine.Send(launch, LiteNetLib.SendOptions.ReliableOrdered);
        }

        yield return new WaitForSeconds(1);

        arrow.gameObject.SetActive(false);
        ballGO.SetActive(true);
        ball.Launch(angle);
        clockRunning = true;
    }

    void EndGame(Player winner) {
        if (winner == Player.None) {
            Manager.UI.Message.SetText("Draw");
        } else if (winner == localRole) {
            Manager.UI.Message.SetText("You win!");
        } else {
            Manager.UI.Message.SetText("You lose!");
        }

        Manager.UI.Message.Show();
        StopGame();

        if (authoritative) {
            EndGameMessage message = new EndGameMessage();
            message.Winner = winner;

            Manager.Network.Engine.Send(message, LiteNetLib.SendOptions.ReliableOrdered);
        }
    }
}
