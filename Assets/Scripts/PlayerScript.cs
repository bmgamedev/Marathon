﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {

    public float Speed = 10.0f;
    public float Jump = 0.3f;
    public LayerMask GroundLayer;

    private Animator animator;
    private Transform groundCheck;

    private Rigidbody2D rb2d;

    private bool isGrounded;



    private string nextStartPos;
    private GameObject connectingDoor;
    private Vector3 defaultStartPos = new Vector3(-4.58f, 0.78f, 0.0f);
    static PlayerScript instance = null;


    void Start()
    {
        animator = GetComponent<Animator>();
        groundCheck = transform.Find("GroundCheck");
        rb2d = GetComponent<Rigidbody2D>();



        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        gameObject.transform.position = defaultStartPos;
        print(gameObject.transform.position);
    }

    void FixedUpdate()
    {
        bool isGrounded = Physics2D.OverlapPoint(groundCheck.position, GroundLayer);

        if (Input.GetButton("Jump"))
        {
            if (isGrounded)
            {
                rb2d.AddForce(Vector2.up * Jump, ForceMode2D.Impulse);
            }
        }

        animator.SetBool("isGrounded", isGrounded);

        float hSpeed = Input.GetAxis("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(hSpeed));

        if (hSpeed > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (hSpeed < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        this.GetComponent<Rigidbody2D>().velocity = new Vector2(hSpeed * Speed, this.GetComponent<Rigidbody2D>().velocity.y);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (isGrounded)
            {
                isGrounded = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            SceneManager.LoadScene("Captured");
        }

        if (collision.GetComponent<Collider2D>().tag == "Door")
        {
            nextStartPos = SceneManager.GetActiveScene().name; // = name of scene being left
            print(nextStartPos);
        }
    }





    void OnLevelWasLoaded()
    {
        /* -----------------------------------------------------------------------------------------------------------------------------
         * find a gameobject in the level that has the name of the next starting position (if null, set starting position as default),
         * get the starting position associated with that gameobject:
         * i.e. 	Player X = find middle of Door X 
         * 				Y = remains the same
         * 				Z = remains the same
         * -----------------------------------------------------------------------------------------------------------------------------
         */

        connectingDoor = (GameObject.Find(nextStartPos));

        if (connectingDoor)
        {
            print(nextStartPos + connectingDoor.transform.position);
            gameObject.transform.position = new Vector3(connectingDoor.transform.position.x, 0.78f, 0.0f);
        }
        else
        {
            print("no object to copy position of");
            gameObject.transform.position = defaultStartPos;
        }
    }



}