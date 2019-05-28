using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject Boss;


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Boss.transform.position = Vector3.MoveTowards(Boss.transform.position, Boss.transform.position + new Vector3(0, 0, 4f) , 4);
        }
    }
}
