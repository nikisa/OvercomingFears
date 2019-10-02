using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelManager : MonoBehaviour {

    public int LevelProgress;

    public GameObject LevelSelectionButton;

    public GameObject[] LevelButtons;
    
    public void SaveLevel() {
        
        // Salvo livello
        
        if (PlayerPrefs.GetInt("LevelID") < SceneManager.GetActiveScene().buildIndex +1) {
            PlayerPrefs.SetInt("LevelID", SceneManager.GetActiveScene().buildIndex);
            // Salvo il nome del livello nella lista dei livelli
            PlayerPrefs.Save();
        }
        
    }
    

    public void resetLevels() {
        PlayerPrefs.SetInt("LevelID" , 0);
        //Debug.Log("reset");
        LevelSelectionSetup();
        LevelSelectionDisable();
    }

    public void LevelSelectionSetup() {
        for (int i = 1; i <= PlayerPrefs.GetInt("LevelID"); i++) {
            LevelButtons[i].GetComponent<Button>().interactable = true;
        }
    }

    public void LevelSelectionDisable() {        

        if (PlayerPrefs.GetInt("LevelID") == 0) {
            LevelSelectionButton.GetComponent<Button>().interactable = false;
        }
        else {
            LevelSelectionButton.GetComponent<Button>().interactable = true;
        }
        
    }

    private void Update() {
        LevelProgress = PlayerPrefs.GetInt("LevelID");
    }

    public void LoadGame() {

        if (PlayerPrefs.GetInt("LevelID") > 10) {
            SceneManager.LoadScene("Level 10");
            GameManager.stateGameplay();
        }
        else {
            SceneManager.LoadScene(PlayerPrefs.GetInt("LevelID") + 1);
            GameManager.stateGameplay();
        }
        
        
    }

}





public class LevelSaves {
    public List<string> LevelsID;
}