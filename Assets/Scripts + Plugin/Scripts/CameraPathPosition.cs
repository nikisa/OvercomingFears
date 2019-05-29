using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class CameraPathPosition : MonoBehaviour
{

    [HideInInspector]
    public bool cameraBond;

    public GameObject cameraDolly;

    public float delay;

    [HideInInspector]
    public Ease easeType = Ease.OutQuint;

    [HideInInspector]
    public Ease angleEaseType = Ease.OutQuint;

    public float easeTime;

    public float easeTimeZoom;


    public bool OneTimeTwining;

    public float LerpTime;


    void Start()
    {
        
        transform.position = cameraDolly.transform.position;
        cameraBond = true;
    }

    void Update()
    {
        
        if (cameraBond) {
            transform.position = cameraDolly.transform.position;
            transform.rotation = cameraDolly.transform.rotation;
        }
        else {

            CameraMove();
            CameraRotate();
        }

    }
    

    public void CameraMove() {
        transform.DOMove(cameraDolly.transform.position, easeTime).SetEase(easeType);
    }

    public void CameraRotate() {
        transform.DORotate(cameraDolly.transform.rotation.eulerAngles , easeTime).SetEase(angleEaseType);
    }

    

    
}
