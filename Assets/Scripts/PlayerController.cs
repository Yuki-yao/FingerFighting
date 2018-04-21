using System.Collections;
using System.Collections.Generic;
using FingerInput;
using UnityEngine;
using Leap.Unity;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

  private Rigidbody2D body;
  private SpriteRenderer sRenderer;
  public GameObject shadow;
  public PlayerController otherPlayer;
  public Slider HPBar;
  private IFingerInput input;
  private Animator animator;
  private float groundY;
  public float speed;
  public bool isHandInput;
  private ActionType curAction;
  private int player;
  private int hp;
  private bool isAlive;

	// Use this for initialization
	void Start () {
    curAction = ActionType.Null;
    body = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    sRenderer = shadow.GetComponent<SpriteRenderer>();
    if(this.name == "Player1")
    {
      player = 1;
    }
    else
    {
      player = 2;
      speed = -speed;
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
    hp = 100;
    HPBar.value = HPBar.maxValue = hp;
    isAlive = true;
	}

  void FixedUpdate() {
    if (!isAlive)
      return;
    Vector2 pos = body.position;
    ActionType at = input.GetAction();
    if (OnAir())
    {
      if (player == 1)
        Debug.Log(System.Environment.TickCount);
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
      animator.ResetTrigger("backward");
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
    curAction = at;
    if (-2.0 < otherPlayer.body.position.x - pos.x && otherPlayer.body.position.x - pos.x < 2.0)
      checkAttack();
  }

  // Update is called once per frame
  void Update () {
	}

  public ActionType GetCurrentAction()
  {
    return curAction;
  }

  public bool OnAir()
  {
    if (body.position.y > groundY + 1e-4)
      return true;
    return false;
  }

  public bool DecreaseHP()
  {
    if (player == 2)
      Debug.Log("got hit");
    if (hp <= 0) return false;
    hp -= 1;
    if (hp <= 0)
    {
      HPBar.value = 0;
      return true;
    }
    HPBar.value = hp;
    return false;
  }

  void checkAttack()
  {
    // TODO: 
    // if (player1 and player2 collide and not on air)
    //   if ((player1 punch and player2 not (defend or squat)) or player1 kick)
    // player2 hp decreased
    // if player2's hp == 0
    //   player2 die
    //
    // HINT:
    // use OnAir() to check if player is on air
    // use otherPlayer to access another player Gameobject
    // use curAction to access this player's action
    // use otherPlayer.GetCurrentAction() to access another player's action
    // you can access health bar like "GameObject.Find("YOUR_HEALTH_BAR_NAME") or declare your own variables
    //   e.g. public Slider healthBar;
    if(!this.OnAir() && !otherPlayer.OnAir())
    {
      ActionType otherAct = otherPlayer.GetCurrentAction();
      if((otherAct == ActionType.Punch && curAction != ActionType.Defend && curAction != ActionType.Squat) || otherAct == ActionType.Kick)
      {
        if(DecreaseHP())
        {
          animator.SetBool("dead", true);
          animator.SetBool("isIdle", false);
          isAlive = false;
        }
      }
    }
  }
}
