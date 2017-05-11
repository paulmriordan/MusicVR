using UnityEngine;
using UnityEngine.EventSystems;

namespace MusicVR.Wall
{
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
}