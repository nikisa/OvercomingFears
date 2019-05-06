using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour {

    public Vector3 offscreenOffset = new Vector3(0f, 10f, 0f);

    Board m_board;

    
    //public float deathDelay = 0f;
    //public float offscreenDelay = 100f;
    //public float iTweenDelay = 0f;
    //public iTween.EaseType easeType = iTween.EaseType.easeInOutQuint;
    //public float moveTime = 0.5f;

    void Awake() {
        m_board = GetComponent<Board>();
    }


    //public void MoveOffBoard(Vector3 target) {
    //    iTween.MoveTo(gameObject, iTween.Hash(
    //        "x", target.x,
    //        "y", target.y,
    //        "z", target.z,
    //        "delay", iTweenDelay,
    //        "easetype", easeType,
    //        "time", moveTime));
    //}

    public void Die() {
        Destroy(gameObject);
        //Debug.Log("+1 kill");
    }

    //IEnumerator DieRoutine() {
    //    yield return new WaitForSeconds(deathDelay);

    //    Vector3 offscreenPos = transform.position +offscreenOffset;

    //    MoveOffBoard(offscreenPos);

    //    yield return new WaitForSeconds(moveItem + offscreenDelay);

    //    if (m_board.capturePositions.Count != 0 && m_board.CurrentCapturePosition < m_board.capturePositions.Count) {
    //        Vector3 capturePos = m_board.capturePositions[m_board.CurrentCapturePosition].position;
    //        transform.position = capturePos + offscreenOffset;

    //        MoveOffBoard(capturePos);

    //        yield return new WaitForSeconds(moveTime);

    //        m_board.CurrentCapturePosition++;
    //        m_boardCurrentCapturePosition = Mathf.Clamp(m_board.CurrentCapturePosition, 0, m_board.capturePositions.Count -1);
    //    }
    //}

}
