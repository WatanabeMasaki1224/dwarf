using System.Collections;
using System.Collections.Generic;
using System.Data;
using Cinemachine;
using UnityEngine;

public class PlayerContollore : MonoBehaviour
{
    public float moveSpeed  = 3f;
    public float gravity = -9.8f;
    private CharacterController controller;
    private Vector3 velocity;
    public bool isHidden { get; private set; }  //ステルス中か判定

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("HideArea"))
        {
            isHidden = true;
            Debug.Log("isHidden = true");
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

}
