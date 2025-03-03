using System;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public struct DCustomEquipmentPart
	{
		public int SlotIndex;

		public DCustomEquipmentDecal[] Decals;

		public override string ToString()
		{
			return string.Format("[DCustomEquipmentPart] SlotIndex: {0} #Decals: {1} Full hash: {2:x8} Resource Hash: {3:x8}", SlotIndex, (Decals != null) ? Decals.Length.ToString() : "-", GetFullHash(), GetResourceHash());
		}

		public int GetResourceHash()
		{
			StructHash sh = default(StructHash);
			if (Decals != null)
			{
				for (int i = 0; i < Decals.Length; i++)
				{
					sh.Combine(Decals[i].GetResourceHash());
				}
			}
			return sh;
		}

		public int GetFullHash()
		{
			StructHash sh = default(StructHash);
			sh.Combine(SlotIndex);
			if (Decals != null)
			{
				for (int i = 0; i < Decals.Length; i++)
				{
					sh.Combine(Decals[i].GetFullHash());
				}
			}
			return sh;
		}

		public bool Validate()
		{
			bool flag = true;
			flag &= (SlotIndex >= 0);
			flag &= (Decals != null);
			int num = 0;
			while (flag && num < Decals.Length)
			{
				flag &= Decals[num].Validate();
				num++;
			}
			return flag;
		}
	}
}
