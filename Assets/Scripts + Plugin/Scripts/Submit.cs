using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Submit : Button
{
    public int X;
    public override void OnSubmit(BaseEventData eventData)
    {
        //Debug.Log("submit");
        base.OnSubmit(eventData);
    }


}
