using UnityEngine;
using System.Collections;

public class TestManager : MonoBehaviour {

    // Use this for initialization
    void Start() {
        GameManager.Instance.StartGameLoop();
        GameManager.Instance.IsGameplay = true;
    }
}
