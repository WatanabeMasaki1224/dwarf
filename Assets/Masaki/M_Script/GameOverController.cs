using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class GameOverController : MonoBehaviour
{
    public Image fadeImage;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button retryButton;
    public Button titleButton;
    bool isGameOver = false;
    public GameObject[] stopCats;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        fadeImage.color = new Color(0, 0, 0, 0);
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
        foreach (var cat in stopCats)
        {
            if (cat != null)
            {
                var catcr = cat.GetComponent<CatController>();
                if (catcr != null)
                {
                    catcr.StopCat(); // ★これだけでOK
                }
            }
        }
        
        fadeImage.DOFade(1f, 1f);
        StartCoroutine(TypeGameOver());
    }

    IEnumerator TypeGameOver()
    {
        yield return new WaitForSeconds(1f);
        gameOverText.DOFade(0.8f, 1f); // じわっと表示
        // ④ 文字を少し震わせる（ホラー）
        gameOverText.transform.DOShakePosition(5f, 8f, 20);

        yield return new WaitForSeconds(1.0f);

        // ⑤ ボタン表示
        retryButton.transform.DOScale(1f, 0.3f).From(0f);
        titleButton.transform.DOScale(1f, 0.3f).From(0f);
    }

    public void Retry()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("リトライ");
    }

    public void BackToTitle()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("Title");
        Debug.Log("スタートへ");
    }
}
