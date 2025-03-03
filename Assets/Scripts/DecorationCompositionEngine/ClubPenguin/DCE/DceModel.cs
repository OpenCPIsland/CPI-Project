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

		public event Action<int, int, Part, Part> PartChanged;

		public event Action OutfitSet;

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
