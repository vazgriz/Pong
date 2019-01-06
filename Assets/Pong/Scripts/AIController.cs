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
        ballRB = ballGO.GetComponent<Rigidbody2D>();
    }

    public void SetBall(GameObject ballGO) {
        this.ballGO = ballGO;
    }

    void Update() {
        float paddleX = rb.position.x;
        float ballX = ballRB.position.x;
        float correction = (ballX - paddleX) / trans.localScale.x;

        paddle.Move(correction);
    }
}
