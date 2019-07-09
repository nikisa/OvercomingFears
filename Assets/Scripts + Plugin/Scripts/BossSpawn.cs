using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossSpawn : MonoBehaviour
{
    private int count = 0;

    public GameObject Boss;


    public float delay;

    public float time;
    public Ease easeType = Ease.OutQuint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(BossSpawning(other));

            if (count > 0)
            {
                Boss.transform.DOMove(Boss.transform.position + new Vector3(0, 0, 4f), time).SetEase(easeType);
            }
            count++;
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Boss.transform.DOMove(Boss.transform.position + new Vector3(0, 0, 4f), time).SetEase(easeType);
        }
    }

    IEnumerator BossSpawning(Collider other) {
        other.GetComponent<PlayerManager>().playerInput.InputEnabled = false;
        other.GetComponent<PlayerManager>().gameObject.transform.GetChild(1).gameObject.SetActive(false);//Disattivo il PlayerCompass
        yield return new WaitForSeconds(delay);
        Boss.transform.GetChild(0).gameObject.SetActive(true);
        Boss.transform.GetChild(1).gameObject.SetActive(true);
        other.GetComponent<PlayerManager>().playerInput.InputEnabled = true;
        other.GetComponent<PlayerManager>().gameObject.transform.GetChild(1).gameObject.SetActive(true);//Riattivo il PlayerCompass
    }
}
