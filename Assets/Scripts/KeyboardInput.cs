using UnityEngine;
using System;
using FingerInput;
using Leap.Unity;

public class KeyboardInput : IFingerInput{
  public int[] lasTime;
  public KeyCode[] keys =
  {
    KeyCode.None,
    KeyCode.LeftShift,
    KeyCode.W,
    KeyCode.A,
    KeyCode.J,
    KeyCode.D,
    KeyCode.S,
    KeyCode.K
  };

  public KeyboardInput()
  {
    lasTime = new int[8];
    for(int i = 0; i < lasTime.Length; i ++)
    {
      lasTime[i] = System.Environment.TickCount;
    }
  }

  public ActionType GetAction()
  {
    for(int i = 1; i < 8; i ++)
    {
      if (Input.GetKey(keys[i]))
      {
        /*
        if(i == (int)ActionType.Jump)
        {
          if(System.Environment.TickCount - lasTime[i] < 300)
          {
            lasTime[i] = System.Environment.TickCount;
            return ActionType.Null;
          }
        }
        */
        lasTime[i] = System.Environment.TickCount;
        return (ActionType)i;
      }
    }
    lasTime[0] = System.Environment.TickCount;
    return ActionType.Null;
  }

  public void SetHandController(LeapHandController hc)
  {

  }
}