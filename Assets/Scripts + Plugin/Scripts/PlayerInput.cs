using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerInput : MonoBehaviour {


    float m_h;
    public float H { get { return m_h; } }

    float m_v;
    public float V { get { return m_v; } }


    bool m_s;
    public bool S { get { return m_s; } }

    bool m_p;
    public bool P { get { return m_p; } }

    bool m_f;
    public bool F { get { return m_f; } }


    bool m_inputEnabled = false;
    public bool InputEnabled {
        get {
            return m_inputEnabled;
        }

        set {
            m_inputEnabled = value;
        }
    }

    //playerInput.InputEnabled = true;


    public void GetKeyInput() {

        if (m_inputEnabled) {
            m_h = Input.GetAxisRaw("Horizontal");
            m_v = Input.GetAxisRaw("Vertical");
            m_s = Input.GetKeyDown(KeyCode.Return);//???
            m_p = Input.GetKey(KeyCode.LeftShift);
            m_f = Input.GetKey(KeyCode.F);



        }
        else {
            m_h = 0f;
            m_v = 0f;
            m_s = false;
            m_p = false;
            m_f = false;
        }
        
    }

   
}