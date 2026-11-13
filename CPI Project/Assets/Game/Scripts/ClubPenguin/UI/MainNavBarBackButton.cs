using ClubPenguin.Input;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(ButtonClickListener))]
	public class MainNavBarBackButton : MonoBehaviour
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct MainNavBarBackButtonClicked
		{
		}

		private ButtonClickListener buttonClickListener;

		private void Awake()
		{
			buttonClickListener = GetComponent<ButtonClickListener>();
		}

		private void OnEnable()
		{
			buttonClickListener.OnClick.AddListener(onButtonClicked);
		}

		private void OnDisable()
		{
			buttonClickListener.OnClick.RemoveListener(onButtonClicked);
		}

		public void SetButtonActive(bool isActive)
		{
			GetComponent<Image>().enabled = isActive;
			GetComponent<Button>().enabled = isActive;
		}

		private void onButtonClicked(ButtonClickListener.ClickType clickType)
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(MainNavBarBackButtonClicked));
		}
	}
}
