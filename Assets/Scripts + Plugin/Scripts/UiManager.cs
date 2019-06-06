using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public UiBase LevelSelectionState;
    public UiBase OptionState;
    public UiBase MainMenu;

    
    
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
}

public enum MenuType
{   MainMenu,
    LevelSelection,
    nullo,
    Option
}
