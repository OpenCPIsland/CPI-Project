using ClubPenguin.Input;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Analytics
{
	public class LogSingularGameActionButtonInteracted : MonoBehaviour
	{
		[SerializeField]
		private string context = string.Empty;

		[SerializeField]
		private string action = string.Empty;

		[SerializeField]
		private string id = string.Empty;

		private bool listenerAdded;

		private ButtonClickListener clickListener;

		private void Start()
		{
			clickListener = GetComponentInParent<ButtonClickListener>();
			if (clickListener == null)
			{
			}
			addListener();
		}

		private void OnEnable()
		{
			addListener();
		}

		private void OnDisable()
		{
			if (listenerAdded && clickListener != null)
			{
				clickListener.OnClick.RemoveListener(OnClicked);
				listenerAdded = false;
			}
		}

		private void addListener()
		{
			if (!listenerAdded && clickListener != null)
			{
				clickListener.OnClick.AddListener(OnClicked);
				listenerAdded = true;
			}
		}

		private void OnClicked(ButtonClickListener.ClickType interactedType)
		{
			string callID = string.Format("{0}_{1}", id, interactedType.ToString());
			Service.Get<ICPSwrveService>().ActionSingular(callID, string.Format("game.{0}", context), action, interactedType.ToString());
		}
	}
}
