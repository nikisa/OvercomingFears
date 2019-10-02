using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateGameplay : StateBehaviourBase
{  
 
    
    private void OnSceneLoaded(Scene scene , LoadSceneMode mode) {
        //Debug.Log("STARTGAME");
        GameManager.Instance.StartGameLoop();
        GameManager.Instance.IsGameplay = true;
    }


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.Instance.transform.GetChild(1).gameObject.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.stateMainMenu();
        //SceneManager.LoadScene("Level " + (ctx.id));

        Cursor.visible = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //GameManager.Instance.IsGameplay = false;
        //SceneManager.LoadScene("Menu");
    }

}
