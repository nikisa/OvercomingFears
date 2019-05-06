using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        switch (other.tag) {
            case "Player":
                Destroy(gameObject);
                other.GetComponent<PlayerManager>().PlayerDead();
                break;
            case "Enemy":
                Destroy(gameObject);
                other.GetComponent<EnemyManager>().Die();
                break;
            default:
                Destroy(gameObject);
                break;
        }
    }
}
