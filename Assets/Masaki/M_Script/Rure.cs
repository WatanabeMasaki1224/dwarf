using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rure : MonoBehaviour
{
    public GameObject rulePanel;
    public AudioSource audioSource;
    public AudioClip clickSE;

    public void Start()
    {
        rulePanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }
    public void OpenRule()
    {
        audioSource.PlayOneShot(clickSE);
        rulePanel.SetActive(true);
    }

    public void CloseRule()
    {
        audioSource.PlayOneShot(clickSE);
        rulePanel.SetActive(false);
    }
}
