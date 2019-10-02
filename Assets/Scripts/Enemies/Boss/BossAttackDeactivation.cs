using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackDeactivation : MonoBehaviour
{

    public GameObject BossAttack;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            BossAttack.SetActive(false);
        }
    }

}
