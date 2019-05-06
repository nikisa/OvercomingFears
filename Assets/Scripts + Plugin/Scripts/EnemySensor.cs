using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySensor : MonoBehaviour {

    public Vector3 directionToSearch = new Vector3(0f, 0f, Board.spacing);

    Node m_nodeToSearch;
    Board m_board;
    
    Node m_previousEnemyNode;

    public Node PreviousEnemyNode { get { return m_previousEnemyNode; } set { m_previousEnemyNode = FindEnemyNode(); } }

    bool m_foundPlayer = false;
    public bool FoundPlayer { get { return m_foundPlayer; } }

	void Awake() {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
	}
	
    public void UpdateSensor(Node enemyNode) {
        Vector3 worldSpacePositionToSearch = transform.TransformVector(directionToSearch) + transform.position;

        if (m_board != null) {
            m_nodeToSearch = m_board.FindNodeAt(worldSpacePositionToSearch);

            if (!enemyNode.LinkedNodes.Contains(m_nodeToSearch)) {
                m_foundPlayer = false;
                return;
            }

            if (m_nodeToSearch == m_board.playerNode) {
                m_foundPlayer = true;
            }
        }
    }

    public Node FindEnemyNode() {
        return m_board.FindNodeAt(transform.position);
    }


    public void SetPreviousEnemyNode(Node n) {
        PreviousEnemyNode = n;
    }

    public Node GetPreviousEnemyNode() {
        return PreviousEnemyNode;
    }

    public void UpdateTriggerToFalse() {
        PreviousEnemyNode.triggerState = false;
    }
}
