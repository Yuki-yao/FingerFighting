using UnityEngine;
using System;
using FingerInput;
using Leap.Unity;

public class KeyboardInput : IFingerInput{
  public int[] lasTime;
  public KeyCode[] keys;
  private int player;

  public KeyboardInput(int player)
  {
    this.player = player;
    if(player == 1)
    {
      keys = new KeyCode[8]
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
    }
    else
    {
      keys = new KeyCode[8]
      {
        KeyCode.None,
        KeyCode.RightShift,
        KeyCode.UpArrow,
        KeyCode.RightArrow,
        KeyCode.Minus,
        KeyCode.LeftArrow,
        KeyCode.DownArrow,
        KeyCode.Equals
      };
    }
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