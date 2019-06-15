using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateMenu : StateBehaviourBase
{


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Menu");
        GameManager.Instance.IsGameplay = false;
    }
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Menu");

        GameManager.Instance.transform.GetChild(2).gameObject.SetActive(true);
        Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().ChangeMenu(MenuType.MainMenu);
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    
}
