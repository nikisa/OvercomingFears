using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    public GameObject Boss;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Boss.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
