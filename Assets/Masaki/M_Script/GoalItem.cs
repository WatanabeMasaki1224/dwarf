using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerContollore pc =other.GetComponent<PlayerContollore>();
            pc.itemCount++;
            pc.GoalItemUI();
            Debug.Log("ƒAƒCƒeƒ€Š“¾"+ pc.itemCount);
            Destroy(gameObject);
        }
    }
}
