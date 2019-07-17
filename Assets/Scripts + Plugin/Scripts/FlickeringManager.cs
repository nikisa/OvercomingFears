using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringManager : MonoBehaviour
{
    public Animator AnimatorController;
    public int value;


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            AnimatorController.SetInteger("flickering",value);
            SoundManager.PlaySound(SoundManager.Sound.Flicker_Livello1);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            AnimatorController.SetInteger("flickering", 0);
            value = 0;
        }
    }

}
