using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager> 
{
	[System.Serializable]
	public class InputState
	{
		public float 		HoldTime = 0.5f;
		public float 		HoldMoveLimit = 10.0f;
		public Vector2 		EdgeDistPan = new Vector2(0.15f, 0.15f);
		public float 		ThresholdStartEdgePan = 10.0f;

		public float 		InputDownTime {get;set;}
		public Vector3 		InputDownPos {get;set;}
	}

	public InputState m_inputState = new InputState();
	public SmoothMouseLook m_mouseLook;
	public WallDragger m_wallDragger;

	void Start () 
	{
		MusicWall.Instance.OnWallDataUpdated += UpdateProperties;
		SequencerButtonInputHander.s_sequencerButtonDrag.OnPanRequested += m_wallDragger.PerformPan;
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
		SequencerButtonInputHander.s_sequencerButtonDrag.Update(m_inputState);

		// Disable input if required
		{
			bool inputBlocked = MusicWallUI.Instance.IsBlockingGameInput();
			inputBlocked |= m_wallDragger.InputConsumer.IsActive();

			m_mouseLook.EnableLook(!inputBlocked);
			SequencerButtonInputHander.s_sequencerButtonDrag.SelectionEnabled(!inputBlocked);
		}
	}

	void UpdateGestures()
	{
		if (Input.GetMouseButtonDown(0))
		{	
			SequencerButtonInputHander.s_sequencerButtonDrag.Clear();
			m_inputState.InputDownTime = Time.time;
			m_inputState.InputDownPos = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0))
		{
			m_inputState.InputDownTime = float.MaxValue;
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
