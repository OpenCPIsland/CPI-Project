using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ClickListener))]
	public class ClickAwayEventSource : AbstractEventSource
	{
		private ClickListener clickListener;

		private void Awake()
		{
			clickListener = GetComponent<ClickListener>();
		}

		private void OnEnable()
		{
			clickListener.OnClicked += onClick;
		}

		protected virtual void OnDisable()
		{
			clickListener.OnClicked -= onClick;
		}

		protected virtual void onClick()
		{
			sendEvent();
		}
	}
}
