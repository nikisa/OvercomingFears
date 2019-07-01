using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossSpawn : MonoBehaviour
{
    public GameObject Boss;

    public float time;
    public Ease easeType = Ease.OutQuint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Boss.transform.GetChild(0).gameObject.SetActive(true);
            Boss.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (Boss.gameObject.activeSelf && other.tag == "Player")
        {
            Boss.transform.DOMove(Boss.transform.position + new Vector3(0, 0, 4f), time).SetEase(easeType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Boss.transform.DOMove(Boss.transform.position + new Vector3(0, 0, 4f), time).SetEase(easeType);
        }
    }
}
