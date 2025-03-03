using ClubPenguin.Avatar;
using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class AvatarDetailsData : BaseData
	{
		[SerializeField]
		private Color bodyColor;

		[SerializeField]
		private DCustomEquipment[] outfit;

		public Color BodyColor
		{
			get
			{
				if (bodyColor != Color.clear)
				{
					return bodyColor;
				}
				return AvatarService.DefaultBodyColor;
			}
			set
			{
				if (bodyColor != value)
				{
					changedBodyColor(value);
					bodyColor = value;
				}
			}
		}

		public DCustomEquipment[] Outfit
		{
			get
			{
				return outfit;
			}
			set
			{
				if (hasOutfitChanged(outfit, value))
				{
					changedOutfit(value);
					outfit = value;
				}
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(AvatarDetailsDataMonoBehaviour);
			}
		}

		public event Action<AvatarDetailsData> OnInitialized;

		public event Action<DCustomEquipment[]> PlayerOutfitChanged;

		public event Action<Color> PlayerColorChanged;

		public void Init(DCustomEquipment[] outfit, Color bodyColor)
		{
			this.outfit = outfit;
			this.bodyColor = bodyColor;
			if (this.OnInitialized != null)
			{
				this.OnInitialized(this);
			}
		}

		public void Init(DCustomEquipment[] outfit)
		{
			Init(outfit, AvatarService.DefaultBodyColor);
		}

		protected override void notifyWillBeDestroyed()
		{
			this.PlayerOutfitChanged = null;
			this.PlayerColorChanged = null;
			this.OnInitialized = null;
		}

		private void changedOutfit(DCustomEquipment[] outfit)
		{
			if (this.PlayerOutfitChanged != null)
			{
				this.PlayerOutfitChanged(outfit);
			}
		}

		private void changedBodyColor(Color color)
		{
			if (this.PlayerColorChanged != null)
			{
				this.PlayerColorChanged(color);
			}
		}

		private bool hasOutfitChanged(DCustomEquipment[] existingOutfit, DCustomEquipment[] newOutfit)
		{
			if (existingOutfit == null)
			{
				return newOutfit != null;
			}
			if (newOutfit == null)
			{
				return existingOutfit != null;
			}
			if (newOutfit.Length != existingOutfit.Length)
			{
				return true;
			}
			for (int i = 0; i < newOutfit.Length; i++)
			{
				if (newOutfit[i].Id != existingOutfit[i].Id)
				{
					return true;
				}
			}
			return false;
		}
	}
}
