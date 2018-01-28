using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MusicVR.WallInput;

namespace MusicVR.Wall
{
	public class SequencerButtonInputHander : ButtonInputHandler
	{
		public static SequencerButtonDrag s_sequencerButtonDrag = null;
		private static WallButtonInputConsumer s_inputConsumer = new WallButtonInputConsumer();

		private SequencerWallButton m_parent;

		protected override WallButtonAbstract Parent { get {return m_parent;}}

		public void InitWithSequencerButton(SequencerWallButton parent)
		{
			m_parent = parent;
		}

		#region Input Events
		public void OnTouchStay()
		{
			if (!Input.GetMouseButton(0))
				return;

			TryStartDrag();
			TrySelectViaDrag();
		}

		public void OnTouchExit()
		{
			MouseDown = false;
		}

		public void OnTouchUp()
		{
			// Click button if no camera drag occurred & no button dragging occurred
			if (s_inputConsumer.IsActive() 
				&& s_sequencerButtonDrag == null
				&& !InputManager.Instance.InputBlockedByUI())
			{
				m_parent.Clicked();
			}

			MouseDown = false;
			s_sequencerButtonDrag = null;
		}
        #endregion

        #region VR Input Events

        public void OnPointerStay()
        {
            TryStartDrag();
            TrySelectViaDrag();
        }

        public void OnPointerExit()
        {
            MouseDown = false;
        }

        public void OnPointerUp()
        {
            MouseDown = false;
            // Click button if no button dragging occurred
            if (s_sequencerButtonDrag == null)
                m_parent.Clicked();
            s_sequencerButtonDrag = null;
        }

        #endregion
        
        void TryStartDrag()
		{
			if (s_sequencerButtonDrag == null)
			{
				MouseDown = true;
				s_sequencerButtonDrag = new SequencerButtonDrag(m_parent.IsSelected 
					? SequencerButtonDrag.E_SelectState.unselecting 
					: SequencerButtonDrag.E_SelectState.selecting);
			}
		}

		void TrySelectViaDrag()
		{
			if (s_sequencerButtonDrag != null)
			{
				bool trySelect = this.m_parent != s_sequencerButtonDrag.LastHitButton;

				if (trySelect)
				{
					MouseDown = true;

                    //Don't toggle if already at desired state
                    var selected = s_sequencerButtonDrag.InputSelectType == SequencerButtonDrag.E_SelectState.selecting;
                    if (m_parent.IsSelected != selected)
                        m_parent.Clicked();

                    s_sequencerButtonDrag.LastHitButton = this.m_parent;
				}
			}
		}
	}
}

