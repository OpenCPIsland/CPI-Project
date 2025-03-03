using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Input
{
	public class InputMapPriority : ScriptableObject
	{
		public List<InputMapLib> PriorityList;

		public InputMapLib ModalInputMap;

		public InputMapLib AnyKeyInputMap;
	}
}
