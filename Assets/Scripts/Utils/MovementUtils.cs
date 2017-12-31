using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MovementHelpers
{
    //http://allenchou.net/2015/04/game-math-precise-control-over-numeric-springing/
    /*
      x     - value             (input/output)
      v     - velocity          (input/output)
      xt    - target value      (input)
      zeta  - damping ratio     (input)
      omega - angular frequency (input)
      h     - time step         (input)
    */
    //______________________________________________________________________
    public static void Spring(ref float x, ref float v, float xt,
                 float zeta, float omega, float h)
    {
        float f = 1.0f + 2.0f * h * zeta * omega;
        float oo = omega * omega;
        float hoo = h * oo;
        float hhoo = h * hoo;
        float detInv = 1.0f / (f + hhoo);
        float detX = f * x + h * v + hhoo * xt;
        float detV = v + hoo * (xt - x);
        x = detX * detInv;
        v = detV * detInv;
    }

    public static void SpringV(ref Vector3 pos, ref Vector3 vel, Vector3 tar,
                               float zeta, float omega, float h)
    {
        MovementHelpers.Spring(ref pos.x, ref vel.x, tar.x, zeta, omega, h);
        MovementHelpers.Spring(ref pos.y, ref vel.y, tar.y, zeta, omega, h);
        MovementHelpers.Spring(ref pos.z, ref vel.z, tar.z, zeta, omega, h);
    }

    public static float ClampAngle(float ang)
    {
        if (ang > 180.0f)
            ang -= 360.0f;
        if (ang < -180.0f)
            ang += 360.0f;
        return ang;
    }
    
    public static void SpringAngle(ref float x, ref float v, float xt,
                               float zeta, float omega, float h)
    {
        float d = MathHelper.DistBetweenAngles(xt, x);
        xt = x + d;
        Spring(ref x, ref v, xt, zeta, omega, h);
        x = ClampAngle(x);
    }

    public static Vector3 SmoothDampAngles(Vector3 currPos, Vector3 tarPos, ref Vector3 vel, float smoothTime)
    {
        currPos.x = SmoothDampAngle(currPos.x, tarPos.x, ref vel.x, smoothTime);
        currPos.y = SmoothDampAngle(currPos.y, tarPos.y, ref vel.y, smoothTime);
        currPos.z = SmoothDampAngle(currPos.z, tarPos.z, ref vel.z, smoothTime);
        return currPos;
    }

    public static Vector3 SmoothDampVector(Vector3 currPos, Vector3 tarPos, ref Vector3 vel, float smoothTime)
    {
        currPos.x = SmoothDampSafe(currPos.x, tarPos.x, ref vel.x, smoothTime);
        currPos.y = SmoothDampSafe(currPos.y, tarPos.y, ref vel.y, smoothTime);
        currPos.z = SmoothDampSafe(currPos.z, tarPos.z, ref vel.z, smoothTime);
        return currPos;
    }

    public static float SmoothDampAngle(float x, float xt, ref float v, float smoothTime)
    {
        float d = MathHelper.DistBetweenAngles(xt, x);
        xt = x + d;
        return ClampAngle(SmoothDampSafe(x, xt, ref v, smoothTime));
    }

    public static float SmoothDampSafe(float x, float xt, ref float v, float smoothTime)
    {
        if (smoothTime != 0)
        {
            return Mathf.SmoothDamp(x, xt, ref v, smoothTime);
        }
        else if (Time.timeScale > 0.01f)
        {
            v = 0;
            return xt;
        }
        else
        {
            return x;
        }
    }
}
