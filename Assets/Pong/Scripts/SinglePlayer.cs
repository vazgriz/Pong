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

    int playerScore;
    int computerScore;
    ScoreUI playerScoreUI;
    ScoreUI computerScoreUI;

    public override void Load() {
        ai = Instantiate(aiPrefab);
        ballGO = Instantiate(ballPrefab);
        ball = ballGO.GetComponent<Ball>();

        ai.GetComponent<AIController>().SetBall(ballGO);

        playerScoreUI = Manager.UI.SouthScore;
        computerScoreUI = Manager.UI.NorthScore;
        playerScoreUI.SetPlayerName("Player 1");
        computerScoreUI.SetPlayerName("Computer");

        ball.Launch(1);
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

        ball.Launch(dir);
    }
}
