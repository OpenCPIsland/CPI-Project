using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class InteractableGroup : MonoBehaviour
	{
		public List<Selectable> Interactables;

		private bool isInteractable;

		public bool IsInteractable
		{
			get
			{
				return isInteractable;
			}
			set
			{
				isInteractable = value;
				for (int i = 0; i < Interactables.Count; i++)
				{
					Interactables[i].interactable = isInteractable;
				}
			}
		}
	}
}
