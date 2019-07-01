using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateMenu : StateBehaviourBase
{


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.IsGameplay = false;
    }
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Menu");

        GameManager.Instance.transform.GetChild(4).gameObject.SetActive(true);


        //Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().ChangeMenu(MenuType.MainMenu);

        //Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().DisableMenu(MenuType.LevelSelection);

        Object.FindObjectOfType<LevelManager>().GetComponent<LevelManager>().LevelSelectionDisable();
        Object.FindObjectOfType<LevelManager>().GetComponent<LevelManager>().LevelSelectionSetup();

        Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>().hasFlashLight = false;
        Object.FindObjectOfType<PlayerManager>().transform.GetChild(3).gameObject.SetActive(false);
        Cursor.visible = true;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.Instance.transform.GetChild(4).gameObject.SetActive(false);
    }


    
}
