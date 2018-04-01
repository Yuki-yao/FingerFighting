using System.Collections;
using System.Collections.Generic;
using FingerInput;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  private Rigidbody2D body;
  private SpriteRenderer sRenderer;
  public GameObject shadow;
  private IFingerInput input;
  private Animator animator;
  private float groundY;
  public float speed;

	// Use this for initialization
	void Start () {
    body = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    sRenderer = shadow.GetComponent<SpriteRenderer>();
    input = new KeyboardInput();
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
  }

  // Update is called once per frame
  void Update () {
		
	}
}
