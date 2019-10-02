using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Window : MonoBehaviour
{
    void Start()
    {
        transform.Find("Btn").GetComponent<Button_UI>().ClickFunc = () => //Debug.Log("Click");
        transform.Find("Btn").GetComponent<Button_UI>().AddButtonSounds();

    }

}
