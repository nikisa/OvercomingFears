using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public int tutorialId;

    GameManager m_gm;

    ScreenFader deletePopup;

    GameObject popup;

    private void Awake() {
        m_gm = FindObjectOfType<GameManager>().GetComponent<GameManager>();        
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
                        popup = Instantiate(m_gm.TutorialsKeyboard[1]);
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
                        popup = Instantiate(m_gm.TutorialsKeyboard[2]);
                        popup.transform.GetChild(0).gameObject.GetComponent<ScreenFader>().FadeOn();
                        m_gm.playerPopupID++;
                    }
                    break;
                    
            }
        }


        if (other.tag == "Player" && SceneManager.GetActiveScene().buildIndex == 6) {

            switch (tutorialId) {
                case 1:
                    if (m_gm.playerPopupID == tutorialId) {
                        popup = Instantiate(m_gm.TutorialsKeyboard[4]);
                        popup.transform.GetChild(0).gameObject.GetComponent<ScreenFader>().FadeOn();
                        m_gm.playerPopupID++;
                    }
                    break;

            }
        }



    }

    IEnumerator DestroyMovementPopup() {
        yield return new WaitForSeconds(deletePopup.timeToFade);
        Destroy(deletePopup);
    }
    
}
