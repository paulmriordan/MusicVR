using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePanningUtil : MonoBehaviour 
{
	public static bool DragTreshReached(float _deltaDragDist, float _dragThreshold)
	{
		return _deltaDragDist > _dragThreshold*Screen.dpi/326.0f;
	}

	public static Vector2 EdgeDragPanAmount(Vector2 screenCoord,
		float EdgePanXBorderFrac,
		float EdgePanYBorderFrac)
	{
		//Pan the camera if the finger is near the edge of the screen
		float sw = Screen.width;
		float sh = Screen.height;
		float minx = sw*EdgePanXBorderFrac;
		float maxx = sw*(1.0f-EdgePanXBorderFrac);
		float miny = sh*EdgePanYBorderFrac;
		float maxy = sh*(1.0f-EdgePanYBorderFrac);
		float panAmount = 0;
		if (EdgePanXBorderFrac != 0 && screenCoord.x < minx)
			panAmount = MathHelper.MinDistToLineSegSqrd(new Vector2(minx,miny), new Vector2(minx,maxy), screenCoord);
		else if (EdgePanXBorderFrac != 0 && screenCoord.x > maxx)										  
			panAmount = MathHelper.MinDistToLineSegSqrd(new Vector2(maxx,miny), new Vector2(maxx,maxy), screenCoord);
		else if (EdgePanYBorderFrac != 0 && screenCoord.y < miny)										  
			panAmount = MathHelper.MinDistToLineSegSqrd(new Vector2(minx,miny), new Vector2(maxx,miny), screenCoord);
		else if (EdgePanYBorderFrac != 0 && screenCoord.y > maxy)										  
			panAmount = MathHelper.MinDistToLineSegSqrd(new Vector2(minx,maxy), new Vector2(maxx,maxy), screenCoord);
		panAmount = Mathf.Sqrt(panAmount);
		var div = Mathf.Max(minx, miny);
		panAmount /= div != 0 ? div : 1.0f; //always div by minx or miny, to avoid sudden pan velocity jump at corners
		if (panAmount > 0)
		{
			Vector2 vec = (screenCoord - new Vector2(sw*0.5f, sh*0.5f)).normalized;
			return -(vec*panAmount); //Pan the opposite direction
		}
		return Vector2.zero;
	}
}
