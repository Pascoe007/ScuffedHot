using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public CharacterController controller;
    public Rigidbody rb;

    private bool doSlowMo = false;

    public float Speed = 6f;
    public float Gravity = -9.81f;
    bool grounded;
    Vector3 velocity;
    float jump = 3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

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

        SlowMo();


    }

    void SlowMo()
    {
        float playerVelocity = rb.velocity.magnitude;
        Debug.Log(Time.timeScale);
        Debug.Log(doSlowMo);
        Debug.Log(playerVelocity);
        if (playerVelocity > 0)
        {
            if (doSlowMo)
            {
                
                
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                doSlowMo = false;

            }
        }
        else
        {
            if (!doSlowMo)
            {
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                doSlowMo = true;

            }
        }
    }
}
