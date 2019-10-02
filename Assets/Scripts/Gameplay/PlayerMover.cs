using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : Mover {

    public float playerMovementDelay = 0.1f;

    PlayerCompass m_playerCompass;

    protected override void Awake() {
        base.Awake();
        m_playerCompass = Object.FindObjectOfType<PlayerCompass>().GetComponent<PlayerCompass>();
    }
    

    public void Setup() {
        UpdateCurrentNode();
        UpdateBoard();
    }


    void UpdateBoard() {
        if (m_board != null) {
            m_board.UpdatePlayerNode();
        }
    }

    protected override IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime) {
    
        if (m_playerCompass != null) {
            m_playerCompass.ShowArrows(false);
        }

        m_player.PlayerAnimatorController.SetInteger("PlayerState" , 10);

        //run the parent class MoveRoutine
        yield return StartCoroutine(base.MoveRoutine(destinationPos, playerMovementDelay));

        UpdateBoard();

        if (m_playerCompass != null) {
            m_playerCompass.ShowArrows(true);
        }

        base.finishMovementEvent.Invoke();

    }

}
