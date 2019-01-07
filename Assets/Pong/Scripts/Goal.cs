using System;
using System.Collections.Generic;
using UnityEngine;

public enum GoalPosition {
    North,
    South
}

public class Goal : MonoBehaviour {
    [SerializeField]
    GoalPosition position;
    GameManager gameManager;

    void Start() {
        if (ServiceLocator<GameManager>.Instance == null) throw new Exception("GameManager not loaded");
        gameManager = ServiceLocator<GameManager>.Instance;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Ball ball = collision.gameObject.GetComponent<Ball>();
        if (ball == null) return;

        gameManager.CurrentGame.NotifyGoal(position);
    }
}
