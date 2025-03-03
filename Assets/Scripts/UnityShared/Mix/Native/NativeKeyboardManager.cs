using UnityEngine;

namespace Mix.Native
{
	public class NativeKeyboardManager : MonoBehaviour
	{
		private NativeKeyboard keyboard;

		public NativeKeyboard Keyboard
		{
			get
			{
				return keyboard;
			}
			private set
			{
				keyboard = value;
			}
		}

		public void Init()
		{
			base.gameObject.name = "NativeKeyboard";
		}

		private void Awake()
		{
			Keyboard = base.gameObject.AddComponent<NativeKeyboard>();
		}
	}
}
