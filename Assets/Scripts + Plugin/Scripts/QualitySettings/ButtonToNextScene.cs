using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonToNextScene : MonoBehaviour
{
    public string sceneName;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(new UnityAction(LoadScene));
    }

    void LoadScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
