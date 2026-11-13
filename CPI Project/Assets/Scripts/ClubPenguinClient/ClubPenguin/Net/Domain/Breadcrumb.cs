using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct Breadcrumb
	{
		public int breadcrumbType;

		public string id;

		public Breadcrumb(string id, int type)
		{
			this.id = id;
			breadcrumbType = type;
		}

		public override string ToString()
		{
			return string.Format("[Breadcrumb] Type: {0} Id: {1}", breadcrumbType, id);
		}
	}
}
