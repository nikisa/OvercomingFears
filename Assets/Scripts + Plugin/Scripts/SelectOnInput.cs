﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class SelectOnInput : MonoBehaviour
{

    public EventSystem eventSystem;
    public GameObject SubmitAction;

    public GameObject windowed;
    public GameObject resolutionDropdown;
    public GameObject graphicsDropdown;

    public bool buttonSelected;
    

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) && !buttonSelected) {
            
            //EventSystem.current.SetSelectedGameObject(selectedObject.gameObject);
            buttonSelected = true;
            
            if (EventSystem.current.currentSelectedGameObject != null) {
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
                    case "GrapichsDropdown":
                        graphicsDropdown.GetComponent<Dropdown>().Show();
                        break;
                    case "ResolutionDropdown":
                        resolutionDropdown.GetComponent<Dropdown>().Show();
                        break;
                    case "Windowed":
                        windowed.GetComponent<Toggle>().isOn = !windowed.GetComponent<Toggle>().isOn;
                        break;
                }
            }
            OnDisable();
        }
    }

    private void OnDisable() {
        buttonSelected = false;
    }
}
