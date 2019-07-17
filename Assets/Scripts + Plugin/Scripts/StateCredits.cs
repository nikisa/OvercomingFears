using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StateCredits : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().ChangeMenu(MenuType.Credits);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (Input.anyKeyDown ||
            Input.GetKeyDown(KeyCode.Joystick1Button1) ||
            Input.GetKeyDown(KeyCode.Joystick1Button2) ||
            Input.GetKeyDown(KeyCode.Joystick1Button3) ||
            Input.GetKeyDown(KeyCode.Joystick1Button4) ||
            Input.GetKeyDown(KeyCode.Joystick1Button5) ||
            Input.GetKeyDown(KeyCode.Joystick1Button6) ||
            Input.GetKeyDown(KeyCode.Joystick1Button7) ||
            Input.GetKeyDown(KeyCode.Joystick1Button8) ||
            Input.GetKeyDown(KeyCode.Joystick1Button9) ||
            Input.GetKeyDown(KeyCode.Joystick1Button10) ||
            Input.GetKeyDown(KeyCode.Joystick1Button11) ||
            Input.GetKeyDown(KeyCode.Joystick1Button12) ||
            Input.GetKeyDown(KeyCode.Joystick1Button13) ||
            Input.GetKeyDown(KeyCode.Joystick1Button14) ||
            Input.GetKeyDown(KeyCode.Joystick1Button15) ||
            Input.GetKeyDown(KeyCode.Joystick1Button16) ||
            Input.GetKeyDown(KeyCode.Joystick1Button17) ||
            Input.GetKeyDown(KeyCode.Joystick1Button18) ||
            Input.GetKeyDown(KeyCode.Joystick1Button19)) {
            Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().LoadMainMenu();
            Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().DisableMenu(MenuType.Credits);

        }



    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().DisableMenu(MenuType.Credits);
    }
    
}
