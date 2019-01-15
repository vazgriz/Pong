using System;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public abstract class NetworkEngine : IDisposable {
    protected NetworkManager Manager { get; private set; }
    public EventBasedNetListener Listener { get; private set; }
    public NetManager NetManager { get; private set; }

    protected NetworkEngine(NetworkManager manager) {
        Manager = manager;
        Listener = new EventBasedNetListener();
        NetManager = new NetManager(Listener, "Pong");
        NetManager.UnconnectedMessagesEnabled = true;
        NetManager.DiscoveryEnabled = true;

        NetManager.Start();
    }

    protected NetworkEngine(NetworkManager manager, int port) {
        Manager = manager;
        Listener = new EventBasedNetListener();
        NetManager = new NetManager(Listener, "Pong");
        NetManager.UnconnectedMessagesEnabled = true;
        NetManager.DiscoveryEnabled = true;

        NetManager.Start(port);
    }

    public virtual void Dispose() {
        NetManager.Stop();
    }

    public void Update() {
        NetManager.PollEvents();
    }
}
