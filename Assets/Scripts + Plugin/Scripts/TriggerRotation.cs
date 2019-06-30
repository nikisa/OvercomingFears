using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRotation : MonoBehaviour
{
    public Animator TriggerController;

    public void StopTriggerRotation(bool value)
    {

        if (!value)
        {
            TriggerController.SetBool("Rotation", value);
            transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(value);
        }
        else
        {
            TriggerController.SetBool("Rotation", value);
            transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(value);
        }
    }
}
