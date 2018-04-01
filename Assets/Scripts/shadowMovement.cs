using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowMovement : MonoBehaviour {

  public GameObject player;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
    Vector3 newpos = this.transform.position;
    newpos.x = player.transform.position.x;
    this.transform.position = newpos;
  }
}
