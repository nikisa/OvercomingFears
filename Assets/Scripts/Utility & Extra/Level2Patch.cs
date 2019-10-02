using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Patch : MonoBehaviour
{
    public GameObject FuckingMonkey1;
    public GameObject FuckingMonkey2;


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            FuckingMonkey1.GetComponent<EnemyManager>().Die();
            FuckingMonkey2.GetComponent<EnemyManager>().Die();
        }
    }
}
