﻿using System.Collections;
using System.Collections.Generic;
using FingerInput;
using UnityEngine;
using Leap.Unity;

public class PlayerController : MonoBehaviour {

  private Rigidbody2D body;
  private SpriteRenderer sRenderer;
  public GameObject shadow;
  private IFingerInput input;
  private Animator animator;
  private float groundY;
  public float speed;
  public bool isHandInput;

	// Use this for initialization
	void Start () {
    body = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    sRenderer = shadow.GetComponent<SpriteRenderer>();
    int player = 0;
    if(this.name == "Player1")
    {
      player = 1;
    }
    else
    {
      player = 2;
    }
    if(isHandInput)
    {
      input = new HandInput(player);
    }
    else
    {
      input = new KeyboardInput(player);
    }
    input.SetHandController(GameObject.Find("LeapHandController").GetComponent<LeapHandController>());
    groundY = body.position.y;
	}

  void FixedUpdate() {
    Vector2 pos = body.position;
    ActionType at = input.GetAction();
    if (pos.y > groundY)
    {
      sRenderer.enabled = true;
      return;
    }
    sRenderer.enabled = false;

    if (at == ActionType.Forward)
    {
      animator.SetTrigger("forward");
      animator.SetBool("isIdle", false);
      Vector2 newpos = new Vector2(pos.x + speed, pos.y);
      body.MovePosition(newpos);
    }
    else if (at == ActionType.Backward)
    {
      Debug.Log("Backward");
      animator.SetTrigger("backward");
      animator.SetBool("isIdle", false);
      Vector2 newpos = new Vector2(pos.x - speed, pos.y);
      body.MovePosition(newpos);
    }
    else if (at == ActionType.Jump)
    {
      animator.SetTrigger("jump");
      animator.SetBool("isIdle", false);
      Vector2 upforce = new Vector2(0, 1300);
      body.AddForce(upforce);
    }
    else if (at == ActionType.Squat)
    {
      animator.SetTrigger("squat");
      animator.SetBool("isIdle", false);
    }
    else if (at == ActionType.Punch)
    {
      animator.SetTrigger("punch");
      animator.SetBool("isIdle", false);
    }
    else if (at == ActionType.Kick)
    {
      animator.SetTrigger("kick");
      animator.SetBool("isIdle", false);
    }
    else if (at == ActionType.Defend)
    {
      animator.SetTrigger("defend");
      animator.SetBool("isIdle", false);
    }
    else
    {
      animator.SetBool("isIdle", true);
    }
    checkAttack(at);
  }

  // Update is called once per frame
  void Update () {
	}

  void checkAttack(ActionType at)
  {
    // TODO: 
    // if (player1 and player2 collide and not on air)
    //   if (player1 punch and player2 not (defend or squat))
    //   or player1 kick
    // player2 hp decreased
  }
}
