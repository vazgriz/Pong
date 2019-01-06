using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour {
    [SerializeField]
    float speed = 1;
    [SerializeField]
    float acceleration = 1;
    [SerializeField]
    float extents = 0;

    Rigidbody2D rb;
    float moveDir;
    float velocity;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(float dir) {
        moveDir = dir;
    }

    void Update() {
        if (Mathf.Abs(moveDir) > 0.1f) {
            velocity = velocity + moveDir * acceleration * Time.deltaTime;
        } else {
            velocity = Mathf.Sign(velocity) * Mathf.Max(0, Mathf.Abs(velocity) - acceleration * Time.deltaTime);
        }

        velocity = Mathf.Clamp(velocity, -speed, speed);

        float x = rb.position.x + velocity * Time.deltaTime;

        if (x <= -extents) {
            x = -extents;
            velocity = 0;
        } else if (x >= extents) {
            x = extents;
            velocity = 0;
        }

        rb.position = new Vector2(x, rb.position.y);
    }
}
