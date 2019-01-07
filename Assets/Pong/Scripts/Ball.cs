﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;

public class Ball : MonoBehaviour {
    [SerializeField]
    int launchAngle = 30;
    [SerializeField]
    float launchVelocity = 10;

    Random rand;
    Random Rand {
        get {
            if (rand == null) {
                rand = new Random();
            }
            return rand;
        }
        set {
            rand = value;
        }
    }

    Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetSeed(int seed) {
        Rand = new Random(seed);
    }

    public void Launch(int direction) {
        rb.position = new Vector2();

        float angle = Rand.Next(-launchAngle / 2, launchAngle / 2);
        Vector2 dir = RotatePoint(new Vector2(0, 1), angle);

        if (direction < 0) {
            dir *= -1;
        }

        rb.velocity = dir * launchVelocity;
    }

    Vector2 RotatePoint(Vector2 point, float angle) {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

        float xNew = point.x * cos - point.y * sin;
        float yNew = point.x * sin + point.y * cos;

        point.x = xNew;
        point.y = yNew;
        return point;
    }
}
