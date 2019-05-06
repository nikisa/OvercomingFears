using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour {

    public int trapID;

    public GameObject projectile;

    float speed = 100f;

    [HideInInspector] public bool canShoot;


	void Start () {
        canShoot = true;
	}
	
    public int GetID() {
        return this.trapID;
    }

    public void Shoot() {
        GameObject temp;

        if (canShoot) {
            temp = Instantiate(projectile, transform.forward + new Vector3(this.transform.position.x, 0.5f, transform.position.z), Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
            canShoot = false;
        }        
    }
}
