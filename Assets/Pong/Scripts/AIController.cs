using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    [SerializeField]
    GameObject ball;

    Paddle paddle;
    Rigidbody2D rb;
    Rigidbody2D ballRB;

    void Start() {
        paddle = GetComponent<Paddle>();
        rb = GetComponent<Rigidbody2D>();
        ballRB = ball.GetComponent<Rigidbody2D>();
    }

    void Update() {
        float paddleX = rb.position.x;
        float ballX = ballRB.position.x;

        paddle.Move(ballX - paddleX);
    }
}
