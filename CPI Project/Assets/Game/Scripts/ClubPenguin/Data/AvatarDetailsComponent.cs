using ClubPenguin.Avatar;
using System;
using UnityEngine;

namespace ClubPenguin.Data
{
	public class AvatarDetailsComponent
	{
		public static readonly Color DefaultColor = new Color32(25, 210, 214, byte.MaxValue);

		private Color bodyColor;

		private DCustomEquipment[] outfit;

		public Color BodyColor
		{
			get
			{
				if (bodyColor != Color.clear)
				{
					return bodyColor;
				}
				return DefaultColor;
			}
			set
			{
				if (bodyColor != value)
				{
					avatarDetailsChanged();
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
				outfit = value;
				avatarDetailsChanged();
			}
		}

		public event Action<DCustomEquipment[], Color> AvatarDetailsChanged;

		public AvatarDetailsComponent(DCustomEquipment[] outfit, Color bodyColor)
		{
			this.outfit = outfit;
			this.bodyColor = bodyColor;
		}

		public AvatarDetailsComponent(DCustomEquipment[] outfit)
			: this(outfit, DefaultColor)
		{
		}

		private void avatarDetailsChanged()
		{
			if (this.AvatarDetailsChanged != null)
			{
				this.AvatarDetailsChanged(outfit, bodyColor);
			}
		}
	}
}
