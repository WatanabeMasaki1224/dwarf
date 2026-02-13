using System.Collections;
using System.Collections.Generic;
using System.Data;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerContollore : MonoBehaviour
{
    public float moveSpeed  = 3f;
    public float gravity = -9.8f;
    private CharacterController controller;
    private Vector3 velocity;
    public bool isHidden { get; private set; }  //ステルス中か判定
    public GameObject soundItem;
    public float placeDistance = 1f; //アイテムをおく距離
    public bool hasSoundItem = false;
    public int itemCount = 0;
    public int needItemCount = 3;
    public TextMeshProUGUI goalItemCountTxet; //アイテム数のテクスト
    public GameObject soundItemUI;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GoalItemUI();
        soundItemUI.SetActive(false);
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        // カメラ基準の方向
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 move = camRight * x + camForward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);   
        if(controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.F) && hasSoundItem)
        {
            PlaceSoundItem();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("HideArea"))
        {
            isHidden = true;
            Debug.Log("isHidden = true");
        }

        if(other.CompareTag("PickupSoundItem"))
        {
            hasSoundItem = true;
            SoundItemUI();
            Destroy(other.gameObject);
            Debug.Log("SoundItemGET");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("HideArea"))
        {
            isHidden = false;
            Debug.Log("isHidden = false;");
        }
    }

    void PlaceSoundItem()
    {
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;         
        camForward.Normalize();

        Vector3 pos = transform.position + camForward * placeDistance;
        Instantiate(soundItem,pos,Quaternion.identity);
        hasSoundItem = false;
        SoundItemUI();
    }

    public void GoalItemUI()
    {
        if(goalItemCountTxet != null)
        {
            goalItemCountTxet.text = $"{itemCount}/{needItemCount}";
        }
    }

    public void SoundItemUI()
    {
        if(soundItemUI != null)
        {
            soundItemUI.SetActive(hasSoundItem);
        }
    }

}
