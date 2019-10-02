using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class StateVideoSettings : StateMachineBehaviour
{


    float m_h;
    float m_hJ;
    public float H { get { return m_h + m_hJ; } }

    float m_v;
    float m_vJ;
    public float V { get { return m_v + m_vJ; } }


    bool m_s;
    bool m_sJ;
    public bool S { get { return m_s | m_sJ; ; } }

    bool m_r;
    bool m_rJ;
    public bool R { get { return m_r | m_rJ; } }

    public GameObject qualityText;
    public GameObject resolutionText;
    public GameObject WindowedText;

    public int VideoSettingsIndex = 0;
    public int qualityNamesIndex = 0;
    public int resolutionIndex = 0;

    public bool windowed = false;

    string[] qualityNames;

    Resolution[] resolutions;

    public void GetKeyInput() {

        m_h = Input.GetAxisRaw("Vertical");
        m_hJ = Input.GetAxisRaw("VerticalJ");

        m_v = Input.GetAxisRaw("Horizontal");
        m_vJ = Input.GetAxisRaw("HorizontalJ");

        m_s = Input.GetKeyDown(KeyCode.Space);
        m_sJ = Input.GetKeyDown(KeyCode.Joystick1Button0);

        m_r = Input.GetKeyDown(KeyCode.R);
        m_rJ = Input.GetKeyDown(KeyCode.Joystick1Button3);

    }


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().ChangeMenu(MenuType.Popup);
        //qualityNames = gameSettings.GetComponent<GameSettings>().QualityNames.ToArray();
        //resolutions = gameSettings.GetComponent<GameSettings>().Resolutions.ToArray();

        qualityNames = new List<string>(QualitySettings.names).ToArray();
        resolutions = new List<Resolution>(Screen.resolutions).ToArray();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
       
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Object.FindObjectOfType<UiManager>().GetComponent<UiManager>().DisableMenu(MenuType.Popup);
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
