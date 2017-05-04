using UnityEngine;
using System.Collections;
using CompositionCommands;

public abstract class WallButtonAbstract : MonoBehaviour
{
	protected CompositionData.InstrumentData m_instrumentData;
	protected WallButtonTween m_buttonTweener;
	protected WallButtonColorController m_buttonColorController;
	protected ButtonInputHandler m_inputHander;

	public WallButtonColorController ColorController {get {return m_buttonColorController;}}
	public ButtonInputHandler InputHandler {get {return m_inputHander;}}
	public WallButtonTween Tweener {get {return m_buttonTweener;}}

	protected virtual void Awake()
	{
		m_buttonTweener = GetComponent<WallButtonTween>();
		m_buttonColorController = GetComponent<WallButtonColorController>();
	}

	public abstract void Clicked();
}

