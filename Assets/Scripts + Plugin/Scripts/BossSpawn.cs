using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossSpawn : MonoBehaviour
{
    public GameObject Boss;


    public float delay;

    public float time;
    public Ease easeType = Ease.OutQuint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(BossSpawning(other));
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
        yield return new WaitForSeconds(delay);
        Boss.transform.GetChild(0).gameObject.SetActive(true);
        Boss.transform.GetChild(1).gameObject.SetActive(true);
        other.GetComponent<PlayerManager>().playerInput.InputEnabled = true;
    }
}
