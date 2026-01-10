using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayerContollore : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpPower = 5f;
    public float gravity = -9.8f;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveInput;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        Jump();
        Gravity();
        Vector3 totalMove = moveInput * moveSpeed + velocity;
        controller.Move(totalMove * Time.deltaTime);
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveInput = new Vector3(h,0, v);
    }

    private void Jump()
    {
        if(controller.isGrounded)
        {
            if(velocity.y < 0)
            {
                velocity.y = 2f;
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpPower;
            }
        }
    }

    private void Gravity()
    {
        velocity.y = gravity * Time.deltaTime;
    }
}
