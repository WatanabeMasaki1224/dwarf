using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    public string gameSceneName = "1stStage";
    public float fadeTime = 1.0f;
    public Image fadeImage;
    public AudioSource audioSource;
    public AudioClip mouseSE;
    public AudioClip clickSE;
    bool starting = false;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void StartGame()
    {
        if (starting) return;
        starting = true;
        fadeImage.color = new Color(0, 0, 0, 0);
        audioSource.PlayOneShot(clickSE);
        Sequence seq = DOTween.Sequence();
        seq.Append(fadeImage.DOFade(1f, 1f));
        seq.InsertCallback(0.5f, () =>
        {
            audioSource.PlayOneShot(mouseSE);
        });
        seq.OnComplete(() =>
        {
            SceneManager.LoadScene(gameSceneName);
        });
    }
}
