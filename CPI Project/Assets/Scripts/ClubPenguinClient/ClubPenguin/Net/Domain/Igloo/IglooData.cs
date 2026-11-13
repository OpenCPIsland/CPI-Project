using ClubPenguin.Net.Domain.Scene;
using System;

namespace ClubPenguin.Net.Domain.Igloo
{
	[Serializable]
	public class IglooData : MutableIglooData
	{
		public int maxIglooLayouts;

		public SceneLayout activeLayout;
	}
}
