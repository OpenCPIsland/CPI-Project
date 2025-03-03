using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Rewards;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class RemotePlayerRewardManager : MonoBehaviour
	{
		private EventChannel eventChannel;

		private CPDataEntityCollection dataEntityCollection;

		private AvatarDataHandle avatarDataHandle;

		private void Awake()
		{
			avatarDataHandle = GetComponent<AvatarDataHandle>();
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
		}

		private void OnEnable()
		{
			eventChannel.AddListener<RewardServiceEvents.LevelUp>(onLevelUp);
		}

		private void OnDisable()
		{
			eventChannel.RemoveAllListeners();
		}

		private bool onLevelUp(RewardServiceEvents.LevelUp evt)
		{
			if (!base.gameObject.IsDestroyed() && avatarDataHandle != null && !avatarDataHandle.Handle.IsNull)
			{
				SessionIdData component = dataEntityCollection.GetComponent<SessionIdData>(avatarDataHandle.Handle);
				if (component != null && evt.SessionId == component.SessionId)
				{
					base.gameObject.AddComponent<LevelUpParticlesRemote>();
				}
			}
			return false;
		}
	}
}
