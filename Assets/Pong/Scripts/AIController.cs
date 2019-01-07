using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    [SerializeField]
    GameObject ballGO;

    Transform trans;
    Paddle paddle;
    Rigidbody2D rb;
    Rigidbody2D ballRB;

    void Start() {
        trans = GetComponent<Transform>();
        paddle = GetComponent<Paddle>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetBall(GameObject ballGO) {
        this.ballGO = ballGO;

        if (ballGO != null) {
            ballRB = ballGO.GetComponent<Rigidbody2D>();
        } else {
            ballRB = null;
        }
    }

    void Update() {
        if (ballGO == null) {
            float paddleX = rb.position.x / trans.localScale.x;

            if (Mathf.Abs(paddleX) > 0.5f) {
                paddle.Move(-paddleX);
            } else {
                paddle.Move(0);
            }
        } else {
            if (Mathf.Sign(ballRB.velocity.y) == Mathf.Sign(rb.position.y)) {
                float paddleX = rb.position.x;
                float ballX = ballRB.position.x;
                float correction = (ballX - paddleX) / trans.localScale.x;

                paddle.Move(correction);
            } else {
                paddle.Move(0);
            }
        }
    }
}
