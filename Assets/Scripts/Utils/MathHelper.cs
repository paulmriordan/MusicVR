using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper : MonoBehaviour {
	
	public static Vector2 GetClosestPosOnSegment(Vector2 v, Vector2 w, Vector2 p) {
		float l2 = (v - w).sqrMagnitude;
		if (l2 == 0.0) return v;   // v == w case
		float t = Vector2.Dot((p - v),(w - v))/ l2;
		if (t < 0.0) return v;      // Beyond the 'v' end of the segment
		else if (t > 1.0) return w; // Beyond the 'w' end of the segment
		return v + t * (w - v);     // Projection falls on the segment
	}

	public static float MinDistToLineSegSqrd(Vector2 v, Vector2 w, Vector2 p) {
		Vector2 proj = GetClosestPosOnSegment(v, w, p);
		return (p - proj).sqrMagnitude;
	}

	public static float MinDistToLineSeg(Vector2 v, Vector2 w, Vector2 p) {
		return Mathf.Sqrt(MinDistToLineSegSqrd(v,w,p));
	}

	public static float MinDistToLine(Vector2 norm, Vector2 linePos, Vector2 p) {
		float t = Vector2.Dot(p - linePos,norm);
		Vector2 projection = linePos + t * (norm);
		return (p - projection).magnitude;
	}

	public static Vector3 GetPosOnPlane(Vector3 planeNormal, Vector3 planePos, Ray ray)
	{
		var plane = new Plane(planeNormal, planePos);
		float d = 0;
		plane.Raycast(ray, out d);
		return ray.GetPoint(d);
	}

    public static float ClampAngle(float value, float start, float end)
    {
        float width = end - start;
        float offsetValue = value - start;

        return (offsetValue - (Mathf.Floor(offsetValue / width) * width)) + start;
    }

    public static float DistBetweenAngles(float targetA, float sourceA)
    {
        var a = targetA - sourceA;
        a = ModNoNeg((a + 180.0f), 360.0f) - 180.0f;
        return a;
    }

    public static float ModNoNeg(float a, float n)
    {
        return a - Mathf.Floor(a / n) * n;
    }

    /// <summary>
    /// </summary>
    /// <returns>Angle in degrees between the vectors</returns>
    public static float AngleBetweenVectors(Vector2 a, Vector2 b)
    {
        var angleA = Mathf.Atan2(a.y, a.x) * Mathf.Rad2Deg;
        var angleB = Mathf.Atan2(b.y, b.x) * Mathf.Rad2Deg;
        return MathHelper.DistBetweenAngles(angleA, angleB);
    }

    public static bool rayCircleNearestPositveIntersection(
        Vector2 rayOrigin,
        Vector2 rayNormal,
        Vector2 circleCentre,
        float circleRadius,
        ref float out_rayHitDist)
    {
        Vector2 n = rayNormal;
        Vector2 p = rayOrigin - circleCentre;
        float r = -circleRadius;

        float b = (2 * n.x * p.x + 2 * n.y * p.y);
        float a = (n.y * n.y + n.x * n.x);
        float c = (p.x * p.x + p.y * p.y + r);
        
        if (a == 0)
            return false;

        float b2_4ac = b*b - 4 * a * c;

        if (b2_4ac < 0)
            return false;

        float sqrt_b2_4ac = Mathf.Sqrt(b2_4ac);
        float u0 = (-b + sqrt_b2_4ac) / (2 * a);
        float u1 = (-b - sqrt_b2_4ac) / (2 * a);

        if (u0 < 0 && u1 < 0) {
            return false;
        } else if (u0 < 0) {
            out_rayHitDist = u1;
        } else if (u1 < 0) {
            out_rayHitDist = u0;
        } else {
            Mathf.Min(u0, u1);
        }
        return true;
    }
}
