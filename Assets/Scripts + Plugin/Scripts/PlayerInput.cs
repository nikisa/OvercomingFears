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
    bool m_sJ;
    public bool S { get { return m_s | m_sJ; ; } }

    bool m_p;
    bool m_pJ;
    public bool P { get { return m_p | m_pJ; } }

    bool m_f;
    bool m_fJ;
    public bool F { get { return m_f | m_fJ; } }

    bool m_e;
    bool m_eJ;
    public bool ESC { get { return m_e | m_eJ; } }

    bool m_r;
    bool m_rJ;
    public bool R { get { return m_r | m_rJ; } }


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
            m_h = Input.GetAxisRaw("Vertical");
            m_v = Input.GetAxisRaw("Horizontal");

            m_s = Input.GetKeyDown(KeyCode.Space);
            m_sJ = Input.GetKeyDown(KeyCode.Joystick1Button0);

            m_p = Input.GetKey(KeyCode.LeftShift);
            m_pJ = Input.GetKey(KeyCode.Joystick1Button2);

            m_f = Input.GetKey(KeyCode.LeftControl);
            m_fJ = Input.GetKey(KeyCode.Joystick1Button1);

            m_e = Input.GetKeyDown(KeyCode.Escape);
            m_eJ = Input.GetKeyDown(KeyCode.Joystick1Button7);

            m_r = Input.GetKeyDown(KeyCode.R);
            m_rJ = Input.GetKeyDown(KeyCode.Joystick1Button3);

        }
        else {
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