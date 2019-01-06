using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayer : Game {
    [SerializeField]
    GameObject aiPrefab;
    [SerializeField]
    GameObject ballPrefab;

    GameObject ai;
    GameObject ball;

    public override void Load() {
        ai = Instantiate(aiPrefab);
        ball = Instantiate(ballPrefab);

        ai.GetComponent<AIController>().SetBall(ball);
    }

    public override void Unload() {
        Destroy(ai);
        Destroy(ball);
    }
}
