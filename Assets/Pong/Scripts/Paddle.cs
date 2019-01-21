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
    Collider2D col;

    public float MoveDir { get; private set; }
    public float Position { get; set; }
    public float Velocity { get; set; }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Move(float dir) {
        MoveDir = dir;
    }

    void Update() {
        float dir = Mathf.Clamp(MoveDir, -1, 1);

        if (Mathf.Abs(dir) > 0.1f) {
            Velocity = Velocity + dir * acceleration * Time.deltaTime;
        } else {
            Velocity = Mathf.Sign(Velocity) * Mathf.Max(0, Mathf.Abs(Velocity) - acceleration * Time.deltaTime);
        }

        Velocity = Mathf.Clamp(Velocity, -speed, speed);

        Position = rb.position.x + Velocity * Time.deltaTime;

        if (Position <= -extents) {
            Position = -extents;
            Velocity = 0;
        } else if (Position >= extents) {
            Position = extents;
            Velocity = 0;
        }

        rb.position = new Vector2(Position, rb.position.y);
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
