using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CompositionCommands;

public class WallUIButton : WallButtonAbstract 
{
	public InstrumentUIData.InstrumentUIButton m_UIButtonData;
	public MeshRenderer MeshRenderer;
	public Transform ButtonBody;
	public Transform Text;

	private bool m_mouseDown;
	private CompositionData m_compositionData;

	public bool MouseDown { 
		get {
			return m_mouseDown;
		}
		set {
			m_mouseDown = value;
			if (m_buttonTweener != null)
				m_buttonTweener.Held = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_buttonTweener = GetComponent<WallButtonTween>();
	}

	public void Init(InstrumentUIData.InstrumentUIButton buttonData,
					CompositionData compositionData,
					CompositionData.InstrumentData instrumentData)
	{
		m_instrumentData = instrumentData;
		m_UIButtonData = buttonData;
		m_compositionData = compositionData;
		SetWidth(buttonData.Width);

		compositionData.OnCompositionChanged += RefreshText;

		RefreshText();
	}

	void OnDestroy()
	{
		m_compositionData.OnCompositionChanged -= RefreshText;
	}

	public override void Clicked ()
	{
		switch ( m_UIButtonData.CommandType)
		{
		case E_CommandType.toggleScale:
			MusicWall.Instance.WallProperties.CompositionData.CommandManager.ExecuteCommand(new ToggleScaleCommand(m_instrumentData));
			break;
		case E_CommandType.toggleInstrument:
			MusicWall.Instance.WallProperties.CompositionData.CommandManager.ExecuteCommand(new ToggleInstrumentCommand(m_instrumentData));
			break;
		default:
			Debug.LogError("Unhandled command type:" + m_UIButtonData.CommandType.ToString());
			break;
		}
	}

	public void OnTouchDown()
	{
		if (!Input.GetMouseButton(0))
			return;
		{
			bool allowStart = true;
			allowStart &= !EventSystem.current.IsPointerOverGameObject();
			if (allowStart)
			{
				MouseDown = true;
			}
		}
	}

	public void OnTouchExit()
	{
		MouseDown = false;
	}

	public void OnTouchUp()
	{
		// Click button if no camera drag occurred & no button selecting occurred
		if (MouseDown)
		{
			Debug.Log("UI button clicked");
			Clicked();
		}

		MouseDown = false;
	}

	public void CustomUpdate()
	{
		if (m_buttonTweener != null)
			m_buttonTweener.CustomUpdate();
	}


	private void SetWidth(int colWidth)
	{
		var parent = Text.parent;
		Text.SetParent(transform);
		ButtonBody.transform.localScale = new Vector3(colWidth, 1.0f, 1.0f);
		Text.SetParent(parent);
	}

	private void RefreshText()
	{
		switch ( m_UIButtonData.CommandType)
		{
		case E_CommandType.toggleScale: 
			Text.GetComponent<TextMesh>().text = m_instrumentData.Scale.ToString();
			break;
		case E_CommandType.toggleInstrument:
			Text.GetComponent<TextMesh>().text = m_instrumentData.InstrumentDefintion.Name.ToString();
			break;
		}

	}
}

