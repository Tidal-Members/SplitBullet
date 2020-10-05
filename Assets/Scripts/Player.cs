﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-100)]
public class Player : Entity
{
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    public float zPosition = 0f;
    public bool noClip = false;
    public bool flying = false;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private Transform lookAtObject;
    /*void Awake()
    {
        ticks += Tick;
        ticksStart += TickStart;
    }*/
    void Start()
    {
        characterController = transform.GetComponent<CharacterController>();
        lookAtObject = transform.Find("Look");
        CommandBackend.AddConCommand("noclip",(string[] args) =>
        {
            noClip = !noClip;
            flying = noClip;
            CommandBackend.IncreaseOutputSize(CommandBackend.line);
            return "NoClip = "+noClip+"\nFlying = "+flying;
        }, "Toggle player collisions and flying.");
        CommandBackend.AddConCommand("fly",(string[] args) =>
        {
            flying = !flying;
            return "Flying = "+flying;
        }, "Toggle player flying.");
    }
    // Update is called once per frame
    void Update()
    {
        if(CommandBackend.currentlyActive)
            return;
        if (health > 0 && !flying)
        {
            groundedPlayer = characterController.isGrounded;
            if (groundedPlayer)
            {
                playerVelocity.y = 0f;
            }

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
            characterController.Move(move * Time.deltaTime * playerSpeed);
            if (transform.position.z != zPosition)
            {
                move.z = (zPosition - transform.position.z);
            }
            if (Input.GetButton("Jump") && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }
            playerVelocity.y += gravityValue * Time.deltaTime;
            characterController.Move(playerVelocity * Time.deltaTime);
        }
        else if(flying)
        {
            Vector3 movingTo = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            if(noClip)
                transform.position = new Vector3(transform.position.x+Input.GetAxis("Horizontal"),transform.position.y+Input.GetAxis("Vertical"));
            else
                characterController.Move(moveDirection * Time.deltaTime);
        }
        //mouse look vvv

        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (lookAtObject.position);

        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //Get the angle between the points
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
        if(angle > 25f && angle < 90f)
            angle = 25f;
        else if(angle < 150f && angle > 90f)
            angle = 150f;
        //Ta Daaa
        lookAtObject.rotation =  Quaternion.Euler(new Vector3(0f,0f,angle));
    }
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }
}
