using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager> {

	public SmoothMouseLook m_mouseLook;
	public WallDragger m_wallDragger;

	void Start () 
	{
		MusicWall.Instance.OnWallDataUpdated += UpdateProperties;
		WallButton.s_wallButtonInputState.OnPanRequested += m_wallDragger.PerformPan;
	}

	public void UpdateProperties(MusicWallData wallData)
	{
		m_wallDragger.Reset(0, -wallData.GetTotalHeight(), wallData.CompositionData.NumCols);
	}

	void Update()
	{
		UpdateGestures();
		UpdateKeyCommands();
		UpdatingObject.Check();
		WallButton.s_wallButtonInputState.Update(m_inputState);

		// Disable input if required
		{
			bool inputBlocked = MusicWallUI.Instance.IsBlockingGameInput();
			inputBlocked |= m_wallDragger.InputConsumer.IsActive();

			m_mouseLook.EnableLook(!inputBlocked);
			WallButton.s_wallButtonInputState.SelectionEnabled(!inputBlocked);
		}
	}

	[System.Serializable]
	public class InputState
	{
		public float HoldTime = 0.5f;
		public float HoldMoveLimit = 10.0f;
		public Vector2 EdgeDistPan = new Vector2(0.15f, 0.15f);
		public float ThresholdStartEdgePan = 10.0f;

		public float inputDownTime {get;set;}
		public  Vector3 inputDownPos {get;set;}
	}
	public InputState m_inputState = new InputState();

	void UpdateGestures()
	{
		if (Input.GetMouseButtonDown(0))
		{	
			WallButton.s_wallButtonInputState.Clear();
			m_inputState.inputDownTime = Time.time;
			m_inputState.inputDownPos = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0))
		{
			m_inputState.inputDownTime = float.MaxValue;
		}

		InputConsumerBase.UpdateConsumers(m_inputState);
	}

	void UpdateKeyCommands()
	{
		if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyUp(KeyCode.Z))
		{
			MusicWall.Instance.WallProperties.CompositionData.CommandManager.Undo();
		}
	}
}
