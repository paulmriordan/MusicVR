using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallUIButton : MonoBehaviour 
{
	public MeshRenderer MeshRenderer;
	public Transform ButtonBody;
	public Transform Text;

	private bool m_mouseDown;
	private WallButtonTween m_buttonTweener;
	private CompositionData.InstrumentData m_instrumentDataRef;
	private System.Action<CompositionData.InstrumentData> m_onClicked;

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

	void Awake()
	{
		m_buttonTweener = GetComponent<WallButtonTween>();
	}

	public void SetWidth(int colWidth)
	{
		var parent = Text.parent;
		Text.SetParent(transform);
		ButtonBody.transform.localScale = new Vector3(colWidth, 1.0f, 1.0f);
		Text.SetParent(parent);
	}

	public void SetInstrument(CompositionData.InstrumentData instrument)
	{
		m_instrumentDataRef = instrument;
	}

	public void SetButtonAction(System.Action<CompositionData.InstrumentData> onClicked)
	{
		m_onClicked = onClicked;
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
			m_onClicked(m_instrumentDataRef);
		}

		MouseDown = false;
	}

	public void CustomUpdate()
	{
		if (m_buttonTweener != null)
			m_buttonTweener.CustomUpdate();
	}
}

