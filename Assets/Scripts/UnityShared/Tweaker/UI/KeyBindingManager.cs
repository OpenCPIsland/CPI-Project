using System;
using System.Collections.Generic;
using System.Linq;
using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public class KeyBindingManager : MonoBehaviour
	{
		private ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		private readonly Dictionary<KeyCode, IInvokable> bindings = new Dictionary<KeyCode, IInvokable>();

		private KeyCode currentPress = KeyCode.None;

		private float secondsUntilActivation;

		public void Init(IEnumerable<IInvokable> invokables)
		{
			foreach (IInvokable invokable in invokables)
			{
				KeyBindingAttribute keyBindingAttribute = invokable.CustomAttributes.FirstOrDefault((ICustomTweakerAttribute a) => a is KeyBindingAttribute) as KeyBindingAttribute;
				if (keyBindingAttribute != null)
				{
					if (invokable.Parameters.Length != 0)
					{
						throw new InvalidOperationException("Cannot add key binding to Invokable that requires arguments.");
					}
					logger.Debug("Found Key Binding: {0} => {1}", keyBindingAttribute.Key, invokable.Name);
					bindings.Add(keyBindingAttribute.Key, invokable);
				}
			}
		}

		public void OnGUI()
		{
			if (currentPress == KeyCode.None)
			{
				foreach (KeyValuePair<KeyCode, IInvokable> binding in bindings)
				{
					if (Event.current.keyCode == binding.Key)
					{
						currentPress = binding.Key;
						secondsUntilActivation = 1f;
					}
				}
			}
		}

		public void Update()
		{
			if (currentPress != 0 && Input.GetKey(currentPress))
			{
				secondsUntilActivation -= Time.unscaledDeltaTime;
				if (secondsUntilActivation <= 0f)
				{
					KeyCode key = currentPress;
					currentPress = KeyCode.None;
					bindings[key].Invoke(null);
				}
			}
			else
			{
				currentPress = KeyCode.None;
			}
		}
	}
}
