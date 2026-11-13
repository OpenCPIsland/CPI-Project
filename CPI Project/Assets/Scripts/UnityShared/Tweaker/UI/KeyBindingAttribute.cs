using System;
using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI
{
	public class KeyBindingAttribute : Attribute, ICustomTweakerAttribute
	{
		public readonly KeyCode Key;

		public KeyBindingAttribute(KeyCode key)
		{
			Key = key;
		}
	}
}
