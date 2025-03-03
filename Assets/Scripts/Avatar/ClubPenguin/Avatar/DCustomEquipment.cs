using System;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public struct DCustomEquipment
	{
		public long Id;

		public int DefinitionId;

		public string Name;

		public DCustomEquipmentPart[] Parts;

		public long DateTimeCreated;

		public override string ToString()
		{
			return string.Format("[DCustomEquipment] DefinitionId: {0} Name: {1} Id: {2} #Parts: {3} Full hash: {4:x8}, Resource hash: {5:x8}", DefinitionId, Name, Id, (Parts != null) ? Parts.Length.ToString() : "-", GetFullHash(), GetResourceHash());
		}

		public int GetResourceHash()
		{
			StructHash sh = default(StructHash);
			sh.Combine(Name ?? string.Empty);
			if (Parts != null)
			{
				for (int i = 0; i < Parts.Length; i++)
				{
					sh.Combine(Parts[i].GetResourceHash());
				}
			}
			return sh;
		}

		public int GetFullHash()
		{
			StructHash sh = default(StructHash);
			sh.Combine(Id);
			sh.Combine(DefinitionId);
			sh.Combine(Name ?? string.Empty);
			if (Parts != null)
			{
				for (int i = 0; i < Parts.Length; i++)
				{
					sh.Combine(Parts[i].GetFullHash());
				}
			}
			return sh;
		}

		public bool Validate()
		{
			bool flag = true;
			flag &= (Id != 0);
			flag &= (DefinitionId != 0);
			flag &= !string.IsNullOrEmpty(Name);
			flag &= (Parts != null);
			int num = 0;
			while (flag && num < Parts.Length)
			{
				flag &= Parts[num].Validate();
				num++;
			}
			return flag;
		}
	}
}
