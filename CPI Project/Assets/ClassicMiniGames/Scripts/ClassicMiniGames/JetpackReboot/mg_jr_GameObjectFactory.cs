using MinigameFramework;
using UnityEngine;

namespace JetpackReboot
{
	public abstract class mg_jr_GameObjectFactory : mg_ICreator<GameObject>
	{
		protected mg_JetpackReboot MiniGame
		{
			get;
			private set;
		}

		protected mg_jr_Resources Resources
		{
			get;
			private set;
		}

		protected mg_jr_GameObjectFactory()
		{
			MiniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			Resources = MiniGame.Resources;
		}

		public abstract GameObject Create();
	}
}
