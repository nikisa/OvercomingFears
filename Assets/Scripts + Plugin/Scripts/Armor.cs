using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour {

    [HideInInspector]
    public Animator AnimatorController;

    BoardManager m_board;

    public int armorID;

    public bool isActive = false;

    public bool brokenSword = false;

    protected Node m_currentNode;

    public Node CurrentNode { get { return m_currentNode; } }

    private void Awake() {
        m_board = Object.FindObjectOfType<GameManager>().GetComponent<BoardManager>();
        AnimatorController = FindObjectOfType<Animator>().GetComponent<Animator>();
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
            AnimatorController.ResetTrigger("Up");
            AnimatorController.SetTrigger("Down");
            AnimatorController.Play("ArmorDOWN");

        }

        else if (isActive == false) {
            transform.GetChild(3).gameObject.SetActive(false);
            AnimatorController.ResetTrigger("Down");
            AnimatorController.SetTrigger("Up");
            AnimatorController.Play("ArmorUP");
        }

        m_board.CheckSword();
        
    }

    public void DeactivateSword() {
        isActive = !isActive;
        if (isActive == false) {
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else if (isActive == true) {
            transform.GetChild(3).gameObject.SetActive(true);
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
