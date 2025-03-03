using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	public static class IglooButtonUtils
	{
		private enum ButtonState
		{
			Enabled,
			Disabled
		}

		public static int IGLOO_TUTORIAL_ID = 19;

		public static bool SetButtonState(DataEntityHandle handle, GameObject buttonGameObject)
		{
			bool flag = Service.Get<ZoneTransitionService>().IsInIgloo && Service.Get<SceneLayoutDataManager>().IsInOwnIgloo();
			ButtonState index = flag ? ButtonState.Disabled : ButtonState.Enabled;
			SpriteSelector componentInChildren = buttonGameObject.GetComponentInChildren<SpriteSelector>();
			if (componentInChildren != null)
			{
				componentInChildren.Select((int)index);
			}
			return flag;
		}
	}
}
