using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEngine : NetworkEngine {
    public ServerEngine(NetworkManager manager) : base(manager, 4444) {

    }

    public override void Dispose() {

    }
}
