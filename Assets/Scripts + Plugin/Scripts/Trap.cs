using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public Material[] materials;

    public int trapID;
    public LayerMask obstacleLayer;

    public bool isShooting;

    LineRenderer _lr;

    public int GetID() {
        return this.trapID;
    }

    private void Awake() {

        _lr = Object.FindObjectOfType<LineRenderer>().GetComponent<LineRenderer>();

        if (trapID <= 6 && trapID >= 1) {
            transform.GetChild(1).gameObject.GetComponent<Renderer>().material = materials[trapID - 1];
        }
    }

    IEnumerator DisableLineRenderer() {
        yield return new WaitForSeconds(.5f);
        _lr.gameObject.SetActive(false);
        _lr.gameObject.SetActive(false);

    }

    public void Shoot() {

        _lr.gameObject.SetActive(true);
        RaycastHit hit;
        _lr.gameObject.SetActive(true);

        _lr.SetPosition(0, transform.GetChild(1).gameObject.transform.position + new Vector3(0, 0, 0));



        if (Physics.Raycast(transform.GetChild(1).gameObject.transform.position, transform.GetChild(1).gameObject.transform.forward, out hit, 100, obstacleLayer)) {
                Debug.Log("HIT");
                Debug.DrawRay(GetComponent<Trap>().transform.GetChild(1).gameObject.transform.position, transform.GetChild(1).gameObject.transform.forward * hit.distance, Color.red);
                
                switch (hit.collider.tag) {
                    case "Enemy":
                        hit.collider.GetComponent<EnemyManager>().Die();
                        _lr.SetPosition(1, hit.point + new Vector3(0, 0, 0));
                        break;
                    case "Mirror":
                        int index = (hit.collider.GetComponent<Mirror>().getIndex()) % 4;
                        _lr.SetPosition(1, hit.point + new Vector3(0, 0, 0));
                    switch (index) {

                            case 0:
                                hit.collider.GetComponent<Mirror>().MirrorShootRight();
                                Debug.Log("case 0");
                                break;

                            case 1:
                                hit.collider.GetComponent<Mirror>().MirrorShootLeft();
                                Debug.Log("case 1");
                                break;

                            case 2:
                                hit.collider.GetComponent<Mirror>().MirrorShootUp();
                                Debug.Log("case 3");
                                break;
                            

                            case 3:
                                hit.collider.GetComponent<Mirror>().MirrorShootDown();
                                Debug.Log("case 2");
                                break;

                    }
                        break;
                    case "Wall":
                        break;
                }

                StartCoroutine(DisableLineRenderer());

        }
        
    }
}
