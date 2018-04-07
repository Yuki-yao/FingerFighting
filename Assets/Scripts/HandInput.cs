using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FingerInput;
using Leap.Unity;

public class HandInput : IFingerInput {

  public LeapHandController handController;

  public ActionType GetAction()
  {
    return handController.GetAction();
  }

  public void SetHandController(LeapHandController hc)
  {
    handController = hc;
  }
}
