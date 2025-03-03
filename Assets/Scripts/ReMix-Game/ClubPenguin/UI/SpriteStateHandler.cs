using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SpriteStateHandler : MonoBehaviour
	{
		[Serializable]
		public struct StateBinding
		{
			public string State;

			public Sprite Value;
		}

		public Image Target;

		public StateBinding[] Bindings;

		private Sprite defaultValue;

		public void Awake()
		{
			defaultValue = Target.sprite;
		}

		public void OnStateChanged(string state)
		{
			Sprite value = defaultValue;
			for (int i = 0; i < Bindings.Length; i++)
			{
				if (Bindings[i].State == state)
				{
					value = Bindings[i].Value;
					break;
				}
			}
			Target.sprite = value;
		}
	}
}
