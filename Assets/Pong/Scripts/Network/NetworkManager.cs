using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class NetworkManager : MonoBehaviour {
    [SerializeField]
    UIManager ui;

    public NetworkEngine Engine { get; private set; }

    void Update() {
        Engine?.Update();
    }

    public void StartClient() {
        if (Engine is ClientEngine) return;
        EndNetworkConnection();
        Engine = new ClientEngine(this);
    }

    public void StartServer() {
        if (Engine is ServerEngine) return;
        EndNetworkConnection();
        Engine = new ServerEngine(this);
    }

    public void EndNetworkConnection() {
        Engine?.Dispose();
        Engine = null;
    }

    public void ResetServers() {
        ui.ResetServers();
    }

    public void AddServer(NetEndPoint endpoint, string serverName) {
        ui.AddServer(endpoint, serverName);
    }

    public void FindServers() {
        ResetServers();
        StartClient();
        ((ClientEngine)Engine).FindServers();
    }
}
