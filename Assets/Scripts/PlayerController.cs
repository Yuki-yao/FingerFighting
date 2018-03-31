using System.Collections;
using System.Collections.Generic;
using FingerInput;
using UnityEngine;

public class PlayerController : MonoBehaviour {

  private Rigidbody2D body;
  private IFingerInput input;
  public float speed;

	// Use this for initialization
	void Start () {
    body = GetComponent<Rigidbody2D>();
    input = new KeyboardInput();
	}

  void FixedUpdate() {
    Vector2 pos = body.position;
    ActionType at = input.GetAction();
    if(at == ActionType.Forward)
    {
      Vector2 newpos = new Vector2(pos.x + speed, pos.y);
      body.MovePosition(newpos);
    }
  }

  // Update is called once per frame
  void Update () {
		
	}
}
