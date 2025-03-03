using ClubPenguin.Cinematography;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Interactables
{
	public class InvitationalItemController : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> ItemPieces;

		[SerializeField]
		public int TotalPieces = 6;

		public int AvailableQuantity
		{
			get
			{
				return AvailableQuantity;
			}
			set
			{
				int num = 0;
				if (value >= 0)
				{
					num = Mathf.CeilToInt((float)value / (float)TotalPieces * (float)ItemPieces.Count);
				}
				if (value < TotalPieces)
				{
					num = Mathf.Min(num, ItemPieces.Count - 1);
				}
				for (int i = 0; i < ItemPieces.Count; i++)
				{
					if (i < num)
					{
						ItemPieces[i].SetActive(true);
					}
					else
					{
						ItemPieces[i].SetActive(false);
					}
				}
			}
		}

		public void Start()
		{
			CameraCullingMaskHelper.SetLayerIncludingChildren(base.gameObject.transform, "AllPlayerInteractibles", true);
		}
	}
}
