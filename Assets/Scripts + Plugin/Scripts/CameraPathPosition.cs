using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class CameraPathPosition : MonoBehaviour {

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

    public bool autoAngle = false;

    #region autoangle variables

    float offsetX;
    float offsetY;
    float offsetZ;

    Quaternion rotationX;
    Quaternion rotationY;
    float rotationZ;

    #endregion

    void Start() {

        transform.position = cameraDolly.transform.position;
        cameraBond = true;
    }

    void Update() {

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
        transform.DOMove(cameraDolly.transform.position, easeTime).SetEase(easeType).OnComplete(() => CameraMove());
    }

    public void CameraRotate() {
        if (autoAngle) {
            transform.LookAt(cameraDolly.GetComponent<cameraFollow>().followAt);
            #region autoangle calculation

            //rotationX.eulerAngles = new Vector3(1 / (Mathf.Tan(offsetY / Mathf.Abs(offsetZ))), transform.rotation.y, transform.rotation.z);
            //rotationY.eulerAngles = new Vector3(transform.rotation.x, 0 - (1 / (Mathf.Tan(offsetX / Mathf.Abs(offsetZ)))), transform.rotation.z);

            //offsetX = cameraDolly.GetComponent<cameraFollow>().followAt.transform.position.x - transform.position.x;
            //offsetY = cameraDolly.GetComponent<cameraFollow>().followAt.transform.position.y - transform.position.y;
            //offsetZ = cameraDolly.GetComponent<cameraFollow>().followAt.transform.position.z - transform.position.z;

            //transform.rotation = new Vector3(rotationX, rotationY, transform.position.z);

            #endregion
        }
        else {
            transform.DORotate(cameraDolly.transform.rotation.eulerAngles, easeTime).SetEase(angleEaseType).OnComplete(()=>CameraRotate());
        }

    }
}
