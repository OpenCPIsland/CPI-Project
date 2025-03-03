using ClubPenguin.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Switches
{
	public class AnimationSwitch : Switch
	{
		public int AnimLayer = 0;

		public string[] FullAnimPathNames = new string[0];

		private EventDispatcher eventDispatcher;

		private DataEntityHandle localPlayerHandle;

		private CPDataEntityCollection dataEntityCollection;

		private int[] animHashes;

		private Animator anim;

		private int curAnim;

		public override string GetSwitchType()
		{
			throw new NotImplementedException();
		}

		public override object GetSwitchParameters()
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			if (FullAnimPathNames.Length > 0)
			{
				animHashes = new int[FullAnimPathNames.Length];
				for (int i = 0; i < animHashes.Length; i++)
				{
					animHashes[i] = Animator.StringToHash(FullAnimPathNames[i]);
				}
			}
			else
			{
				base.enabled = false;
			}
			eventDispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			if (localPlayerHandle.IsNull || !dataEntityCollection.HasComponent<PresenceData>(localPlayerHandle))
			{
				eventDispatcher.AddListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerAdded);
			}
			else
			{
				anim = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<Animator>();
			}
			Change(false);
		}

		public void OnDestroy()
		{
			if (localPlayerHandle.IsNull)
			{
				eventDispatcher.RemoveListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerAdded);
			}
		}

		private bool onLocalPlayerAdded(NetworkControllerEvents.LocalPlayerJoinedRoomEvent evt)
		{
			eventDispatcher.RemoveListener<NetworkControllerEvents.LocalPlayerJoinedRoomEvent>(onLocalPlayerAdded);
			localPlayerHandle = evt.Handle;
			anim = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<Animator>();
			return false;
		}

		public void LateUpdate()
		{
			if (!(anim != null))
			{
				return;
			}
			int fullPathHash = anim.GetCurrentAnimatorStateInfo(AnimLayer).fullPathHash;
			if (fullPathHash == curAnim)
			{
				return;
			}
			bool onoff = false;
			for (int i = 0; i < animHashes.Length; i++)
			{
				if (fullPathHash == animHashes[i])
				{
					onoff = true;
					break;
				}
			}
			curAnim = fullPathHash;
			Change(onoff);
		}
	}
}
