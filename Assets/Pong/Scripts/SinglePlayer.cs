using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : Game {
    [SerializeField]
    GameObject aiPrefab;
    [SerializeField]
    GameObject ballPrefab;

    GameObject ai;
    GameObject ball;

    int playerScore;
    int computerScore;
    ScoreUI playerScoreUI;
    ScoreUI computerScoreUI;

    public override void Load() {
        ai = Instantiate(aiPrefab);
        ball = Instantiate(ballPrefab);

        playerScoreUI = Manager.UI.SouthScore;
        computerScoreUI = Manager.UI.NorthScore;
        playerScoreUI.SetPlayerName("Player 1");
        computerScoreUI.SetPlayerName("Computer");

        ai.GetComponent<AIController>().SetBall(ball);
    }

    public override void Unload() {
        Destroy(ai);
        Destroy(ball);
    }

    public override void NotifyGoal(GoalPosition position) {
        if (position == GoalPosition.North) {
            playerScore++;
            Manager.UI.SouthScore.SetScore(playerScore);
        } else if (position == GoalPosition.South) {
            computerScore++;
            Manager.UI.NorthScore.SetScore(computerScore);
        }
    }
}
