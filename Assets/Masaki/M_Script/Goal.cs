using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    bool isClear;
    ClearController clearController;

    private void Start()
    {
        clearController = GetComponent<ClearController>();
    }

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
        if (clearController != null)
        {
            clearController.PlayClear();
        }
        else
        {
            Debug.LogError("ClearController ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
        }
    }
}
