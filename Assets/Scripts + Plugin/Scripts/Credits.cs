using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Credits : MonoBehaviour {

    public GameObject Cutscene;
    public VideoPlayer credits;
    public RawImage rawImage;

    bool isPlaying = false;

    IEnumerator PlayCreditsCoroutine() {
        isPlaying = true;
        credits.Prepare();
        while (!credits.isPrepared) {
            yield return new WaitForSeconds(1);
            break;
        }

        rawImage.texture = credits.texture;
        credits.Play();
        yield return new WaitForSeconds(50);
        credits.Stop();
        Cutscene.SetActive(false);
        isPlaying = false;
    }


    private void Update() {
        if (isPlaying == true && (Input.anyKeyDown ||
            Input.GetKeyDown(KeyCode.Joystick1Button1) ||
            Input.GetKeyDown(KeyCode.Joystick1Button2) ||
            Input.GetKeyDown(KeyCode.Joystick1Button3) ||
            Input.GetKeyDown(KeyCode.Joystick1Button4) ||
            Input.GetKeyDown(KeyCode.Joystick1Button5) ||
            Input.GetKeyDown(KeyCode.Joystick1Button6) ||
            Input.GetKeyDown(KeyCode.Joystick1Button7) ||
            Input.GetKeyDown(KeyCode.Joystick1Button8) ||
            Input.GetKeyDown(KeyCode.Joystick1Button9) ||
            Input.GetKeyDown(KeyCode.Joystick1Button10) ||
            Input.GetKeyDown(KeyCode.Joystick1Button11) ||
            Input.GetKeyDown(KeyCode.Joystick1Button12) ||
            Input.GetKeyDown(KeyCode.Joystick1Button13) ||
            Input.GetKeyDown(KeyCode.Joystick1Button14) ||
            Input.GetKeyDown(KeyCode.Joystick1Button15) ||
            Input.GetKeyDown(KeyCode.Joystick1Button16) ||
            Input.GetKeyDown(KeyCode.Joystick1Button17) ||
            Input.GetKeyDown(KeyCode.Joystick1Button18) ||
            Input.GetKeyDown(KeyCode.Joystick1Button19))) {
            skipCutscene();
            //isPlaying = false;
        }
    }


    public void PlayCredits() {
        StartCoroutine(PlayCreditsCoroutine());
    }

    public void skipCutscene() {
        if (Cutscene.activeSelf) {
            credits.Stop();
            
            Cutscene.SetActive(false);
        }
    }
}