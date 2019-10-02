using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCamera : MonoBehaviour
{
    cameraTrigger _cameraTrigger;

    public int localCameraID;

    public PlayerManager player;
    public GameObject cameraFocus;

    public bool onAir;

    [Range(0f, 1f)]
    public float lerpValue;

    private void Awake() {

        _cameraTrigger = Object.FindObjectOfType<cameraTrigger>().GetComponent<cameraTrigger>();
        player = Object.FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        cameraFocus.transform.position = player.transform.position;
    }

    private void Update() {

        if (onAir) {
            cameraFocus.transform.position = Vector3.Lerp(player.transform.position, transform.position, lerpValue);
        }
        
    }

    

}
