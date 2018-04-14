/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine.UI;
using FingerInput;

namespace Leap.Unity
{
  /**
   * LeapHandController uses a Factory to create and update HandRepresentations based on Frame's received from a Provider  */
  public class LeapHandController : MonoBehaviour
  {
    protected LeapProvider provider;

    private const int TYPES = 8;
    private List<List<double>> lcount = new List<List<double>>();
    private List<List<double>> rcount = new List<List<double>>();
    private List<double> ltotalCount = new List<double>();
    private List<double> rtotalCount = new List<double>();

    private ActionType laction;
    private ActionType raction;

    protected virtual void OnEnable()
    {
      provider = requireComponent<LeapProvider>();

      provider.OnUpdateFrame += OnUpdateFrame;
      provider.OnFixedFrame += OnFixedFrame;

      laction = ActionType.Null;
      raction = ActionType.Null;

      for (int i = 0; i < TYPES; i++)
      {
        List<double> lnewList = new List<double>();
        lcount.Add(lnewList);
        List<double> rnewList = new List<double>();
        rcount.Add(rnewList);
      }
    }

    protected virtual void OnDisable()
    {
      provider.OnUpdateFrame -= OnUpdateFrame;
      provider.OnFixedFrame -= OnFixedFrame;
    }

    /** Updates the graphics HandRepresentations. */
    protected virtual void OnUpdateFrame(Frame frame)
    {
      if (frame != null)
      {

        //Debug.Log(frame.CurrentFramesPerSecond);

        //Update label text.
        if (frame.Hands.Count > 0)
        {
          Hand curHand;
          for(int i = 0; i < frame.Hands.Count; i ++)
          {
            curHand = frame.Hands[i];
            if (curHand.IsLeft)
              UpdateGesture(curHand, true);
            else
              UpdateGesture(curHand, false);
          }
        }
        else
        {
          laction = ActionType.Null;
          raction = ActionType.Null;
        }
      }
    }

    /** Updates the physics HandRepresentations. */
    protected virtual void OnFixedFrame(Frame frame)
    {

    }

    private T requireComponent<T>() where T : Component
    {
      T component = GetComponent<T>();
      if (component == null)
      {
        string componentName = typeof(T).Name;
        Debug.LogError("LeapHandController could not find a " + componentName + " and has been disabled.  Make sure there is a " + componentName + " on the same gameObject.");
        enabled = false;
      }
      return component;
    }


    enum GestureType
    {
      Defend, Jump, Backward, Punch, ForwardL, ForwardR, Squat, Kick, Null
    }

    private bool isSquat(Hand hand)
    {
      float angle = 0;
      for (int i = 1; i < 5; i++)
      {
        Finger finger = hand.Fingers[i];
        Vector interDir = finger.bones[(int)Bone.BoneType.TYPE_INTERMEDIATE].Direction;
        Vector proxDir = finger.bones[(int)Bone.BoneType.TYPE_PROXIMAL].Direction;
        Vector metaDir = finger.bones[(int)Bone.BoneType.TYPE_METACARPAL].Direction;
        float angle_ip = interDir.AngleTo(proxDir) * Mathf.Rad2Deg;
        float angle_pm = proxDir.AngleTo(metaDir) * Mathf.Rad2Deg;
        angle += angle_ip + angle_pm;
      }
      angle /= 8;
      //Debug.Log(angle);
      if (Mathf.Abs(angle - 90) < 15)
        return true;
      return false;
    }

    private bool isKick(Hand hand)
    {
      Finger indexFg = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
      Finger midFg = hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE];
      Vector indexDir = indexFg.bones[(int)Bone.BoneType.TYPE_PROXIMAL].Direction;
      Vector midDir = midFg.bones[(int)Bone.BoneType.TYPE_PROXIMAL].Direction;
      Vector indexMetaDir = indexFg.bones[(int)Bone.BoneType.TYPE_METACARPAL].Direction;
      float angle = indexDir.AngleTo(midDir) * Mathf.Rad2Deg;
      float metaAngle = indexMetaDir.AngleTo(indexDir);
      //Debug.Log(angle);
      if (angle > 55 && metaAngle < 15)
        return true;
      return false;
    }

    private bool isPunch(Hand hand)
    {
      Finger indexFg = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
      Finger thumb = hand.Fingers[(int)Finger.FingerType.TYPE_THUMB];
      Vector thumbDir = thumb.bones[(int)Bone.BoneType.TYPE_PROXIMAL].Direction;
      Vector indexMetaDir = indexFg.bones[(int)Bone.BoneType.TYPE_METACARPAL].Direction;
      float angle = indexMetaDir.AngleTo(thumbDir) * Mathf.Rad2Deg;
      //Debug.Log(angle);
      if (angle > 40)
        return true;
      return false;
    }

    private bool isBackward(Hand hand)
    {
      Finger indexFg = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
      Finger midFg = hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE];
      Vector indexDistDir = indexFg.bones[(int)Bone.BoneType.TYPE_DISTAL].Direction;
      Vector midDistDIr = midFg.bones[(int)Bone.BoneType.TYPE_DISTAL].Direction;
      Vector midMetaDIr = midFg.bones[(int)Bone.BoneType.TYPE_METACARPAL].Direction;
      Vector indexMetaDir = indexFg.bones[(int)Bone.BoneType.TYPE_METACARPAL].Direction;
      float angle = indexDistDir.AngleTo(indexMetaDir) * Mathf.Rad2Deg;
      angle += midDistDIr.AngleTo(midMetaDIr) * Mathf.Rad2Deg;
      angle /= 2;
      //Debug.Log(angle);
      if (angle < 30)
        return true;
      return false;
    }

    private bool isDefend(Hand hand)
    {
      if (hand.PalmNormal.y > 0)
        return true;
      return false;
    }

    private bool isJump(Hand hand)
    {
      Vector v = hand.PalmVelocity;
      if (v.Magnitude > 0.3 && v.AngleTo(Vector.Up) * Mathf.Rad2Deg < 30)
        return true;
      return false;
    }

    private bool isForwardL(Hand hand)
    {
      Finger indexFg = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
      Finger midFg = hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE];
      Vector indexDir = indexFg.bones[(int)Bone.BoneType.TYPE_PROXIMAL].Direction;
      Vector midDir = midFg.bones[(int)Bone.BoneType.TYPE_PROXIMAL].Direction;
      float angle = indexDir.AngleTo(midDir) * Mathf.Rad2Deg;
      float indexToNorm = indexDir.AngleTo(hand.PalmNormal) * Mathf.Rad2Deg;
      float midToNorm = midDir.AngleTo(hand.PalmNormal) * Mathf.Rad2Deg;
      //Debug.Log(angle);
      //Debug.Log(indexFg.TipVelocity.Magnitude);
      if (angle > 30 && indexToNorm > midToNorm)
        return true;
      return false;
    }

    private bool isForwardR(Hand hand)
    {
      Finger indexFg = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
      Finger midFg = hand.Fingers[(int)Finger.FingerType.TYPE_MIDDLE];
      Vector indexDir = indexFg.bones[(int)Bone.BoneType.TYPE_PROXIMAL].Direction;
      Vector midDir = midFg.bones[(int)Bone.BoneType.TYPE_PROXIMAL].Direction;
      float angle = indexDir.AngleTo(midDir) * Mathf.Rad2Deg;
      float indexToNorm = indexDir.AngleTo(hand.PalmNormal) * Mathf.Rad2Deg;
      float midToNorm = midDir.AngleTo(hand.PalmNormal) * Mathf.Rad2Deg;
      //Debug.Log(angle);
      //Debug.Log(indexFg.TipVelocity.Magnitude);
      if (angle > 30 && indexToNorm <= midToNorm)
        return true;
      return false;
    }

    private const double TIME_TH = 2000;  //1000ms
    private double ltot = 0;
    private double rtot = 0;
    private const double lambda = 0.1;
    private int lprevType = -1;
    private int rprevType = -1;

    private bool checkInvert(List<double> a, List<double> b, bool isLeftHand)
    {
      double tot;
      if (isLeftHand)
        tot = ltot;
      else
        tot = rtot;
      int curTot = 0;
      for (int i = 0; i < a.Count; i++)
        for (int j = 0; j < b.Count; j++)
        {
          if (a[i] < b[j])
            curTot++;
        }
      tot = tot * (1 - lambda) + curTot * lambda;
      if (isLeftHand)
        ltot = tot;
      else
        rtot = tot;
      if (a.Count * b.Count * 0.25 < tot && tot < a.Count * b.Count * 0.75)
        return true;
      //Debug.Log(string.Format("curTot:{0}, tot:{1} a*b:{2}", curTot, tot, a.Count * b.Count));
      return false;
    }

    private bool check(GestureType at, bool isLeftHand)
    {
      List<List<double>> count;
      List<double> totalCount;
      if(isLeftHand)
      {
        count = lcount;
        totalCount = ltotalCount;
      }
      else
      {
        count = rcount;
        totalCount = rtotalCount;
      }
      double curTime = System.Environment.TickCount;
      count[(int)at].Add(curTime);
      for (int i = 0; i < totalCount.Count; i++)
      {
        if (curTime - totalCount[i] < TIME_TH)
        {
          totalCount.RemoveRange(0, i);
          break;
        }
      }
      for (int i = 0; i < count[(int)at].Count; i++)
      {
        if (curTime - count[(int)at][i] < TIME_TH)
        {
          count[(int)at].RemoveRange(0, i);
          break;
        }
      }
      if (at == GestureType.ForwardL || at == GestureType.ForwardR)
      {
        //Debug.Log("Forward");
        int thisAct = count[(int)at].Count;
        int anotherAct = count[(int)at ^ 1].Count;
        //Debug.Log(((int)at) ^ 1);
        /*
        if (!(thisAct + anotherAct > totalCount.Count * 0.4 &&
          thisAct > (thisAct + anotherAct) * 0.25 &&
          thisAct < (thisAct + anotherAct) * 0.75))
          Debug.Log(string.Format("this:{0}, another:{1}, total:{2}", thisAct, anotherAct, totalCount.Count));
          */
        if (thisAct + anotherAct > totalCount.Count * 0.4 &&
          thisAct > (thisAct + anotherAct) * 0.25 &&
          thisAct < (thisAct + anotherAct) * 0.75 &&
          checkInvert(count[(int)at], count[(int)at ^ 1], isLeftHand))
          return true;
      }
      else
      {
        double actPurity = (at.Equals(GestureType.Jump)) ? 0.1 : 0.4;
        if (count[(int)at].Count > totalCount.Count * actPurity)
          return true;
      }
      return false;
    }

    private bool checkForward(bool isLeftHand)
    {
      List<List<double>> count;
      List<double> totalCount;
      int prevType;
      if (isLeftHand)
      {
        count = lcount;
        totalCount = ltotalCount;
        prevType = lprevType;
      }
      else
      {
        count = rcount;
        totalCount = rtotalCount;
        prevType = rprevType;
      }
      double curTime = System.Environment.TickCount;
      for (int i = 0; i < totalCount.Count; i++)
      {
        if (curTime - totalCount[i] < TIME_TH)
        {
          totalCount.RemoveRange(0, i);
          break;
        }
      }
      for (int t = (int)GestureType.ForwardL; t <= (int)GestureType.ForwardR; t++)
        for (int i = 0; i < count[t].Count; i++)
        {
          if (curTime - count[t][i] < TIME_TH)
          {
            count[t].RemoveRange(0, i);
            break;
          }
        }
      if (!(prevType == (int)GestureType.ForwardL || prevType == (int)GestureType.ForwardR))
        return false;
      int thisAct = count[(int)GestureType.ForwardL].Count;
      int anotherAct = count[(int)GestureType.ForwardR].Count;
      if (!(thisAct + anotherAct > totalCount.Count * 0.4 &&
        thisAct > (thisAct + anotherAct) * 0.25 &&
        thisAct < (thisAct + anotherAct) * 0.75))
        Debug.Log(string.Format("this:{0}, another:{1}, total:{2}", thisAct, anotherAct, totalCount.Count));
      if (thisAct + anotherAct > totalCount.Count * 0.4 &&
        thisAct > (thisAct + anotherAct) * 0.25 &&
        thisAct < (thisAct + anotherAct) * 0.75 &&
        checkInvert(count[(int)GestureType.ForwardL], count[(int)GestureType.ForwardR], isLeftHand))
        return true;
      return false;
    }

    private void UpdateGesture(Hand hand, bool isLeftHand)
    {
      List<double> totalCount;
      int prevType;
      ActionType action;
      if (isLeftHand)
      {
        totalCount = ltotalCount;
        prevType = lprevType;
        action = laction;
      }
      else
      {
        //Debug.Log("Right"+ System.Environment.TickCount);
        totalCount = rtotalCount;
        prevType = rprevType;
        action = raction;
      }
      action = ActionType.Null;
      double curTime = System.Environment.TickCount;
      totalCount.Add(curTime);
      if (isDefend(hand))
      {
        if (check(GestureType.Defend, isLeftHand))
        {
          //Debug.Log("DEFEND");
          if (checkForward(isLeftHand))
          {
            action = ActionType.Forward;
          }
          else
          {
            prevType = (int)GestureType.Defend;
            action = ActionType.Defend;
          }
          if (isLeftHand)
            laction = action;
          else
            raction = action;
          return;
        }
      }

      if (isJump(hand))
      {
        Debug.Log("JUMP");
        if (check(GestureType.Jump, isLeftHand))
        {
          if (checkForward(isLeftHand))
          {
            action = ActionType.Forward;
          }
          else
          {
            prevType = (int)GestureType.Jump;
            action = ActionType.Jump;
          }
          if (isLeftHand)
            laction = action;
          else
            raction = action;
          return;
        }
      }

      if (isSquat(hand))
      {
        if (check(GestureType.Squat, isLeftHand))
        {
          if (checkForward(isLeftHand))
          {
            action = ActionType.Forward;
          }
          else
          {
            prevType = (int)GestureType.Squat;
            action = ActionType.Squat;
          }
          if (isLeftHand)
            laction = action;
          else
            raction = action;
          return;
        }
      }

      if (isKick(hand))
      {
        if (check(GestureType.Kick, isLeftHand))
        {
          if (checkForward(isLeftHand))
          {
            action = ActionType.Forward;
          }
          else
          {
            prevType = (int)GestureType.Kick;
            action = ActionType.Kick;
          }
          if (isLeftHand)
            laction = action;
          else
            raction = action;
          return;
        }
      }

      if (isBackward(hand))
      {
        if (check(GestureType.Backward, isLeftHand))
        {
          if (checkForward(isLeftHand))
          {
            action = ActionType.Forward;
          }
          else
          {
            prevType = (int)GestureType.Backward;
            action = ActionType.Backward;
          }
          if (isLeftHand)
            laction = action;
          else
            raction = action;
          return;
        }
      }

      if (isPunch(hand))
      {
        if (check(GestureType.Punch, isLeftHand))
        {
          if (checkForward(isLeftHand))
          {
            action = ActionType.Forward;
          }
          else
          {
            prevType = (int)GestureType.Punch;
            action = ActionType.Punch;
          }
          if (isLeftHand)
            laction = action;
          else
            raction = action;
          return;
        }
      }

      if (isForwardL(hand))
      {
        if (check(GestureType.ForwardL, isLeftHand))
        {
          prevType = (int)GestureType.ForwardL;
          action = ActionType.Forward;
          if (isLeftHand)
            laction = action;
          else
            raction = action;
          return;
        }
      }

      if (isForwardR(hand))
      {
        if (check(GestureType.ForwardR, isLeftHand))
        {
          prevType = (int)GestureType.ForwardR;
          action = ActionType.Forward;
          if (isLeftHand)
            laction = action;
          else
            raction = action;
          return;
        }
      }

      if (checkForward(isLeftHand))
      {
        action = ActionType.Forward;
      }
      if (isLeftHand)
        laction = action;
      else
        raction = action;
      return;
    }

    public ActionType GetAction(int player)
    {
      if (player == 1)
        return laction;
      else
      {
        Debug.Log("              " + raction);
        return raction;
      }
    }
  }
}
