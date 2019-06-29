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

        }
        else
        {
            TriggerController.SetBool("Rotation", value);
        }
    }
}
