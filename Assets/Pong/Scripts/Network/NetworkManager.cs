using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class NetworkManager : MonoBehaviour {
    public NetworkEngine Engine { get; private set; }

    void Update() {
        Engine?.Update();
    }

    public void StartClient() {
        if (Engine is ClientEngine) return;
        EndNetworkConnection();
        Engine = new ClientEngine();
    }

    public void StartServer() {
        if (Engine is ServerEngine) return;
        EndNetworkConnection();
        Engine = new ServerEngine();
    }

    public void EndNetworkConnection() {
        Engine?.Dispose();
        Engine = null;
    }
}
