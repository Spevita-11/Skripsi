using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;
    [SerializeField] private Button level3Button;

    private void Awake()
    {
        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(QuitGame);

        level1Button.onClick.RemoveAllListeners();
        level1Button.onClick.AddListener(() => LoadLevel("Level 1"));

        level2Button.onClick.RemoveAllListeners();
        level2Button.onClick.AddListener(() => LoadLevel("Level 2"));

        level3Button.onClick.RemoveAllListeners();
        level3Button.onClick.AddListener(() => LoadLevel("Level 3"));

        InitLevelSelection();
    }

    private void Start()
    {
        AudioManager.Instance.PlayBGM("MainMenu");
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    private void InitLevelSelection()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        switch (levelReached)
        {
            case 1:
                level1Button.interactable = true;
                level2Button.interactable = false;
                level3Button.interactable = false;
                break;
            case 2:
                level1Button.interactable = true;
                level2Button.interactable = true;
                level3Button.interactable = false;
                break;
            case 3:
                level1Button.interactable = true;
                level2Button.interactable = true;
                level3Button.interactable = true;
                break;
        }
    }
}
