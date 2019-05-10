using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public int trapID;
    public LayerMask obstacleLayer;

    public bool isShooting;

    public int GetID() {
        return this.trapID;
    }

    public void Shoot() {
        
            RaycastHit hit;
            if (Physics.Raycast(transform.GetChild(1).gameObject.transform.position, transform.GetChild(1).gameObject.transform.forward, out hit, 100, obstacleLayer)) {
                Debug.Log("HIT");
                Debug.DrawRay(GetComponent<Trap>().transform.GetChild(1).gameObject.transform.position, transform.GetChild(1).gameObject.transform.forward * hit.distance, Color.red);
                
                switch (hit.collider.tag) {
                    case "Enemy":
                        hit.collider.GetComponent<EnemyManager>().Die(); break;
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
        
            }
        
    }
}
