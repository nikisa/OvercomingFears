using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossTrigger : MonoBehaviour
{
    public GameObject Boss;
    public float time;
    public Ease easeType = Ease.OutQuint;


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Boss.transform.DOMove(Boss.transform.position + new Vector3(0, 0, 4f) , time).SetEase(easeType);
            StartCoroutine(ResetBoss());
            

        }
    }

    private void OnTriggerExit(Collider other) { 
        if (other.tag == "Player") {
            Boss.transform.DOMove(Boss.transform.position + new Vector3(0, 0, 4f) , time).SetEase(easeType);
            StartCoroutine(ResetBoss());

        }
    }

    public IEnumerator ResetBoss() {
        Boss.transform.GetChild(0).GetComponent<EnemyMover>().EnemyAnimatorController.SetInteger("ChaserState", 3);
        yield return new WaitForSeconds(.1f);
        Boss.transform.GetChild(0).GetComponent<EnemyMover>().EnemyAnimatorController.SetInteger("ChaserState", 2);
    }

}

