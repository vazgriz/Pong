﻿using System.Collections;
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
    Collider2D col;
    float moveDir;
    float velocity;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Move(float dir) {
        moveDir = dir;
    }

    void Update() {
        float dir = Mathf.Clamp(moveDir, -1, 1);

        if (Mathf.Abs(dir) > 0.1f) {
            velocity = velocity + dir * acceleration * Time.deltaTime;
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

    IEnumerator Debounce() {
        col.enabled = false;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        col.enabled = true;
    }

    void OnCollisionEnter(Collision collision) {
        Ball ball = collision.gameObject.GetComponent<Ball>();

        if (ball != null) {
            StartCoroutine(Debounce());
        }
    }
}
