using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MusicVR.Wall
{
	public class SequencerButtonInputHander : ButtonInputHandler
	{
		public static SequencerButtonDrag s_sequencerButtonDrag = null;

		private SequencerWallButton m_parent;

		protected override WallButtonAbstract Parent { get {return m_parent;}}

		public void InitWithSequencerButton(SequencerWallButton parent)
		{
			m_parent = parent;
		}
        
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

