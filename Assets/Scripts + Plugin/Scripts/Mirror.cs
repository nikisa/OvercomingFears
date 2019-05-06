using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MovableObject {

    public bool downLeft = false;
    public bool downRight = false;
    public bool upLeft = false;
    public bool upRight = false;

    [HideInInspector]public int i;

    public LayerMask obstacleLayer;

    bool[] mirrorRotations;
    
    void Start() {
        mirrorRotations = new bool[] { downRight, downLeft, upLeft, upRight };
        InitIndex();
        InitMirror();
    }

    public int mirrorID;

    public void UpdateMirrorRotation() {
        i++;
        InitMirror();
    }

    public void InitMirror() {
        if (i % mirrorRotations.Length == 0) {
            gameObject.transform.rotation = Quaternion.Euler(0,315,0);
        }
        else if (i % mirrorRotations.Length == 1) {
            gameObject.transform.rotation = Quaternion.Euler(0, 45, 0);
        }
        else if (i % mirrorRotations.Length == 2) {
            gameObject.transform.rotation = Quaternion.Euler(0, 135, 0);
        }
        else if (i % mirrorRotations.Length == 3) {

            gameObject.transform.rotation = Quaternion.Euler(0, 225, 0);
        }
    }

    public void InitIndex() {
        if (downRight == true) {
            i = 0;
        }
        else if (downLeft == true) {
            i = 1;
        }
        else if (upLeft == true) {
            i = 2;
        }
        else if (upRight == true) {
            i = 3;
        }
    }

    public int getIndex() {
        return i;
    }

    public void MirrorShootLeft(){
        RaycastHit hit;
        Vector3 temp = transform.position - new Vector3(.3f,0,0);
        if (Physics.Raycast(temp, Vector3.left, out hit, 100, obstacleLayer)) {
            Debug.Log("ShootLeft");
            Debug.DrawRay(GetComponent<Mirror>().transform.position + new Vector3(0, 0.5f), Vector3.left * hit.distance, Color.red);

            switch (hit.collider.tag) {
                case "Enemy":
                    hit.collider.GetComponent<EnemyManager>().Die();
                    break;
                case "Mirror":
                    int index = 100;
                    index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;

                    switch (index) {

                        case 3:
                            hit.collider.GetComponent<Mirror>().MirrorShootUp();
                            Debug.Log("case 3");
                            break;

                        case 0:
                            hit.collider.GetComponent<Mirror>().MirrorShootDown();
                            Debug.Log("case 0");
                            break;
                        
                    }
                break;
                            
                case "Wall":
                    Debug.Log("WALL");
                    break;
            }
        }
    }


    public void MirrorShootRight() {
        RaycastHit hit;
        Vector3 temp = transform.position + new Vector3(.3f, 0, 0);
        if (Physics.Raycast(temp, Vector3.right, out hit, 100, obstacleLayer)) {
            Debug.Log("ShootRight");
            Debug.DrawRay(GetComponent<Mirror>().transform.position + new Vector3(0, 0.5f), Vector3.right * hit.distance, Color.red);

            switch (hit.collider.tag) {
                case "Enemy":
                    hit.collider.GetComponent<EnemyManager>().Die();
                    break;
                case "Mirror":
                    int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;

                    switch (index) {

                        case 1:
                            hit.collider.GetComponent<Mirror>().MirrorShootDown();
                            Debug.Log("case 1");
                            break;
                        case 2:
                            hit.collider.GetComponent<Mirror>().MirrorShootUp();
                            Debug.Log("case 2");
                            break;
                    }
                    break;

                case "Wall":
                    Debug.Log("WALL");
                    break;
            }
        }
    }

    public void MirrorShootUp() {
        RaycastHit hit;
        Vector3 temp = transform.position + new Vector3(0, 0, .3f);
        if (Physics.Raycast(temp, Vector3.forward, out hit, 1000, obstacleLayer)) {
            Debug.Log("ShootUp");
            Debug.DrawRay(GetComponent<Mirror>().transform.position + new Vector3(0, 0.5f), Vector3.forward * hit.distance, Color.red);

            switch (hit.collider.tag) {
                case "Enemy":                    
                    hit.collider.GetComponent<EnemyManager>().Die();
                    break;
                case "Mirror":
                    int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;

                    switch (index) {

                        case 0:
                            hit.collider.GetComponent<Mirror>().MirrorShootRight();
                            Debug.Log("case 0");
                            break;
                        case 1:
                            hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                            Debug.Log("case 1");
                            break;
                    }

                    break;

                case "Wall":
                    Debug.Log("WALL");
                    break;
            }
        }
    }

    public void MirrorShootDown() {
        RaycastHit hit;
        Vector3 temp = transform.position - new Vector3(0, 0, .3f);
        if (Physics.Raycast(temp, Vector3.back, out hit, 100, obstacleLayer)) {
            Debug.Log("ShootDown");
            Debug.DrawRay(GetComponent<Mirror>().transform.position + new Vector3(0, 0.5f), Vector3.back * hit.distance, Color.red);

            switch (hit.collider.tag) {
                case "Enemy":
                    hit.collider.GetComponent<EnemyManager>().Die();
                    break;
                case "Mirror":
                    int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;

                    switch (index) {

                        case 2:
                            hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                            Debug.Log("case 2");
                            break;
                        case 3:
                            hit.collider.GetComponent<Mirror>().MirrorShootRight();
                            Debug.Log("case 3");
                            break;
                    }

                    break;

                case "Wall":
                    Debug.Log("WALL");
                    break;
            }
        }
    }
}
