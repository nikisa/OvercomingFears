using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingWall : MonoBehaviour {

    /// <summary>
    /// FUNZIONA TUTTO MA ESEGUE LA PUSH 2 VOLTE (STESSO PROBLEMA DELLA TRAPPOLA CHE SPARAVA PIù PROIETTILI)
    /// BISOGNA CORREGGERLO ALTRIMENTI SE DOPO ESSERE STATO SPINTO VAI SUL TARGET NODE DI UN ALTRO PUSHINGWALL VIENI SPINTO 2 VOLTE IN UN TURNO (2 VOLTE PERCHé DA 2 PUSHINGWALLS DIVERSI)
    /// </summary>

    public int pushingWallID;
    Board m_board;
    Node TargetNode , PushedByNode;    
    PlayerMover m_player;

    void Awake() {
        m_board = Object.FindObjectOfType<Board>();
        m_player = Object.FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();
    }

    void Start() {
        //TargetNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(2, 0, 0));
        //PushedByNode = m_board.FindNodeAt(transform.GetChild(0).transform.position);
        //Debug.Log(transform.position);
        //Debug.Log(transform.GetChild(0).transform.position);
    }

    public int GetID() {
        return this.pushingWallID;
    }

    public void Push() {
        Debug.Log("PUSH");
        if (transform.position.x > transform.GetChild(0).transform.position.x) {

            TargetNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(2, 0, 0));
            PushedByNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(4, 0, 0));

            foreach (var movableObject in m_board.FindMovableObjectsAt(TargetNode)) {
                movableObject.Jump(PushedByNode.transform.position);
            }

            foreach (var enemy in m_board.FindEnemiesAt(TargetNode)) {
                enemy.m_enemyMover.Jump(PushedByNode.transform.position);
            }

            if (m_board.playerNode == TargetNode) {
                m_player.Jump(PushedByNode.transform.position);
            }


        }
        else if (transform.position.x < transform.GetChild(0).transform.position.x) {


            TargetNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(-2, 0, 0));
            PushedByNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(-4, 0, 0));

            foreach (var movableObject in m_board.FindMovableObjectsAt(TargetNode)) {
                movableObject.Jump(PushedByNode.transform.position);
            }

            foreach (var enemy in m_board.FindEnemiesAt(TargetNode)) {
                enemy.m_enemyMover.Jump(PushedByNode.transform.position);
            }

            if (m_board.playerNode == TargetNode) {
                m_player.Jump(PushedByNode.transform.position);
            }
        }
        else if (transform.position.z < transform.GetChild(0).transform.position.z) {

            TargetNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(0, 0, -2));
            PushedByNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(0, 0, -4));

            foreach (var movableObject in m_board.FindMovableObjectsAt(TargetNode)) {
                movableObject.Jump(PushedByNode.transform.position);
            }

            foreach (var enemy in m_board.FindEnemiesAt(TargetNode)) {
                enemy.m_enemyMover.Jump(PushedByNode.transform.position);
            }

            if (m_board.playerNode == TargetNode) {
                m_player.Jump(PushedByNode.transform.position);
            }
        }
        else if (transform.position.z > transform.GetChild(0).transform.position.z) {

            TargetNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(0, 0, 2));
            PushedByNode = m_board.FindNodeAt(transform.GetChild(0).transform.position + new Vector3(0, 0, 4));

            foreach (var movableObject in m_board.FindMovableObjectsAt(TargetNode)) {
                movableObject.Jump(PushedByNode.transform.position);
            }

            foreach (var enemy in m_board.FindEnemiesAt(TargetNode)) {
                enemy.m_enemyMover.Jump(PushedByNode.transform.position);
            }

            if (m_board.playerNode == TargetNode) {
                m_player.Jump(PushedByNode.transform.position);
            }
        }
    }
}
