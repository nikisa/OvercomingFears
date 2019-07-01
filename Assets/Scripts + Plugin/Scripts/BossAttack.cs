using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour { 

    public GameManager _gm;
    public PlayerManager m_player;

    void Awake() {
        _gm = Object.FindObjectOfType<GameManager>().GetComponent<GameManager>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            m_player.playerInput.InputEnabled = false;
            _gm.GetComponent<GameManager>().LoseLevel();
        }
    }
}
