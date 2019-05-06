using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemySensor))]

public class EnemyManager : TurnManager {

    public EnemyMover m_enemyMover;

    EnemySensor m_enemySensor;
    public EnemySensor GetEnemySensor { get { return m_enemySensor; } }

    EnemyDeath m_enemyDeath;
    Board m_board;

    PlayerManager m_player;

    

    bool m_isDead = false;
    public bool isScared = false;
    public bool wasScared = false;
    public bool isOff; // private 

    public bool isDead { get { return m_isDead; } }

    public UnityEvent deathEvent;

    protected override void Awake() {

        base.Awake();

        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
        m_enemyMover = GetComponent<EnemyMover>();
        m_enemySensor = GetComponent<EnemySensor>();
        m_enemyDeath = GetComponent<EnemyDeath>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

    public void PlayTurn() {
        
        if (m_isDead || isScared || isOff) {
            FinishTurn();
            return;
        }
        
        StartCoroutine(PlayTurnRoutine());
    }

    IEnumerator PlayTurnRoutine() {
     
        if (m_gameManager != null && !m_gameManager.IsGameOver) {
            //detect player
            m_enemySensor.UpdateSensor(m_enemyMover.CurrentNode);

            yield return new WaitForSeconds(0f);

            if (m_enemySensor.FoundPlayer && isScared == false) {
                //attack player
                //notify the GM to lose the level
                
                m_gameManager.LoseLevel();
            }
            else {
                // movement
                m_enemyMover.MoveOneTurn(); // --> finishMovementEvent.Invoke()
                
            }
        }        
    }


        
    public void Die() {

        if (m_board.FindNodeAt(transform.position).isATrigger) {
            m_board.FindNodeAt(transform.position).UpdateTriggerToTrue(); //RISCHIO
            m_board.FindNodeAt(transform.position).triggerState = false;
        }


        if (m_isDead) {
            return;
        }
        m_isDead = true;
        

        if (deathEvent != null) {
            deathEvent.Invoke();
        }
    }

    public void SetMovementType(MovementType mt)
    {
        m_enemyMover.movementType = mt; 
    }

    public MovementType GetMovementType()
    {
        return m_enemyMover.movementType;
    }

    public MovementType GetFirstMovementType()
    {
        return m_enemyMover.firstMovementType;
    }


    public void PushLeft() {
        if (m_player.hasLightBulb) {
            
            Node EnemyNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.z == EnemyNode.transform.position.z && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x > EnemyNode.transform.position.x) {
                Debug.Log("MoveLeft");
                m_enemyMover.MoveLeft();

                m_enemyMover.destination = m_player.transform.position;
                m_enemyMover.FaceDestination();
                m_player.clearPlayerPath();
                m_enemyMover.firstChaserMove = false;
            }
        }
    }

    public void PushRight() {
        if (m_player.hasLightBulb) {

            Node EnemyNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.z == EnemyNode.transform.position.z && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x < EnemyNode.transform.position.x) {
                Debug.Log("MoveRight");
                m_enemyMover.MoveRight();

                m_enemyMover.destination = m_player.transform.position;
                m_enemyMover.FaceDestination();
                m_player.clearPlayerPath();
                m_enemyMover.firstChaserMove = false;
            }
        }
    }


    //PUSH UP
    public void PushUp() {
        if (m_player.hasLightBulb) {

            Node EnemyNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.x == EnemyNode.transform.position.x && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z < EnemyNode.transform.position.z) {
                Debug.Log("MoveUp");
                m_enemyMover.MoveForward();

                m_enemyMover.destination = m_player.transform.position;
                m_enemyMover.FaceDestination();
                m_player.clearPlayerPath();
                m_enemyMover.firstChaserMove = false;
            }
        }
    }



    //PUSH DOWN
    public void PushDown() {
        if (m_player.hasLightBulb) {

            Node EnemyNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.x == EnemyNode.transform.position.x && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z > EnemyNode.transform.position.z) {
                Debug.Log("MoveDown");
                m_enemyMover.MoveBackward();

                m_enemyMover.destination = m_player.transform.position;
                m_enemyMover.FaceDestination();
                m_player.clearPlayerPath();
                m_enemyMover.firstChaserMove = false;

            }
        }
    }



    public ItemData GetData() {
        ItemData itemData = new ItemData() {
            BoardPosition = transform.position,
            ItemType = ItemData.Type.Enemy,
        };
        return itemData;
    }
}
