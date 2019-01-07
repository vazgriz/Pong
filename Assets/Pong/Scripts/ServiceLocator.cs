using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator<T> {
    public static T Instance { get; set; }
}