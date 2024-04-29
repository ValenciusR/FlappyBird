using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CodeMonkey;
using System.Diagnostics;

public class Bird : MonoBehaviour
{
    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;

    private static Bird instance;

    public static Bird GetInstance() {  return instance; }

    private const float JUMP_VALUE = 100f;
    private Rigidbody2D BirdRB;
    private State state;
    private PlayerInput playerInput;
    private BirdInputAction inputActions;

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead
    }

    private void Awake()
    {
        instance = this;
        BirdRB = GetComponent<Rigidbody2D>();
        BirdRB.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
        playerInput = GetComponent<PlayerInput>();

        inputActions = new BirdInputAction();
        inputActions.Bird.Enable();
        inputActions.Bird.Jump.performed += Bird_Jump;
        OnDied += Bird_OnDied;
    }

    private void Bird_OnDied(object sender, EventArgs e)
    {
        state = State.Dead;
        inputActions.Bird.Disable();
    }

    private void Bird_Jump(InputAction.CallbackContext context)
    {
        //UnityEngine.Debug.Log(context);
        if(context.performed)
        {
            switch (state)
            {
                default:
                    break;
                case State.WaitingToStart:

                    state = State.Playing;
                    BirdRB.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);

                    break;
                case State.Playing:
                    Jump();
                    break;
                case State.Dead:
                    break;
            }
        }
    }

    private void Update()
    {
        /*switch(state){
            default:
                break;
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    state = State.Playing;
                    BirdRB.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    Jump();
                }
                break;
            case State.Dead:
                break;
        }*/
        
    }

    private void Jump()
    {
        BirdRB.velocity = Vector2.up * JUMP_VALUE;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BirdRB.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
}
