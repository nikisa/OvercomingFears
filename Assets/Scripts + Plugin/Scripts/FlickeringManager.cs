using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringManager : MonoBehaviour
{
    public Animator AnimatorController;


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            AnimatorController.SetBool("flick1",true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            AnimatorController.SetBool("flick1", false);
        }
    }

}
