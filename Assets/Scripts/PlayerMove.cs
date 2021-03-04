using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;

    public float Speed = 6f;
    public float Gravity = -9.81f;
    bool grounded;
    Vector3 velocity;
    float jump = 3f;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * Speed * Time.deltaTime);

        velocity.y += Gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        grounded = controller.isGrounded;
        if (grounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y += Mathf.Sqrt(jump * -3.0f * Gravity);
        }

        
    }
}
