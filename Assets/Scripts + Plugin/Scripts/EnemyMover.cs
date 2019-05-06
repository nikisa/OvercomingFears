using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// DUE ENEMIES COMPENETRANO TRA LORO SE UNO VIENE SPINTO NELLA TRAIETTORIA DELL'ALTRO
/// </summary>


public enum MovementType {
    Stationary,
    Patrol,
    Spinner,
    Chaser
}

public class EnemyMover : Mover {
    
    public Vector3 directionToMove = new Vector3(0f , 0f , Board.spacing);

    public MovementType firstMovementType = MovementType.Stationary;
    public MovementType movementType = MovementType.Stationary;

    
    public float standTime = 0f;

    PlayerManager m_player;

    [HideInInspector]public Vector3 firstDest;
    [HideInInspector]public Vector3 spottedDest;

   [HideInInspector] public static int index;

    [HideInInspector] public bool firstChaserMove = false;

	protected override void Awake() {
        base.Awake();
        faceDestination = true;
        movementType = firstMovementType;
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

    protected override void Start(){
        base.Start();
        
    }

    public void MoveOneTurn() {
        
        switch (movementType) {
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Stationary:
                Stand();
                break;
            case MovementType.Spinner:
                Spin();
                break;
            case MovementType.Chaser:
                Chase();
                break;
        }
    }

    void Chase() {
        StartCoroutine(ChaseRoutine());
    }

    IEnumerator ChaseRoutine() {

        Vector3 startPos = new Vector3(m_currentNode.Coordinate.x, 0f, m_currentNode.Coordinate.y);

        firstDest = startPos + transform.TransformVector(directionToMove);

        spottedDest = startPos + transform.TransformVector(directionToMove * 2f);

        if (m_board.playerNode == m_board.FindNodeAt(spottedDest) && !m_player.spottedPlayer && m_board.FindNodeAt(firstDest).LinkedNodes.Contains(m_board.FindNodeAt(spottedDest))) {

            Debug.Log("Spotted!");
            
            m_board.ChasingPreviousPlayerNode = m_board.playerNode; //Cambiare PreviousPlayerNode , qui o su Board
            //Move(firstDest , 0f);
            m_player.spottedPlayer = true;

            m_player.UpdatePlayerPath();

        }

        else if (m_player.spottedPlayer) {

            m_player.UpdatePlayerPath();

            if (firstChaserMove == false) { // && CASELLA SUCCESSIVA NON è OCCUPATA (post armature)
                Move(firstDest, 0f);
                firstChaserMove = true;
            }
            else { // && CASELLA SUCCESSIVA NON è OCCUPATA (post armature)
                Debug.Log("Chasing...");

                //m_board.ChaserNewDest = m_board.ChasingPreviousPlayerNode;
                //m_board.ChasingPreviousPlayerNode = m_board.playerNode;

                //Debug.Log(m_board.ChasingPreviousPlayerNode);

                if (!m_player.hasLightBulb || m_player.transform.position != firstDest) {

                    Debug.Log(m_player.GetPlayerPath(index));

                    Move(m_player.GetPlayerPath(index).transform.position, 0f);

                    yield return new WaitForSeconds(0.6f);

                    destination = m_player.GetPlayerPath(index + 1).transform.position;
                    FaceDestination();
                    
                    
                }
                
                index++;
            }
        }

         
        

        while (isMoving) {
            yield return null;
        }

        base.finishMovementEvent.Invoke();
    }



    
    void Patrol() {
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine() {
        Vector3 startPos = new Vector3(m_currentNode.Coordinate.x, 0f, m_currentNode.Coordinate.y);

        //One space forward
        Vector3 newDest = startPos + transform.TransformVector(directionToMove);

        //Two space forward
        Vector3 nextDest = startPos + transform.TransformVector(directionToMove * 2f);

        //COntrollare il movimento
        

        

        if (m_board != null) {
            Node newDestNode = m_board.FindNodeAt(newDest);
            Node nextDestNode = m_board.FindNodeAt(nextDest);

            if (newDestNode == null || newDestNode.LinkedNodes.Contains(newDestNode) || m_board.FindMovableObjectsAt(newDestNode).Count != 0 || m_board.FindSwordsAt(newDestNode).Count != 0 || (m_board.playerNode == newDestNode && m_player.spottedPlayer)) {

               

                Spin();

                yield return new WaitForSeconds(rotateTime);
            }
            else {
                Move(newDest, 0f);

                while (isMoving) {
                    yield return null;
                }
            }


            if (nextDestNode == null || nextDestNode.LinkedNodes.Contains(nextDestNode) || m_board.FindMovableObjectsAt(nextDestNode).Count != 0 || m_board.FindSwordsAt(nextDestNode).Count != 0 || (m_board.playerNode == nextDestNode && m_player.spottedPlayer)) {

                    //SPOSTARE MOVIMENTO QUI DENTRO ALTRIMENTI SI BLOCCA NELL ANGOLINO E NON SI GIRA

                    destination = startPos;
                    FaceDestination();

                    yield return new WaitForSeconds(rotateTime);
                }

            base.finishMovementEvent.Invoke();
            }
        }
    
    void Stand() {
        StartCoroutine(StandRoutine());
    }

    IEnumerator StandRoutine() {
        yield return new WaitForSeconds(standTime);
        base.finishMovementEvent.Invoke();
    }

    public void Spin() {
        StartCoroutine(SpinRoutine());
    }
    IEnumerator SpinRoutine() {
        Vector3 localForward = new Vector3(0f, 0f , Board.spacing);
        destination = transform.TransformVector(localForward * -1f) + transform.position;

        FaceDestination();

        yield return new WaitForSeconds(rotateTime);

        base.finishMovementEvent.Invoke();
    }

}
    
