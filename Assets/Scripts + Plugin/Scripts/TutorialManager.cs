using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour {
    public int tutorialId;

    GameManager m_gm;
    PlayerManager m_player;

    ScreenFader deletePopup;

    GameObject popup;

    private void Awake() {
        m_gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        m_player = FindObjectOfType<PlayerManager>().GetComponent<PlayerManager>();
        m_gm.playerPopupID = 1;
    }

    private void OnTriggerEnter(Collider other) {

        if (other.tag == "Player" && SceneManager.GetActiveScene().buildIndex == 1) {

            switch (tutorialId) {
                case 1:
                    if (m_gm.playerPopupID == tutorialId) {
                        deletePopup = Object.FindObjectOfType<ScreenFader>().GetComponent<ScreenFader>();
                        deletePopup.FadeOff();
                        m_gm.playerPopupID++;
                        StartCoroutine(DestroyMovementPopup());

                    }
                    break;

                case 2:
                    if (m_gm.playerPopupID == tutorialId) {
                        if (!m_player.usingController) {
                            popup = Instantiate(m_gm.TutorialsKeyboard[1]);
                        }
                        else {
                            popup = Instantiate(m_gm.TutorialsController[1]);
                        }
                        popup.transform.GetChild(0).gameObject.GetComponent<ScreenFader>().FadeOn();
                        m_gm.playerPopupID++;
                    }
                    break;

                case 3:
                    if (m_gm.playerPopupID == tutorialId) {
                        deletePopup = Object.FindObjectOfType<ScreenFader>().GetComponent<ScreenFader>();
                        deletePopup.FadeOff();
                        m_gm.playerPopupID++;
                        StartCoroutine(DestroyMovementPopup());

                    }
                    break;

            }
        }


        if (other.tag == "Player" && SceneManager.GetActiveScene().buildIndex == 4) {

            switch (tutorialId) {
                case 1:
                    if (m_gm.playerPopupID == tutorialId) {
                        if (!m_player.usingController) {
                            popup = Instantiate(m_gm.TutorialsKeyboard[2]);
                        }
                        else {
                            popup = Instantiate(m_gm.TutorialsController[2]);
                        }
                        popup.transform.GetChild(0).gameObject.GetComponent<ScreenFader>().FadeOn();
                        StartCoroutine(nextTutorial(3));
                        m_gm.playerPopupID++;
                    }
                    break;

            }
        }


        if (other.tag == "Player" && SceneManager.GetActiveScene().buildIndex == 6) {

            switch (tutorialId) {
                case 1:
                    if (m_gm.playerPopupID == tutorialId) {
                        if (!m_player.usingController) {
                            popup = Instantiate(m_gm.TutorialsKeyboard[4]);
                        }
                        else {
                            popup = Instantiate(m_gm.TutorialsController[4]);
                        }
                        popup.transform.GetChild(0).gameObject.GetComponent<ScreenFader>().FadeOn();
                        m_gm.playerPopupID++;
                        StartCoroutine(nextTutorial(4));
                        StartCoroutine(nextTutorialLvl6(5));
                    }
                    break;

            }
        }



    }

    IEnumerator DestroyMovementPopup() {
        yield return new WaitForSeconds(deletePopup.timeToFade);
        Destroy(deletePopup);
    }

    IEnumerator nextTutorial(int i) {
        yield return new WaitForSeconds(2f);

        deletePopup = Object.FindObjectOfType<ScreenFader>().GetComponent<ScreenFader>();
        deletePopup.FadeOff();
        StartCoroutine(DestroyMovementPopup());

        yield return new WaitForSeconds(2f);

        if (!m_player.usingController) {
            popup = Instantiate(m_gm.TutorialsKeyboard[i]);
        }
        else {
            popup = Instantiate(m_gm.TutorialsController[i]);
        }
        popup.transform.GetChild(0).gameObject.GetComponent<ScreenFader>().FadeOn();

        yield return new WaitForSeconds(2f);

        deletePopup = Object.FindObjectOfType<ScreenFader>().GetComponent<ScreenFader>();
        deletePopup.FadeOff();
        StartCoroutine(DestroyMovementPopup());
    }

    IEnumerator nextTutorialLvl6(int i) {
        yield return new WaitForSeconds(2f);

        if (!m_player.usingController) {
            popup = Instantiate(m_gm.TutorialsKeyboard[i]);
        }
        else {
            popup = Instantiate(m_gm.TutorialsController[i]);
        }
        popup.transform.GetChild(0).gameObject.GetComponent<ScreenFader>().FadeOn();

        yield return new WaitForSeconds(2f);

        deletePopup = Object.FindObjectOfType<ScreenFader>().GetComponent<ScreenFader>();
        deletePopup.FadeOff();
        StartCoroutine(DestroyMovementPopup());
    }
}
