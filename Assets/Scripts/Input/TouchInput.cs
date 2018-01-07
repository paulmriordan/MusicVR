using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour {

	public LayerMask touchInputMask;
	public int MaxDistance;

	private Camera cam;
	private List<GameObject> touchList = new List<GameObject>();
	private List<GameObject> touchListOld = new List<GameObject>();
	private RaycastHit hit;

	void Start ()
	{
		cam = this.GetComponent<Camera>();
	}
	void Update () 
	{
		#if UNITY_EDITOR || UNITY_STANDALONE
		if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
		{
			touchListOld.Clear();
			touchListOld.AddRange(touchList);
			touchList.Clear();


			Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, MaxDistance, touchInputMask);
			if (hit.collider != null)
			{
				GameObject recipent = hit.collider.gameObject;
				touchList.Add(recipent);

				if (Input.GetMouseButtonDown(0))
				{
					recipent.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
				}
				if (Input.GetMouseButtonUp(0))
				{
					recipent.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
				}
				if (Input.GetMouseButton(0) && Input.GetMouseButtonDown(0) == false &&touchListOld.Contains(recipent) == false)
				{
					recipent.SendMessage("OnTouchEntered", hit.point, SendMessageOptions.DontRequireReceiver);
				}
				if (Input.GetMouseButton(0))
				{
					recipent.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
			foreach (GameObject g in touchListOld)
			{
				if (!touchList.Contains(g) && g != null)
				{
					g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		#endif

		#if UNITY_IOS || UNITY_ANDROID
		if (Input.touchCount > 0)
		{
			touchListOld.Clear();
			touchListOld.AddRange(touchList);
			touchList.Clear();

			Touch[] currentTouches = Input.touches;
			for (int i = 0; i < Input.touchCount; i++)
			{
				Physics.Raycast(cam.ScreenPointToRay(currentTouches[i].position), out hit, MaxDistance, touchInputMask);
				if (hit.collider != null)
				{
					GameObject recipent = hit.transform.gameObject;
					touchList.Add(recipent);

					if (currentTouches[i].phase == TouchPhase.Began)
					{
						recipent.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
					}
					if (currentTouches[i].phase == TouchPhase.Ended)
					{
						recipent.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
					}
					if (currentTouches[i].phase == TouchPhase.Stationary || currentTouches[i].phase == TouchPhase.Moved)
					{
						recipent.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
					}
					if (currentTouches[i].phase == TouchPhase.Moved && currentTouches[i].phase != TouchPhase.Began && touchListOld.Contains(recipent) == false)
					{
						recipent.SendMessage("OnTouchEntered", hit.point, SendMessageOptions.DontRequireReceiver);
					}
					if (currentTouches[i].phase == TouchPhase.Moved)
					{
						recipent.SendMessage("OnTouchMoved", hit.point, SendMessageOptions.DontRequireReceiver);
					}
					if (currentTouches[i].phase == TouchPhase.Canceled)
					{
						recipent.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			foreach (GameObject g in touchListOld)
			{
				if (!touchList.Contains(g) && g != null)
				{
					g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		#endif
	}
}
