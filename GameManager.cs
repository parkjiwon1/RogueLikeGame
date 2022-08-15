using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOption;

    public GameObject gameOver;

    public GameObject PotionFactory;

    public GameObject PotalFactory;
    public void CloseOptionWindow()
    {
        gameOption.SetActive(false);

        Time.timeScale = 1f;
    }
    public void OnClickNewGame()
    {
        SceneManager.LoadScene("MainStage");
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
