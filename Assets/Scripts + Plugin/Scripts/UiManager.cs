﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public UiBase LevelSelectionState;
    public UiBase OptionState;
    public UiBase MainMenu;
    public UiBase VideoSettingsState;

    bool value = false;

    public void ChangeMenu(MenuType _menuType)
    {
        switch (_menuType)
        {
            case MenuType.MainMenu:
                MainMenu.Setup();
                break;
            case MenuType.LevelSelection:
                LevelSelectionState.Setup();
                break;
            case MenuType.nullo:
                break;
            case MenuType.Option:
                OptionState.Setup();
                break;
            case MenuType.VideoSettingsType:
                VideoSettingsState.Setup();
                break;
            default:
                break;
        }
    }
    public void DisableMenu(MenuType _menuType)
    {
        switch (_menuType)
        {
            case MenuType.MainMenu:
                MainMenu.Disable();
                break;
            case MenuType.LevelSelection:
                LevelSelectionState.Disable();
                break;
            case MenuType.nullo:
                break;
            case MenuType.Option:
                OptionState.Disable();
                break;
            case MenuType.VideoSettingsType:
                VideoSettingsState.Disable();
                break;
            default:
                break;
        }
    }
    public void Option()
    {
        GameManager.stateOption();
    }
    public void LevelSelection()
    {
        GameManager.stateLevelSelection();
    }

    public void BackToMainMenu()
    {
        GameManager.stateMainMenu();
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene(0);
        GameManager.stateMenu();
        GameManager.stateMainMenu();

    }
    public void VideoSettings()
    {
        GameManager.stateVideoSettings();
    }
    public void GameplayUI()
    {
        GameManager.stateGameplayUI();
    }

    public void SoundSlider()
    {
       

        if (value)
        {
            Debug.Log("true");
            GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>().interactable = false;
            GameObject.FindGameObjectWithTag("Slider").transform.localScale = new Vector3(0, 0, 0);

            value = false;
        }
        else
        {
            GameObject.FindGameObjectWithTag("Slider").GetComponent<Slider>().interactable = true;
            GameObject.FindGameObjectWithTag("Slider").transform.localScale = new Vector3(3, 3, 3);
            value = true;
        }
    }

}

public enum MenuType
{   MainMenu,
    LevelSelection,
    nullo,
    Option,
    VideoSettingsType,

}


