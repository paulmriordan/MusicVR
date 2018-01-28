using UnityEngine;
using UnityEngine.EventSystems;

namespace MusicVR.Wall
{
	public class ClickableButtonInputHandler : ButtonInputHandler
	{
		private WallButtonAbstract m_parent;

		protected override WallButtonAbstract Parent { get {return m_parent;}}

		public void Init(WallButtonAbstract parent)
		{
			m_parent = parent;
		}

        #region Mobile input events
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

        #endregion

        #region VR Input Events

        public void OnPointerStay()
        {
            MouseDown = true;
        }

        public void OnPointerExit()
        {
            MouseDown = false;
        }

        public void OnPointerUp()
        {
            if (MouseDown)
            {
                Debug.Log("UI button clicked");
                m_parent.Clicked();
            }

            MouseDown = false;
        }

        #endregion
    }
}
