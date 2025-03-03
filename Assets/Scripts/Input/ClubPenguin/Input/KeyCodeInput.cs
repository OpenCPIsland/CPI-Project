using UnityEngine;

namespace ClubPenguin.Input
{
	public class KeyCodeInput : Input<ButtonInputResult>
	{
		public KeyCode[] Keys = new KeyCode[0];

		private KeyCode[] mutableKeys;

		public KeyCode PrimaryKey
		{
			get
			{
				return (mutableKeys.Length != 0) ? mutableKeys[0] : KeyCode.None;
			}
		}

		public override void Initialize(KeyCodeRemapper keyCodeRemapper)
		{
			mutableKeys = new KeyCode[Keys.Length];
			for (int i = 0; i < Keys.Length; i++)
			{
				mutableKeys[i] = keyCodeRemapper.GetKeyCode(Keys[i]);
			}
			base.Initialize(keyCodeRemapper);
		}

		protected override bool process(int filter)
		{
			bool flag = false;
			bool flag2 = false;
			KeyCode[] array = mutableKeys;
			foreach (KeyCode key in array)
			{
				flag2 |= UnityEngine.Input.GetKeyDown(key);
				flag |= UnityEngine.Input.GetKey(key);
			}
			inputEvent.WasJustPressed = (flag2 && !inputEvent.IsHeld);
			inputEvent.WasJustReleased = (!flag && inputEvent.IsHeld);
			inputEvent.IsHeld = (flag || flag2);
			return inputEvent.IsHeld || inputEvent.WasJustReleased;
		}
	}
}
