using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XORDelay : MonoBehaviour
{
    float movementDelay;

    private void OnTriggerEnter(Collider other) {
        movementDelay = other.GetComponent<PlayerMover>().playerMovementDelay;
        other.GetComponent<PlayerMover>().playerMovementDelay = .5f;
    }

    private void OnTriggerExit(Collider other) {
        other.GetComponent<PlayerMover>().playerMovementDelay = movementDelay;
    }
}
