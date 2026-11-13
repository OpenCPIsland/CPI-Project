using Disney.MobileNetwork;
using Mix.Native;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class MockKeyboardController : MonoBehaviour
	{
		public void HideKeyboard()
		{
			Service.Get<NativeKeyboardManager>().Keyboard.Hide();
		}
	}
}
