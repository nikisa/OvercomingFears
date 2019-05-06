using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    Board m_board;

    protected Node m_currentNode;
    public Node CurrentNode { get { return m_currentNode; } }

    private void Awake() {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    private void Start() {
        m_currentNode = m_board.FindNodeAt(transform.position);
    }

    public Node FindSwordNode() {
        return m_board.FindNodeAt(transform.position);
    }


    //private void OnTriggerExit(Collider other) {
    //    Debug.Log("tua madre");
    //    if (other.gameObject.tag == "Enemy") {
    //        other.GetComponent<EnemyManager>().Die();
    //    }
    //}


    public void CaptureEnemies() {
        if (m_board != null) {
            List<EnemyManager> enemies = m_board.FindEnemiesAt(m_board.FindNodeAt(transform.position));
            if (enemies.Count != 0) {
                foreach (EnemyManager enemy in enemies) {
                    if (enemy != null) {
                        enemy.Die();
                    }
                }
            }
        }
    }


}
