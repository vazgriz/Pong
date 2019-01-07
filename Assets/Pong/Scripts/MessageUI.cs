using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour {
    [SerializeField]
    Text text;

    public void SetText(string text) {
        this.text.text = text;
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
