using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour {


    public Vector3 destination;

    public bool faceDestination = false;

    public bool isMoving = false;
    public iTween.EaseType movementEaseType = iTween.EaseType.easeInOutExpo;
    public iTween.EaseType rotationEaseType = iTween.EaseType.easeInOutExpo;

    [HideInInspector]
    public float moveTime = .5f;

    [HideInInspector]
    public float rotateTime = 0.5f;

    public float iTweenDelay = 0f;

    protected BoardManager m_board;

    protected Node m_currentNode;

    public Node CurrentNode { get { return m_currentNode; } }

    public UnityEvent finishMovementEvent;

    protected PlayerManager m_player;

    private MovableObject MO;



    // Use this for initialization
    protected virtual void Awake() {
        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        m_player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
    }

    protected virtual void Start() {
        //UpdateCurrentNode();
    }

    IEnumerator ShadowCollision()
    {
        yield return new WaitForSeconds(rotateTime);
        m_player.PlayerAnimatorController.SetInteger("PlayerState", 9);
        yield return new WaitForSeconds(0.23f);
        m_player.PlayerAnimatorController.SetInteger("PlayerState", 0);
    }

    public void Move(Vector3 destinationPos, float delayTime = 0f) {
        
        if (isMoving) {
            return;
        }

        if (m_board != null) {
            Node targetNode = m_board.FindNodeAt(destinationPos);

            if (this.name != "Boss")
            {
                if (targetNode != null && m_currentNode != null && m_currentNode.LinkedNodes.Contains(targetNode))
                {
                    if (((targetNode.isAGate && targetNode.GetGateState() == true) || !targetNode.isAGate) && (m_board.FindMovableObjectsAt(targetNode).Count == 0 || (m_board.FindMovableObjectsAt(targetNode).Count > 0 && m_board.FindMovableObjectsAt(targetNode)[0].isMoving)))
                    {
                        StartCoroutine(MoveRoutine(destinationPos, delayTime));
                    }
                    else
                    {
                            //rotate
                            
                        if (tag == "Player")
                        {
                            FaceDestination(targetNode.transform.position);
                            StartCoroutine(ShadowCollision());
                        }
                            //play animation
                    }

                }
                else
                {
                    //Debug.Log("CURRENT NODE NOT CONNECTED");



                }
            }
            else
            {
                if (targetNode != null && m_currentNode != null)
                {
                    if ((targetNode.isAGate && targetNode.GetGateState() == true) || !targetNode.isAGate)
                    {
                        //Debug.Log(this.name + " Move test");
                        StartCoroutine(MoveRoutine(destinationPos, delayTime));
                    }

                }
                else
                {
                    //Debug.Log("CURRENT NODE NOT CONNECTED");
                    
                }
            }

            
        }
    }

    public void Jump(Vector3 destinationPos, float delayTime = 0.25f) {

        //if (isMoving) {
        //    return;
        //}
        if (m_board != null) {
            Node targetNode = m_board.FindNodeAt(destinationPos);
            if (targetNode != null) {
                    StartCoroutine(MoveRoutine(destinationPos, delayTime));
            }
            else {
                //Debug.Log("DEAD");
                StartCoroutine(MoveRoutine(destinationPos, delayTime));
            }
        }
    }



    protected virtual IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime) {

        isMoving = true;
        destination = destinationPos;
        
        if (faceDestination) {
            FaceDestination();
            yield return new WaitForSeconds(0);
        }

        //turn to face destination

        yield return new WaitForSeconds(delayTime);
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", destinationPos.x,
            "y", destinationPos.y,
            "z", destinationPos.z,
            "delay", iTweenDelay,
            "easetype", movementEaseType,
            "time", moveTime
        ));

        while (Vector3.Distance(destinationPos, transform.position) > 0.01f) {
            yield return null;
        }

        iTween.Stop(gameObject);
        transform.position = destinationPos;
        isMoving = false;

        UpdateCurrentNode();


    }

    public void MoveLeft() {
        Vector3 newPosition = transform.position + new Vector3(-BoardManager.spacing, 0, 0);
        Move(newPosition, 0);
    }

    public void MoveRight() {
        Vector3 newPosition = transform.position + new Vector3(BoardManager.spacing, 0, 0);
        Move(newPosition, 0);
    }

    public void MoveForward() {
        Vector3 newPosition = transform.position + new Vector3(0, 0, BoardManager.spacing);
        Move(newPosition, 0);
    }

    public void MoveBackward() {
        Vector3 newPosition = transform.position + new Vector3(0, 0, -BoardManager.spacing);
        Move(newPosition, 0);
    }


    public void UpdateCurrentNode() {
        m_currentNode = m_board.FindNodeAt(transform.position);
    }

    public void FaceDestination() {
        Vector3 relativePosition = destination - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        float newY = newRotation.eulerAngles.y;

        iTween.RotateTo(gameObject, iTween.Hash(
            "y", newY,
            "delay", 0f,
            "easetype", rotationEaseType,
            "time", rotateTime
            ));
    }

    public void FaceDestination(Vector3 _destination) {
        Vector3 relativePosition = _destination - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        float newY = newRotation.eulerAngles.y;

        iTween.RotateTo(gameObject, iTween.Hash(
            "y", newY,
            "delay", 0f,
            "easetype", rotationEaseType,
            "time", rotateTime
            ));
    }
}