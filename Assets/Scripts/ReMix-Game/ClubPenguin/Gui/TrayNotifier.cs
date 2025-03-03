using UnityEngine;

namespace ClubPenguin.Gui
{
	public class TrayNotifier : MonoBehaviour
	{
		protected TrayController controller;

		public virtual void Awake()
		{
			controller = Object.FindObjectOfType<TrayController>();
			if (controller == null)
			{
				throw new UnityException("Could not find parent tray controller!");
			}
		}
	}
}
