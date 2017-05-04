using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ButtonInputHandler : MonoBehaviour
{
	private bool m_mouseDown;

	protected abstract WallButtonAbstract Parent {get;}

	public bool MouseDown { 
		get {
			return m_mouseDown;
		}
		set {
			m_mouseDown = value;
			if (Parent.Tweener != null)
				Parent.Tweener.Held = value;
		}
	}
}

public class ClickableButtonInputHandler : ButtonInputHandler
{
	private WallButtonAbstract m_parent;

	protected override WallButtonAbstract Parent { get {return m_parent;}}

	public void Init(WallButtonAbstract parent)
	{
		m_parent = parent;
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
			m_parent.Clicked();
		}

		MouseDown = false;
	}
}

