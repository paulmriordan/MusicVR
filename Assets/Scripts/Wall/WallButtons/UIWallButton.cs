using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MusicVR.Composition;

namespace MusicVR.Wall
{
	public class UIWallButton : WallButtonAbstract 
	{
		public InstrumentUIData.InstrumentUIButton m_UIButtonData;
		public MeshRenderer MeshRenderer;
		public Transform ButtonBody;
		public Transform Text;

		private CompositionData m_compositionData;

		protected override void Awake()
		{
			base.Awake();
			ClickableButtonInputHandler clickable;
			m_inputHander = clickable = GetComponent<ClickableButtonInputHandler>();
			clickable.Init(this);
		}

		public void Init(InstrumentUIData.InstrumentUIButton buttonData,
						CompositionData compositionData,
						InstrumentData instrumentData)
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
			var commandManager = MusicWall.Instance.WallProperties.CompositionData.CommandManager;
			commandManager.ExecuteCommand(InstrumentUICommandFactory.Create(m_UIButtonData.CommandType, m_instrumentData));
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
			case InstrumentUIData.E_InstrumentCommand.toggleScale: 
				Text.GetComponent<TextMesh>().text = m_instrumentData.Scale.ToString();
				break;
			case InstrumentUIData.E_InstrumentCommand.toggleInstrument:
				Text.GetComponent<TextMesh>().text = m_instrumentData.InstrumentDefinition.Name.ToString();
				break;
			}

		}
	}
}

