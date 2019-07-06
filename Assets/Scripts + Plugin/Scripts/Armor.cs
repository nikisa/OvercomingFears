using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour {

    public Material[] materials;

    public Animator AnimatorController;

    BoardManager m_board;

    public int armorID;

    public bool isActive = false;

    public bool brokenSword = false;

    protected Node m_currentNode;

    public Node CurrentNode { get { return m_currentNode; } }

    private void Awake() {
        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        

        if (armorID <= 6 && armorID >= 1) {
            transform.GetChild(2).gameObject.GetComponent<Renderer>().material = materials[armorID - 1];
        }
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
            transform.GetChild(3).gameObject.SetActive(true);
            //Debug.Log("Sword at node " + m_board.FindNodeAt(transform.GetChild(3).transform.position) + " activated");
            this.AnimatorController.SetBool("ArmorState" , isActive);
        }
        else if (isActive == false) {
            transform.GetChild(3).gameObject.SetActive(false);
            this.AnimatorController.SetBool("ArmorState", isActive);
        }
        //m_board.CheckSword();
    }

    public void DeactivateSword() {
        isActive = !isActive;
        if (isActive == false) {
            transform.GetChild(3).gameObject.SetActive(false);
            this.AnimatorController.SetBool("ArmorState", false);
        }
        else if (isActive == true) {
            transform.GetChild(3).gameObject.SetActive(true);
            this.AnimatorController.SetBool("ArmorState", true);
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
        
        return m_board.FindNodeAt(transform.position + (transform.forward * BoardManager.spacing));
    }
}
