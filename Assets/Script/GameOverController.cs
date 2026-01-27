using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Image fadeImage;
    bool isGameOver = false;

    public void PlayGameOver()
    {
       if (isGameOver) return;
       isGameOver = true;
        //フェードイン
        fadeImage.DOFade(1f, 0.5f);
        //少し待ってリロード
        Invoke(nameof(Reload), 1.2f);
    }

    void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
