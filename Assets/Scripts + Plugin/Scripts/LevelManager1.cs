using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class LevelManager : MonoBehaviour {
    
    public string LevelToLoad;

    public List<string> LevelsID = new List<string>();

    public GameObject LevelSelectionButton;

    public GameObject[] LevelButtons;
    
    public void SaveLevel() {
        
        LevelData saveClass = new LevelData();
        saveClass.ID = "Level " + (SceneManager.GetActiveScene().buildIndex + 1).ToString();
        LevelsID.Add(saveClass.ID);
        Debug.LogFormat("{0} saved!", saveClass.ID);

        // Salvo livello
        
        PlayerPrefs.SetString("LevelID", SceneManager.GetActiveScene().buildIndex + "");
        // Salvo il nome del livello nella lista dei livelli
        PlayerPrefs.Save();
    }
    

    public void resetLevels() {
        LevelsID.Clear();
        PlayerPrefs.SetString("LevelID" , "0");
    }

    public void LevelSelectionSetup() {
        for (int i = 1; i <= PlayerPrefs.GetInt("LevelID"); i++) {
            LevelButtons[i].GetComponent<Button>().interactable = true;
        }
    }

    public void LevelSelectionDisable() {        

        if (PlayerPrefs.GetInt("LevelsID") == 0) {
            LevelSelectionButton.GetComponent<Button>().interactable = false;
        }
        
    }

}

[Serializable]
public class LevelData {
    public string ID;
    public List<ItemData> Items;
}

public class LevelSaves {
    public List<string> LevelsID;
}