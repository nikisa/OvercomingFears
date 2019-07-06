using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    BoardManager m_board;

    public float WaitTimeSwordKill = 0.5f;

    protected Node m_currentNode;
    public Node CurrentNode { get { return m_currentNode; } }

    private void Awake() {
        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
    }

    private void Start() {
        m_currentNode = m_board.FindNodeAt(transform.position);
    }

    public Node FindSwordNode() {
        return m_board.FindNodeAt(transform.position);
    }


    //public void CaptureEnemies() {
    //    if (m_board != null) {
    //        List<EnemyManager> enemies = m_board.FindEnemiesAt(m_board.FindNodeAt(transform.position));
    //        //Debug.Log(m_board.FindNodeAt(transform.position));

    //        //Enemies rimane vuoto

    //        if (enemies.Count != 0 ) {

    //            foreach (EnemyManager enemy in enemies) {
    //                if (enemy != null) {
    //                    enemy.Die();
    //                }
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// PATCH MOMENTANEA
    /// </summary>

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy") {

            foreach (EnemyManager enemy in m_board.m_gm.m_enemies) {
                enemy.delayedEmptyTurn = .3f;
            }

            if (m_board.FindNodeAt(other.transform.position).isATrigger)
            {
                StartCoroutine(WaitTriggerToFalse(other));
            }
            StartCoroutine(SwordKillWaitTime(other));            
        }
    }

    IEnumerator SwordKillWaitTime(Collider other) {
        yield return new WaitForSeconds(WaitTimeSwordKill);
        other.GetComponent<EnemyManager>().Die();

        foreach (Node trigger in m_board.TriggerNodes) {
            if (trigger != null && m_board.FindNodeAt(other.transform.position).GetTriggerId(m_board.FindNodeAt(other.transform.position)) == m_board.FindNodeAt(other.transform.position).GetTriggerId(trigger)) {
                trigger.triggerTemp.GetComponent<TriggerRotation>().StopTriggerRotation(false);
            }
        }
    }

    IEnumerator WaitTriggerToFalse(Collider other) {
        yield return new WaitForSeconds(WaitTimeSwordKill + .65f);
        m_board.UpdateTriggerToFalseLevel3(m_board.FindNodeAt(other.transform.position));
    }


}
