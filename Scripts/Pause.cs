using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;


public class Pause : MonoBehaviour
{
    public GameObject pauseOverlay;
    public GameObject lobby;
    private void Start()
    {
        if (pauseOverlay == null)
        {
            Debug.LogError("Could not find the pause screen");
        }
        else
        {
            pauseOverlay.SetActive(false);
        }
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !lobby.activeSelf)
        {
            Debug.Log("Pressed escape");
            pause();
        }
    }

    public void pause()
    {
        if (pauseOverlay.activeSelf)
        {
            pauseOverlay.SetActive(false);
        }
        else
        {
            pauseOverlay.SetActive(true);
        }
    }

    public void exitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void loadGameScene()
    {
        SceneManager.LoadScene("GuessWhoMap");
    }

    public void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      UnityEngine.Application.Quit();
#endif
    }
}
