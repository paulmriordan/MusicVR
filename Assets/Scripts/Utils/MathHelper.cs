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

	public static int Mod(int x, int m) {
		return (x%m + m)%m;
	}
}
