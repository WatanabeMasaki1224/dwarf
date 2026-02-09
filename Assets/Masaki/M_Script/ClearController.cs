using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ClearController : MonoBehaviour
{
    public Image fadeImage;
    public GameObject clearPanel;
    public TextMeshProUGUI clearText;
    public Button retryButton;
    public Button titleButton;

    bool isClear = false;

    void Start()
    {
        clearPanel.SetActive(false);
        fadeImage.color = new Color(0, 0, 0, 0);
        clearText.alpha = 0;

        retryButton.transform.localScale = Vector3.zero;
        titleButton.transform.localScale = Vector3.zero;
    }

    public void PlayClear()
    {
        if (isClear) return;
        isClear = true;
        clearPanel.SetActive(true);
        // ★プレイヤー操作停止
        PlayerContollore player = FindObjectOfType<PlayerContollore>();
        player.enabled = false;
        // ★マウス解放（超重要）  これをしないとマウス操作が反応しない
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        fadeImage.DOFade(0.7f, 0.5f);
        clearText.DOFade(1f, 0.5f).SetDelay(0.2f);
        clearText.transform.DOScale(1f, 0.3f).From().SetDelay(0.2f);
        retryButton.transform.DOScale(1f, 0.3f).From(0f).SetDelay(0.6f);
        titleButton.transform.DOScale(1f, 0.3f).From(0f).SetDelay(0.7f);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
