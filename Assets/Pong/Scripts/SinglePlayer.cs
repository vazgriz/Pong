using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : Game {
    [SerializeField]
    GameObject aiPrefab;
    [SerializeField]
    GameObject ballPrefab;

    GameObject ai;
    GameObject ballGO;
    Ball ball;
    RectTransform arrow;

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
        computerScoreUI.SetPlayerName("Computer");

        StartCoroutine(LaunchBall(1));
    }

    public override void Unload() {
        Destroy(ai);
        Destroy(ballGO);
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

        StartCoroutine(LaunchBall(dir));
    }

    IEnumerator LaunchBall(int dir) {
        float angle = ball.SelectAngle(dir);
        arrow.gameObject.SetActive(true);
        arrow.localEulerAngles = new Vector3(0, 0, angle);
        ai.GetComponent<AIController>().SetBall(null);
        ballGO.SetActive(false);

        yield return new WaitForSeconds(1);

        arrow.gameObject.SetActive(false);
        ai.GetComponent<AIController>().SetBall(ballGO);
        ballGO.SetActive(true);
        ball.Launch(angle);
    }
}
