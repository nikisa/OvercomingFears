using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class SelectOnInput : MonoBehaviour
{

    public EventSystem eventSystem;
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
                case "Level 1":
                    GameManager.Instance.startLevel();
                    break;
                case "Level 2":
                    GameManager.Instance.startLevel();
                    break;
                case "Level 3":
                    GameManager.Instance.startLevel();
                    break;
                case "Level 4":
                    GameManager.Instance.startLevel();
                    break;
                case "Level 5":
                    GameManager.Instance.startLevel();
                    break;
                case "Level 6":
                    GameManager.Instance.startLevel();
                    break;
                case "Level 7":
                    GameManager.Instance.startLevel();
                    break;

                case "SETTINGS":
                    SubmitAction.GetComponent<UiManager>().Option();
                    break;
            }
            OnDisable();
        }
    }

    private void OnDisable() {
        buttonSelected = false;
    }
}
