using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Image fadeImage;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button retryButton;
    public Button titleButton;
    bool isGameOver = false;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        fadeImage.color = new Color(0,0,0,0);
        gameOverText.alpha = 0;
        retryButton.transform.localScale = Vector3.zero;
        titleButton.transform.localScale = Vector3.zero;
    }
    public void PlayGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        gameOverPanel.SetActive(true);
        // ★プレイヤー操作停止
        PlayerContollore player = FindObjectOfType<PlayerContollore>();
        player.enabled = false;

        // ★マウス解放（超重要）  これをしないとマウス操作が反応しない
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fadeImage.DOFade(1f, 0.5f); //フェード
        gameOverText.DOFade(1f, 0.5f).SetDelay(0.3f); //gameOverの文字
        gameOverText.transform.DOScale(1f,0.3f).From().SetDelay(0.3f);
        retryButton.transform.DOScale(1f,0.3f).From(0f).SetDelay(0.7f);
        titleButton.transform.DOScale(1f, 0.3f).From(0f).SetDelay(0.8f);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("リトライ");
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
        Debug.Log("スタートへ");
    }
}
