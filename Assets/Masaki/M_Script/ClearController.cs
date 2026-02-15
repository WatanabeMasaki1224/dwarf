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
    public GameObject[] stopCats;
    public AudioSource audioSource;
    public AudioClip clearBGM;
    public AudioClip clickSE;
    public Image clickFadeImage;
    
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
        foreach(var cat  in stopCats)
        {
            if(cat!=null)
            {
                var catcr = cat.GetComponent<CatController>();
                if (catcr != null)
                {
                    catcr.StopCat(); 
                }
            }
        }
        if(clearBGM != null)
        {
            audioSource.clip = clearBGM;
            audioSource.Play();
        }
        // ★マウス解放（超重要）  これをしないとマウス操作が反応しない
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // ① 画面を暗くする（先に見せる）
        fadeImage.DOFade(1f, 0.5f).OnComplete(() =>
        {
            // ② 少し間を作る
            DOVirtual.DelayedCall(0.2f, () =>
            {
                // ③ CLEAR文字出現（小→大→適正）
                clearText.alpha = 1;
                clearText.transform.localScale = Vector3.zero;

                clearText.transform.DOScale(1.4f, 0.35f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        clearText.transform.DOScale(1.0f, 0.15f);
                    });

                // ④ ボタン表示（少し遅らせる）
                retryButton.transform.DOScale(1f, 0.3f).From(0f).SetDelay(0.6f);
                titleButton.transform.DOScale(1f, 0.3f).From(0f).SetDelay(0.7f);
            });
        });
    }

    public void Retry()
    {
        audioSource.PlayOneShot(clickSE);
        clickFadeImage.DOFade(1f, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void BackToTitle()
    {
        audioSource.PlayOneShot(clickSE);
        clickFadeImage.DOFade(1f, 1f).OnComplete(() =>
        {
            SceneManager.LoadScene("Title");
        });
    }
}
