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
    public AudioSource audioSource;
    public AudioClip clickSE;
    public AudioClip gameOverBGM;
    public Image clickFadeImage;
    public AudioSource stageBGM;

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
        //プレイヤー操作停止
        PlayerContollore player = FindObjectOfType<PlayerContollore>();
        player.enabled = false;
        //マウス解放（超重要）  これをしないとマウス操作が反応しない
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        foreach (var cat in stopCats)
        {
            if (cat != null)
            {
                var catcr = cat.GetComponent<CatController>();
                if (catcr != null)
                {
                    catcr.StopCat(); 
                }
            }
        }

        if (gameOverBGM != null)
        {
            audioSource.clip = gameOverBGM;
            audioSource.Play();
        }
        
        stageBGM.Stop();
        fadeImage.DOFade(1f, 1f);
        StartCoroutine(TypeGameOver());
    }

    IEnumerator TypeGameOver()
    {
        yield return new WaitForSeconds(1f);
        gameOverText.DOFade(0.8f, 1f); // じわっと表示
        //文字を少し震わせる
        gameOverText.transform.DOShakePosition(5f, 8f, 20);
        yield return new WaitForSeconds(1.0f);
        retryButton.transform.DOScale(1f, 0.3f).From(0f);
        titleButton.transform.DOScale(1f, 0.3f).From(0f);
    }

    public void Retry()
    {
        audioSource.PlayOneShot(clickSE);
        DOTween.KillAll();
        clickFadeImage.DOFade(1f, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void BackToTitle()
    {
        audioSource.PlayOneShot(clickSE);
        DOTween.KillAll();
        clickFadeImage.DOFade(1f, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene("Title");
        });
        Debug.Log("スタートへ");
    }
}
