using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerToIdle : MonoBehaviour
{

    public Animator animatorController;

    public void ReturnToIdle() {
        animatorController.SetInteger("PlayerState", 0);
    }
}
