using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UiManager : MonoBehaviour
{
    public UiBase LevelSelectionState;
    public UiBase OptionState;
    public UiBase MainMenu;
    public UiBase Popup;
    public UiBase StateCover;
    public UiBase PlayMenu;

    bool value = false;

    public bool isCover;

    public void ChangeMenu(MenuType _menuType)
    {
        switch (_menuType)
        {
            case MenuType.Cover:
                isCover = true;
                StateCover.Setup();
                break;
            case MenuType.MainMenu:
                MainMenu.Setup();
                break;
            case MenuType.PlayMenu:
                PlayMenu.Setup();
                break;
            case MenuType.LevelSelection:
                LevelSelectionState.Setup();
                break;
            case MenuType.nullo:
                break;
            case MenuType.Option:
                OptionState.Setup();
                break;
            case MenuType.Popup:
                Popup.Setup();
                break;
            default:
                break;
        }
    }
    public void DisableMenu(MenuType _menuType)
    {
        switch (_menuType)
        {
            case MenuType.Cover:
                isCover = false;
                StateCover.Disable();
                break;
            case MenuType.MainMenu:
                MainMenu.Disable();
                break;
            case MenuType.PlayMenu:
                PlayMenu.Disable();
                break;
            case MenuType.LevelSelection:
                LevelSelectionState.Disable();
                break;
            case MenuType.nullo:
                break;
            case MenuType.Option:
                OptionState.Disable();
                break;
            case MenuType.Popup:
                Popup.Disable();
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

    public void PlayGame() {
        //Debug.Log("PLAY GAME");
        GameManager.statePlayMenu();
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
    public void LoadPopup()
    {
        GameManager.statePopup();
    }
    public void GameplayUI()
    {
        GameManager.stateGameplayUI();
    }


}

public enum MenuType
{   Cover,
    MainMenu,
    PlayMenu,
    LevelSelection,
    nullo,
    Option,
    Popup,

}


