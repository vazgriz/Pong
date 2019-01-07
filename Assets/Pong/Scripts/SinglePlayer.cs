using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : Game {
    [SerializeField]
    GameObject aiPrefab;
    [SerializeField]
    GameObject ballPrefab;
    [SerializeField]
    float timeLimit = 180;

    GameObject ai;
    GameObject ballGO;
    Ball ball;
    RectTransform arrow;

    float clock;
    bool clockRunning;
    int playerScore;
    int computerScore;
    ScoreUI playerScoreUI;
    ScoreUI computerScoreUI;

    public override void Load() {
        ai = Instantiate(aiPrefab);
        ballGO = Instantiate(ballPrefab);
        ball = ballGO.GetComponent<Ball>();

        ai.GetComponent<AIController>().SetBall(ballGO);

        arrow = Manager.UI.Arrow;
        playerScoreUI = Manager.UI.SouthScore;
        computerScoreUI = Manager.UI.NorthScore;
        playerScoreUI.SetPlayerName("Player 1");
        playerScoreUI.SetScore(0);
        computerScoreUI.SetPlayerName("Computer");
        computerScoreUI.SetScore(0);

        StartCoroutine(LaunchBall(1));

        clock = timeLimit;
        clockRunning = false;
        playerScore = 0;
        computerScore = 0;
    }

    public override void Unload() {
        Destroy(ai);
        Destroy(ballGO);
    }

    protected override void Update() {
        if (clockRunning) {
            clock -= Time.deltaTime;
        }

        if (clock <= 0f) {
            if (playerScore == computerScore) {
                EndGame("Draw");
            } else if (playerScore > computerScore) {
                EndGame("You win!");
            } else if (computerScore > playerScore) {
                EndGame("You lose!");
            }
        } else {
            int time = Mathf.RoundToInt(clock);
            Manager.UI.Clock.text = string.Format("{0}:{1:00}", time / 60, time % 60);
        }
    }

    public override void NotifyGoal(GoalPosition position) {
        int dir = 1;
        if (position == GoalPosition.North) {
            playerScore++;
            Manager.UI.SouthScore.SetScore(playerScore);
            dir = 1;
        } else if (position == GoalPosition.South) {
            computerScore++;
            Manager.UI.NorthScore.SetScore(computerScore);
            dir = -1;
        }

        if (playerScore >= 5) {
            EndGame("You win!");
        } else if (computerScore >= 5) {
            EndGame("You lose!");
        } else {
            StartCoroutine(LaunchBall(dir));
        }
    }

    void EndGame(string message) {
        Manager.UI.Message.SetText(message);
        Manager.UI.Message.Show();
        ballGO.SetActive(false);
    }

    IEnumerator LaunchBall(int dir) {
        float angle = ball.SelectAngle(dir);
        arrow.gameObject.SetActive(true);
        arrow.localEulerAngles = new Vector3(0, 0, angle);
        ai.GetComponent<AIController>().SetBall(null);
        ballGO.SetActive(false);
        clockRunning = false;

        yield return new WaitForSeconds(1);

        arrow.gameObject.SetActive(false);
        ai.GetComponent<AIController>().SetBall(ballGO);
        ballGO.SetActive(true);
        ball.Launch(angle);
        clockRunning = true;
    }
}
