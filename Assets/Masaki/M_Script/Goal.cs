using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public Image fadeImage;
    bool isClear;

    private void OnTriggerEnter(Collider other)
    {
        if(isClear) return;
        if(other.CompareTag("Player"))
        {
            PlayerContollore pc = other.GetComponent<PlayerContollore>();
            if (pc.itemCount >= pc.needItemCount)
            {
                isClear = true;
                PlayClear();
            }
            else
            {
                Debug.Log($"‚ ‚Æ{pc.needItemCount - pc.itemCount}ŒÂ•K—v");
            }
        }
    }

    void PlayClear()
    {
        if(fadeImage != null)
        {
            fadeImage.DOFade(1f, 0.5f);
        }
        Debug.Log("CLEAR");
    }
}
