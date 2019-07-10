using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class SelectoOnInput : MonoBehaviour
{

    public EventSystem eventSystem;
    public GameObject selectedObject;
    public GameObject SubmitAction;

    public bool buttonSelected;
    

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) && !buttonSelected) {
            
            //EventSystem.current.SetSelectedGameObject(selectedObject.gameObject);
            buttonSelected = true;
            Debug.Log("SELECTED " + EventSystem.current.currentSelectedGameObject.name);
            switch (EventSystem.current.currentSelectedGameObject.name) {
                case "NEW GAME":
                    SubmitAction.GetComponent<UiManager>().PlayGame();
                    break;
                case "SETTINGS":
                    SubmitAction.GetComponent<UiManager>().Option();
                    break;
            }

        }
    }

    private void OnDisable() {
        buttonSelected = false;
    }
}
