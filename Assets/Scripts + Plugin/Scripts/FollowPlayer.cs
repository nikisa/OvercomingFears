using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

    public GameObject character;
    Transform _target;
    Vector3 characterPosition;
    public float cameraHeight;
    public float cameraAngle;

    void Update() {

        characterPosition = new Vector3(character.transform.position.x + cameraAngle, cameraHeight, character.transform.position.z - cameraAngle);
        transform.position = characterPosition;
        transform.LookAt(_target);
    }
}