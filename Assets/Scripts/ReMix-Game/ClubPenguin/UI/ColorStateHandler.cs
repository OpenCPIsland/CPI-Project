using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ColorStateHandler : MonoBehaviour
	{
		[Serializable]
		public struct StateBinding
		{
			public string State;

			public Color Value;
		}

		public Graphic Target;

		public StateBinding[] Bindings;

		private Color defaultValue;

		public void Awake()
		{
			defaultValue = Target.color;
		}

		public void OnStateChanged(string state)
		{
			Color value = defaultValue;
			for (int i = 0; i < Bindings.Length; i++)
			{
				if (Bindings[i].State == state)
				{
					value = Bindings[i].Value;
					break;
				}
			}
			Target.color = value;
		}
	}
}
