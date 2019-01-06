using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    [SerializeField]
    GameObject ball;

    Transform trans;
    Paddle paddle;
    Rigidbody2D rb;
    Rigidbody2D ballRB;

    void Start() {
        trans = GetComponent<Transform>();
        paddle = GetComponent<Paddle>();
        rb = GetComponent<Rigidbody2D>();
        ballRB = ball.GetComponent<Rigidbody2D>();
    }

    void Update() {
        float paddleX = rb.position.x;
        float ballX = ballRB.position.x;
        float correction = (ballX - paddleX) / trans.localScale.x;

        paddle.Move(correction);
    }
}
