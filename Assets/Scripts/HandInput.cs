using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FingerInput;
using Leap.Unity;

public class HandInput : IFingerInput {

  public LeapHandController handController;
  int player;

  public HandInput(int player)
  {
    this.player = player;
  }

  public ActionType GetAction()
  {
    return handController.GetAction(player);
  }

  public void SetHandController(LeapHandController hc)
  {
    handController = hc;
  }
}
