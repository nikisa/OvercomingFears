using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShaft : MonoBehaviour
{

    public void lightShaftScale(float length)
    {
        transform.localScale = new Vector3(transform.localScale.x , transform.localScale.y , length) ;
    } 
}
