using System;
using UnityEngine;

namespace ClubPenguin.Core
{
	[Serializable]
	public class Manifest : ScriptableObject
	{
		public ScriptableObject[] Assets;

		public void OnValidate()
		{
			ScriptableObject[] assets = Assets;
			foreach (ScriptableObject scriptableObject in assets)
			{
			}
		}
	}
}
