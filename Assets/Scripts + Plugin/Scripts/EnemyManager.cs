﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(EnemySensor))]

public class EnemyManager : TurnManager {

    public EnemyMover m_enemyMover;

    public EnemySensor m_enemySensor;
    public EnemySensor GetEnemySensor { get { return m_enemySensor; } }

    EnemyDeath m_enemyDeath;
    BoardManager m_board;

    PlayerManager m_player;

    public float delay;

    bool m_isDead = false;
    public bool isScared = false;
    public bool wasScared = false;
    public bool isOff; // private 

    public bool isDead { get { return m_isDead; } }

   
    
    public UnityEvent deathEvent;

    protected override void Awake() {

        base.Awake();
        
        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        m_enemyMover = GetComponent<EnemyMover>();
        m_enemySensor = GetComponent<EnemySensor>();
        m_enemyDeath = GetComponent<EnemyDeath>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();

        m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 0);
        m_enemyMover.EnemyAnimatorController.SetInteger("ChaserState", 0);
    }

    public void PlayTurn() {
        
        if (m_isDead || this.gameObject.activeSelf == false) {
            FinishTurn();
            return;
        }
        else if(this.gameObject.activeSelf == true)
        {
            Debug.Log("PLAY TURN");
            StartCoroutine(PlayTurnRoutine());
        }
            
        
    }

    IEnumerator PlayTurnRoutine() {
     
        if (m_gameManager != null && !m_gameManager.IsGameOver) {
            //detect player
            m_enemySensor.UpdateSensor(m_enemyMover.CurrentNode);

            yield return new WaitForSeconds(0f);

            if (m_enemySensor.FoundPlayer && isOff == false) {
                //attack player
                //notify the GM to lose the level
                //lr.transform.gameObject.SetActive(true);
                Debug.Log(this.name + "MORTE");
                m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 2);
                m_gameManager.LoseLevel();
                
            }
            else {
                // movement
                m_enemyMover.MoveOneTurn(); // --> finishMovementEvent.Invoke()
                if (m_enemySensor.FoundPlayer)
                {
                    m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 1);
                }


                

            }
        }        
    }


        
    public void Die() {

        if (gameObject!= null &&  m_enemyMover.movementType == MovementType.Stationary)
        {
            m_enemyMover.EnemyAnimatorController.SetInteger("StaticState", 3);
        }
        else if (gameObject != null && m_enemyMover.movementType == MovementType.Chaser)
        {
            m_enemyMover.EnemyAnimatorController.SetInteger("ChaserState", 4);
        }

        if (m_board.FindNodeAt(transform.position).isATrigger) {
            m_board.FindNodeAt(transform.position).UpdateTriggerToTrue();
            m_board.FindNodeAt(transform.position).triggerState = false;
        }

        if (m_isDead)
        {
            return;
        }
        m_isDead = true;


        StartCoroutine(WaitTimeForKill());

    }

    IEnumerator WaitTimeForKill()
    {
        yield return new WaitForSeconds(delay);
        
        if (deathEvent != null)
        {
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

    //(Old) pushEnemies with LightBulb
    #region

    //public void PushLeft() {
    //    if (m_player.hasLightBulb) {

    //        Node EnemyNode = m_board.FindNodeAt(transform.position);

    //        if (m_board.playerNode.transform.position.z == EnemyNode.transform.position.z && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x > EnemyNode.transform.position.x) {
    //            Debug.Log("MoveLeft");
    //            m_enemyMover.MoveLeft();

    //            m_enemyMover.destination = m_player.transform.position;
    //            m_enemyMover.FaceDestination();
    //            m_player.clearPlayerPath();
    //            m_enemyMover.firstChaserMove = false;
    //        }
    //    }
    //}

    //public void PushRight() {
    //    if (m_player.hasLightBulb) {

    //        Node EnemyNode = m_board.FindNodeAt(transform.position);

    //        if (m_board.playerNode.transform.position.z == EnemyNode.transform.position.z && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x < EnemyNode.transform.position.x) {
    //            Debug.Log("MoveRight");
    //            m_enemyMover.MoveRight();

    //            m_enemyMover.destination = m_player.transform.position;
    //            m_enemyMover.FaceDestination();
    //            m_player.clearPlayerPath();
    //            m_enemyMover.firstChaserMove = false;
    //        }
    //    }
    //}


    ////PUSH UP
    //public void PushUp() {
    //    if (m_player.hasLightBulb) {

    //        Node EnemyNode = m_board.FindNodeAt(transform.position);

    //        if (m_board.playerNode.transform.position.x == EnemyNode.transform.position.x && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z < EnemyNode.transform.position.z) {
    //            Debug.Log("MoveUp");
    //            m_enemyMover.MoveForward();

    //            m_enemyMover.destination = m_player.transform.position;
    //            m_enemyMover.FaceDestination();
    //            m_player.clearPlayerPath();
    //            m_enemyMover.firstChaserMove = false;
    //        }
    //    }
    //}



    ////PUSH DOWN
    //public void PushDown() {
    //    if (m_player.hasLightBulb) {

    //        Node EnemyNode = m_board.FindNodeAt(transform.position);

    //        if (m_board.playerNode.transform.position.x == EnemyNode.transform.position.x && Vector3.Distance(EnemyNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z > EnemyNode.transform.position.z) {
    //            Debug.Log("MoveDown");
    //            m_enemyMover.MoveBackward();

    //            m_enemyMover.destination = m_player.transform.position;
    //            m_enemyMover.FaceDestination();
    //            m_player.clearPlayerPath();
    //            m_enemyMover.firstChaserMove = false;

    //        }
    //    }
    //}

    #endregion



    public ItemData GetData() {
        ItemData itemData = new ItemData() {
            BoardPosition = transform.position,
            ItemType = ItemData.Type.Enemy,
        };
        return itemData;
    }
}
