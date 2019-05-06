using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : Mover {

    public bool inScene = true;
    public bool hasMoved = false;
    public bool hasStopped = false;

    public bool upBlocked = false;
     public bool downBlocked = false;
     public bool leftBlocked = false;
     public bool rightBlocked = false;


    private Node m_previousMovableObjectNode;
    public Node PreviousMovableObjectNode { get { return m_previousMovableObjectNode; } set { m_previousMovableObjectNode = FindMovableObjectNode(); } }


    public Node FindMovableObjectNode() {
        return m_board.FindNodeAt(transform.position);
    }

    public void UpdateTriggerToFalse() {
        PreviousMovableObjectNode.triggerState = false;
    }

    public void SetPreviousMovableObjectNode(Node n) {
        PreviousMovableObjectNode = n;
    }

    public Node GetPreviousMovableObjectNode() {
        return PreviousMovableObjectNode;
    }


    public void PushRight() {
        if (m_board != null || inScene) {
            Node movableObjectNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.z == movableObjectNode.transform.position.z && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x < movableObjectNode.transform.position.x && m_board.playerNode.LinkedNodes.Contains(movableObjectNode)) {
                m_player.transform.GetChild(2).gameObject.SetActive(false);
                m_player.hasLightBulb = false;
                this.MoveRight();
                hasMoved = true;
                hasStopped = false;
            }
        }
    }

    public void PushLeft() {
        if (m_board != null || inScene) {
            Node movableObjectNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.z == movableObjectNode.transform.position.z && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) <= 2f && m_board.playerNode.transform.position.x > movableObjectNode.transform.position.x && m_board.playerNode.LinkedNodes.Contains(movableObjectNode)) {
                m_player.transform.GetChild(2).gameObject.SetActive(false);
                m_player.hasLightBulb = false;
                this.MoveLeft();
                hasMoved = true;
                hasStopped = false;
            }
        }
    }

    public void PushForward() {
        if (m_board != null || inScene) {
            Node movableObjectNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.x == movableObjectNode.transform.position.x && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z < movableObjectNode.transform.position.z && m_board.playerNode.LinkedNodes.Contains(movableObjectNode)) {
                m_player.transform.GetChild(2).gameObject.SetActive(false);
                m_player.hasLightBulb = false;
                this.MoveForward();
                hasMoved = true;
                hasStopped = false;
            }
        }
    }

    public void PushBackward() {
        if (m_board != null || inScene) {
            Node movableObjectNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.x == movableObjectNode.transform.position.x && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z > movableObjectNode.transform.position.z && m_board.playerNode.LinkedNodes.Contains(movableObjectNode)) {
                m_player.transform.GetChild(2).gameObject.SetActive(false);
                m_player.hasLightBulb = false;
                this.MoveBackward();
                hasMoved = true;
                hasStopped = false;
            }
        }  
    }

    //PUSH___________________________________________

    public void PullLeft() {

        if (m_board != null || inScene){
            Node movableObjectNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.z == movableObjectNode.transform.position.z && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x < movableObjectNode.transform.position.x) {
                m_player.transform.GetChild(2).gameObject.SetActive(false);
                m_player.hasLightBulb = false;
                this.MoveLeft();
                hasMoved = true;
                hasStopped = false;
            }
        }
    }

    public void PullRight() {

        if (m_board != null || inScene) {
            Node movableObjectNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.z == movableObjectNode.transform.position.z && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.x > movableObjectNode.transform.position.x) {
                m_player.transform.GetChild(2).gameObject.SetActive(false);
                m_player.hasLightBulb = false;
                this.MoveRight();
                hasMoved = true;
                hasStopped = false;
            }
        }
    }

    public void PullBackward() {

        if (m_board != null || inScene) {
            Node movableObjectNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.x == movableObjectNode.transform.position.x && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z < movableObjectNode.transform.position.z) {
                m_player.transform.GetChild(2).gameObject.SetActive(false);
                m_player.hasLightBulb = false;
                this.MoveBackward();
                hasMoved = true;
                hasStopped = false;
            }
        }
    }

    public void PullForward() {

        if (m_board != null || inScene) {
            Node movableObjectNode = m_board.FindNodeAt(transform.position);

            if (m_board.playerNode.transform.position.x == movableObjectNode.transform.position.x && Vector3.Distance(movableObjectNode.transform.position, m_board.playerNode.transform.position) < 3f && m_board.playerNode.transform.position.z > movableObjectNode.transform.position.z) {
                m_player.transform.GetChild(2).gameObject.SetActive(false);
                m_player.hasLightBulb = false;
                this.MoveForward();
                hasMoved = true;
                hasStopped = false;
            }
        }
    }
    
    //PULL___________________________________________


    public void checkNodeForObstacle() { //Restituisce 2 se la via è libera , altrimenti 1

        if (inScene) {

            leftBlocked = false;
            rightBlocked = false;
            downBlocked = false;
            upBlocked = false;


            Node nextMovableObjectNodeLeft = m_board.FindNodeAt(transform.position + new Vector3(-2f, 0, 0));
            Node nextMovableObjectNodeRight = m_board.FindNodeAt(transform.position + new Vector3(2f, 0, 0));
            Node nextMovableObjectNodeUp = m_board.FindNodeAt(transform.position + new Vector3(0, 0, 2f));
            Node nextMovableObjectNodeDown = m_board.FindNodeAt(transform.position + new Vector3(0, 0, -2f));

            if ((nextMovableObjectNodeLeft == null || m_board.FindMovableObjectsAt(nextMovableObjectNodeLeft).Count != 0 || m_board.FindEnemiesAt(nextMovableObjectNodeLeft).Count != 0 || !m_board.FindNodeAt(this.transform.position).LinkedNodes.Contains(nextMovableObjectNodeLeft) || (nextMovableObjectNodeLeft.isAGate && nextMovableObjectNodeLeft.GetGateState() == false)) && m_board.playerNode.transform.position == transform.position + new Vector3(2f, 0, 0)) {
                leftBlocked = true;
            }

            if ((nextMovableObjectNodeRight == null || m_board.FindMovableObjectsAt(nextMovableObjectNodeRight).Count != 0 || m_board.FindEnemiesAt(nextMovableObjectNodeRight).Count != 0 || !m_board.FindNodeAt(this.transform.position).LinkedNodes.Contains(nextMovableObjectNodeRight) || (nextMovableObjectNodeRight.isAGate && nextMovableObjectNodeRight.GetGateState() == false)) && m_board.playerNode.transform.position == transform.position + new Vector3(-2f, 0, 0)) {
                rightBlocked = true;
            }

            if ((nextMovableObjectNodeUp == null || m_board.FindMovableObjectsAt(nextMovableObjectNodeUp).Count != 0 || m_board.FindEnemiesAt(nextMovableObjectNodeUp).Count != 0 || !m_board.FindNodeAt(this.transform.position).LinkedNodes.Contains(nextMovableObjectNodeUp) || (nextMovableObjectNodeUp.isAGate && nextMovableObjectNodeUp.GetGateState() == false)) && m_board.playerNode.transform.position == transform.position + new Vector3(0, 0, -2f)) {
                upBlocked = true;
            }

            if ((nextMovableObjectNodeDown == null || m_board.FindMovableObjectsAt(nextMovableObjectNodeDown).Count != 0 || m_board.FindEnemiesAt(nextMovableObjectNodeDown).Count != 0 || !m_board.FindNodeAt(this.transform.position).LinkedNodes.Contains(nextMovableObjectNodeDown) || (nextMovableObjectNodeDown.isAGate && nextMovableObjectNodeDown.GetGateState() == false)) && m_board.playerNode.transform.position == transform.position + new Vector3(0, 0, 2f)) {
                downBlocked = true;
            }
        }
    }

    public ItemData GetData() {
        ItemData itemData = new ItemData() {
            BoardPosition = transform.position,
            ItemType = ItemData.Type.MovableObject,
        };
        return itemData;
    }
}
