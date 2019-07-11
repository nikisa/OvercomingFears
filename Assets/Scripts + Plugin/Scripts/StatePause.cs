using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatePause : StateBehaviourBase
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>().playerInput.InputEnabled = false;
        Cursor.visible = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (Input.GetKeyDown(KeyCode.Joystick1Button1)) {
            Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>().playerInput.InputEnabled = true;
            Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>().PauseCanvas.transform.GetChild(0).gameObject.SetActive(false);
            GameManager.stateGameplay();
        }
        else if (Input.GetKeyDown(KeyCode.Joystick1Button2)) {
            Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>().PauseCanvas.transform.GetChild(0).gameObject.SetActive(false);
            GameManager.stateMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Joystick1Button3)) {
            GameManager.Instance.LoseLevel();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>().playerInput.InputEnabled = true;
        Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>().PauseCanvas.transform.GetChild(0).gameObject.SetActive(false);
    }

}
