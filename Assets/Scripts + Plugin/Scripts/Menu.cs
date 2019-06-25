using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    

    public void NewGame()
    {
        Debug.Log("NEW GAME");
        SceneManager.LoadScene("Level 1");
        GameManager.stateGameplay(); //Chiamata evento 
        
    }
    public void LoadGame()
    {
        Debug.Log("LOAD GAME");
    }
    public void LevelSelection()
    {
        Debug.Log("LEVEL SELECTION");
    }
    public void Option()
    {
        Debug.Log("OPTION");
    }
    public void Exit()
    {
        Debug.Log("EXIT");
    }
    public void BackToMenu()
    {
        GameManager.stateMenu();
    }

}
