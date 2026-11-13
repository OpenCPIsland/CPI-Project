using System;
using UnityEngine;

namespace ClubPenguin.DCE
{
	[DisallowMultipleComponent]
	public class DceModel : MonoBehaviour
	{
		public class Part
		{
		}

		public DecorationDefinition Definition;

#pragma warning disable 0067
		public event Action<int, int, Part, Part> PartChanged;

		public event Action OutfitSet;
#pragma warning restore 0067

		public void Awake()
		{
		}

		public void ApplyDecoration()
		{
		}

		public bool RemoveDecoration(string name)
		{
			return false;
		}

		public void ClearDecoration()
		{
		}
	}
}
