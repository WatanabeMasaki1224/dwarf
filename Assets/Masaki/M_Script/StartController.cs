using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    public string gameSceneName = "1stStage";
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
