﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateOptions : StateMachineBehaviour
{

    public GameObject StateOption;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().ChangeMenu(MenuType.Option);
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button1)) {
            if (StateOption.activeSelf) {
                Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().LoadMainMenu();
                Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().DisableMenu(MenuType.Option);
            }
        }
    }


    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().DisableMenu(MenuType.Option);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
