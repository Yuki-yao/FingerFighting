using UnityEngine;
using FingerInput;

public class KeyboardInput : IFingerInput{
  public ActionType GetAction()
  {
    if (Input.GetKey(KeyCode.D))
      return ActionType.Forward;
    return ActionType.Null;
  }
}