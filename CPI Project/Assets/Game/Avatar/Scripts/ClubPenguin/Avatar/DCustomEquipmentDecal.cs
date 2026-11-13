using System;

namespace ClubPenguin.Avatar
{
	[Serializable]
	public struct DCustomEquipmentDecal
	{
		public EquipmentDecalType Type;

		public string TextureName;

		public int DefinitionId;

		public int Index;

		public float Scale;

		public float Uoffset;

		public float Voffset;

		public float Rotation;

		public bool Repeat;

		public override string ToString()
		{
			return string.Format("[DCustomEquipmentDecal] DefinitionId: {0} Name: {1} Type: {2} Full hash: {3:x8} Resource hash: {4:x8}", DefinitionId, TextureName, Type, GetFullHash(), GetResourceHash());
		}

		public int GetResourceHash()
		{
			StructHash sh = default(StructHash);
			sh.Combine(TextureName ?? string.Empty);
			return sh;
		}

		public int GetFullHash()
		{
			StructHash sh = default(StructHash);
			sh.Combine(Type);
			sh.Combine(TextureName ?? string.Empty);
			sh.Combine(DefinitionId);
			sh.Combine(Index);
			sh.Combine(Scale);
			sh.Combine(Uoffset);
			sh.Combine(Voffset);
			sh.Combine(Rotation);
			sh.Combine(Repeat);
			return sh;
		}

		public bool Validate()
		{
			bool flag = true;
			flag &= !string.IsNullOrEmpty(TextureName);
			return flag & (DefinitionId != 0);
		}
	}
}
