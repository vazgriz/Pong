using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField]
    Paddle paddle;

    public void Bind(Paddle paddle) {
        this.paddle = paddle;
    }

    void Update() {
        if (!paddle) return;

        float input = 0;

        if (Input.GetKey(KeyCode.A)) {
            input -= 1;
        }

        if (Input.GetKey(KeyCode.D)) {
            input += 1;
        }

        paddle.Move(input);
    }
}
