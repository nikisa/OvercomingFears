﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    Resolution[] resolutions;
    public Dropdown resolutionDropdown;
    public AudioMixer audioMixer;

    private void Start() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++) {
            //GameObject resolution = (GameObject)Instantiate(resolutionButtonPrefab);
            //resolution.GetComponentInChildren<Text>().text = ResolutionToString(resolutions[i]);
            //int index = i;

            //resolution.GetComponent<Button>().onClick.AddListener(() => {SetResolution(index);} );
            //resolution.transform.parent = menuPanel;

            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    string ResolutionToString(Resolution resolution) {
        return resolution.width + " x " + resolution.height;
    }

    public void SetResolution(int resolutionIndex) {

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float volume) {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullScreen) {
        Screen.fullScreen = isFullScreen;
    }
}
