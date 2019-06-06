using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    

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

    bool m_e;
    public bool ESC { get { return m_e; } }

    bool m_r;
    public bool R { get { return m_r; } }


    bool m_inputEnabled = false;
    public bool InputEnabled
    {
        get
        {
            return m_inputEnabled;
        }

        set
        {
            m_inputEnabled = value;
        }
    }

    //playerInput.InputEnabled = true;


    public void GetKeyInput()
    {
        
        if (m_inputEnabled)
        {
            m_h = Input.GetAxisRaw("Vertical");
            m_v = Input.GetAxisRaw("Horizontal");
            m_s = Input.GetKeyDown(KeyCode.Space);
            m_p = Input.GetKey(KeyCode.LeftShift);
            m_f = Input.GetKey(KeyCode.LeftControl);
            m_e = Input.GetKeyDown(KeyCode.Escape);
            m_r = Input.GetKey(KeyCode.R);

        }
        else
        {
            m_h = 0f;
            m_v = 0f;
            m_s = false;
            m_p = false;
            m_f = false;
            m_e = false;
            m_r = false;
        }

    }


}