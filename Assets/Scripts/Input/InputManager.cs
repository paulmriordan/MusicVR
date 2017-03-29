using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager> {

	public SmoothMouseLook m_mouseLook;
	public WallDragger m_wallDragger;

	void Start () 
	{
		MusicWall.Instance.OnWallDataUpdated += UpdateProperties;
	}

	public void UpdateProperties(MusicWallData wallData)
	{
		m_wallDragger.Reset(0, -wallData.GetTotalHeight());
	}

	void Update()
	{
		UpdateGestures();
		UpdatingObject.Check();

		// Disable input if required
		{
			bool inputBlocked = MusicWallUI.Instance.IsBlockingGameInput();
			inputBlocked |= IsCameraDragOccuring();

			m_mouseLook.EnableLook(!inputBlocked);
			WallButton.s_wallButtonInputState.SelectionEnabled(!inputBlocked);
		}

		m_wallDragger.DraggingEnabled = IsCameraDragOccuring();
	}

	public float HoldTime = 0.5f;
	public float HoldMoveLimit = 10.0f;

	bool cameraDragging = false;
	public float inputDownTime;
	Vector3 inputDownPos;

	public bool IsCameraDragOccuring()
	{
		return cameraDragging;
	}

	enum E_InputConsumeType { none, buttons, panning}
	E_InputConsumeType m_inputType;

	void UpdateGestures()
	{
		if (Input.GetMouseButtonDown(0))
		{	
			m_inputType = E_InputConsumeType.none;
			WallButton.s_wallButtonInputState.Clear();
			inputDownTime = Time.time;
			Debug.Log("input down time " + inputDownTime);
			inputDownPos = Input.mousePosition;
		}

		if (Input.GetMouseButton(0))
		{
			var d2 = (Input.mousePosition - inputDownPos).sqrMagnitude;
			if (!cameraDragging)
				Debug.Log("drag d " + Mathf.Sqrt(d2));
			if (!cameraDragging &&  d2 > HoldMoveLimit * HoldMoveLimit && !WallButton.s_wallButtonInputState.WallInputActive())
			{
				Debug.Log("starting camera drag");
				cameraDragging = true;
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			cameraDragging = false;
			inputDownTime = float.MaxValue;
		}
	}
}
