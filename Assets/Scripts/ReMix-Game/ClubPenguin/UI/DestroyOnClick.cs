using ClubPenguin.Input;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ButtonClickListener))]
	public class DestroyOnClick : MonoBehaviour
	{
		public GameObject ObjectToDestroy;

		private ButtonClickListener clickListener;

		private void Awake()
		{
			clickListener = GetComponent<ButtonClickListener>();
		}

		private void OnEnable()
		{
			clickListener.OnClick.AddListener(onListenerClicked);
		}

		private void OnDisable()
		{
			clickListener.OnClick.RemoveListener(onListenerClicked);
		}

		private void onListenerClicked(ButtonClickListener.ClickType interactedType)
		{
			Object.Destroy(ObjectToDestroy);
		}
	}
}
