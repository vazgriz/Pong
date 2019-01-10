using System;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public abstract class NetworkEngine : IDisposable {
    public EventBasedNetListener Listener { get; private set; }
    public NetManager NetManager { get; private set; }

    protected NetworkEngine() {
        Listener = new EventBasedNetListener();
        NetManager = new NetManager(Listener, "Pong");
        NetManager.UnconnectedMessagesEnabled = true;
        NetManager.DiscoveryEnabled = true;

        NetManager.Start();
    }

    protected NetworkEngine(int port) {
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
