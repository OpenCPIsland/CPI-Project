using System;
using UnityEngine.Events;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(InputField))]
	[AddComponentMenu("UI/Extensions/Input Field Submit")]
	public class InputFieldEnterSubmit : MonoBehaviour
	{
		[Serializable]
		public class EnterSubmitEvent : UnityEvent<string>
		{
		}

		public EnterSubmitEvent EnterSubmit;

		private InputField _input;

		private void Awake()
		{
			_input = GetComponent<InputField>();
			_input.onEndEdit.AddListener(OnEndEdit);
		}

		public void OnEndEdit(string txt)
		{
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				EnterSubmit.Invoke(txt);
			}
		}
	}
}
