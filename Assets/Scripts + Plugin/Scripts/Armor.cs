using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour {

    Board m_board;

    public int armorID;

    public bool isActive = false;

    public bool brokenSword = false;

    protected Node m_currentNode;

    public Node CurrentNode { get { return m_currentNode; } }

    private void Awake() {
        m_board = Object.FindObjectOfType<Board>().GetComponent<Board>();
    }

    private void Start() {
        m_currentNode = m_board.FindNodeAt(transform.position);
    }

    public int GetID() {
        return armorID;
    }

    public void ActivateSword() {
        isActive = !isActive;
        if (!brokenSword && isActive == true) {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        else if (isActive == false) {
            transform.GetChild(0).gameObject.SetActive(false);
            Debug.Log(transform.GetChild(0).gameObject.activeSelf + " --- " + transform.GetChild(0).gameObject.activeSelf);
        }

        m_board.CheckSword();
        
    }

    public void DeactivateSword() {
        isActive = !isActive;
        if (isActive == false) {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (isActive == true) {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void DestroySword() {
        brokenSword = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public Node FindArmorNode() {
        return m_board.FindNodeAt(transform.position);
    }

    public Node FindSwordNode() {
        
        return m_board.FindNodeAt(transform.position + (transform.forward * Board.spacing));
    }
}
