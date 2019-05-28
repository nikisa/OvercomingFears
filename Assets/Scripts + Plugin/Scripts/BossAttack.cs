using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour { 

    public GameManager _gm;

    void Awake() {
        _gm = Object.FindObjectOfType<GameManager>().GetComponent<GameManager>();    
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            _gm.GetComponent<GameManager>().LoseLevel();
        }
    }
}
