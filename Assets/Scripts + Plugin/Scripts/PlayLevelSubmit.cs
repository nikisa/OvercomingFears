using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayLevelSubmit : MonoBehaviour
{
    public PlayerManager player;
    public GameManager gm;
    public ScreenFader screenFaderDeath;
    public ScreenFader startButton;
    public Button playButton;


    void Update()
    {
        if (Input.anyKeyDown || player.isControllerInput()) {
            gm.PlayLevel();
            screenFaderDeath.FadeOff();
            startButton.FadeOff();
            playButton.enabled = false;
        }
    }
}
