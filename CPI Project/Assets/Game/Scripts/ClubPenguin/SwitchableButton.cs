using ClubPenguin.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(Button))]
	public class SwitchableButton : MonoBehaviour
	{
		public const int OFF_SPRITE_INDEX = 0;

		public const int ON_SPRITE_INDEX = 1;

		public SpriteSelector SpriteSelector;

		public InputEvents.Switches Switch;

		private Button button;

		private EventDispatcher dispatcher;

		private bool onoff = false;

		public bool OnOff
		{
			get
			{
				return onoff;
			}
			set
			{
				onoff = value;
				SpriteSelector.SelectSprite(onoff ? 1 : 0);
			}
		}

		public void Awake()
		{
			button = GetComponent<Button>();
			dispatcher = Service.Get<EventDispatcher>();
			if (SpriteSelector == null)
			{
				SpriteSelector = GetComponent<SpriteSelector>();
			}
		}

		public void OnEnable()
		{
			button.onClick.AddListener(onClick);
		}

		public void OnDisable()
		{
			button.onClick.RemoveListener(onClick);
		}

		private void onClick()
		{
			OnOff = !OnOff;
			dispatcher.DispatchEvent(new InputEvents.SwitchChangeEvent(Switch, OnOff));
		}
	}
}
