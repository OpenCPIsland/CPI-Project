using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class AvatarLocomotionStateSetter : MonoBehaviour
	{
		public delegate void ActionButtonInvokedDelegate(LocomotionAction action);

		public delegate void GenericStateButtonInvokedDelegate(LocomotionState state);

		private AvatarView avatarView;

		private CPDataEntityCollection dataEntityCollection;

		private LocomotionData locomotionData;

		public event ActionButtonInvokedDelegate ActionButtonInvoked;

		public event GenericStateButtonInvokedDelegate GenericStateButtonInvoked;

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			avatarView = GetComponent<AvatarView>();
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle))
			{
				if (dataEntityCollection.HasComponent<RemotePlayerData>(handle))
				{
					if (dataEntityCollection.TryGetComponent(handle, out locomotionData))
					{
						addListeners();
					}
					else
					{
						Log.LogError(this, "Entity missing a LocomotionData component, locomotion state listeners not added");
					}
				}
			}
			else
			{
				dataEntityCollection.EventDispatcher.AddListener<DataEntityEvents.ComponentAddedEvent<LocomotionData>>(onLocomotionDataAdded);
			}
		}

		private bool onLocomotionDataAdded(DataEntityEvents.ComponentAddedEvent<LocomotionData> evt)
		{
			DataEntityHandle handle;
			if (AvatarDataHandle.TryGetPlayerHandle(base.gameObject, out handle) && evt.Handle == handle)
			{
				dataEntityCollection.EventDispatcher.RemoveListener<DataEntityEvents.ComponentAddedEvent<LocomotionData>>(onLocomotionDataAdded);
				locomotionData = evt.Component;
				addListeners();
			}
			return false;
		}

		private void OnDestroy()
		{
			removeListeners();
		}

		private void addListeners()
		{
			if (avatarView != null)
			{
				avatarView.OnReady += onViewReady;
			}
			if (locomotionData != null)
			{
				locomotionData.PlayerLocoStateChanged += OnPlayerLocoStateChanged;
			}
		}

		private void removeListeners()
		{
			if (avatarView != null)
			{
				avatarView.OnReady -= onViewReady;
			}
			if (locomotionData != null)
			{
				locomotionData.PlayerLocoStateChanged -= OnPlayerLocoStateChanged;
			}
		}

		private void onViewReady(AvatarBaseAsync view)
		{
			if (locomotionData == null)
			{
				return;
			}
			LocomotionTracker component = GetComponent<LocomotionTracker>();
			switch (locomotionData.LocoState)
			{
			case LocomotionState.Slide:
				if (component.IsCurrentControllerOfType<SlideController>())
				{
					GetComponent<Animator>().SetBool(AnimationHashes.Params.Slide, true);
					GetComponent<Animator>().Play(AnimationHashes.States.Slide.Enter, 0);
				}
				else
				{
					component.SetCurrentController<SlideController>();
				}
				break;
			case LocomotionState.Sitting:
			{
				if (!component.IsCurrentControllerOfType<SitController>())
				{
					component.SetCurrentController<SitController>();
				}
				SitController sitController = component.GetCurrentController() as SitController;
				if (sitController != null)
				{
					sitController.RemoteSit(null);
				}
				break;
			}
			}
		}

		public void ActionButton(LocomotionAction action)
		{
			if (this.ActionButtonInvoked != null)
			{
				this.ActionButtonInvoked(action);
			}
		}

		private void OnPlayerLocoStateChanged(LocomotionState state)
		{
			if (this.GenericStateButtonInvoked != null)
			{
				this.GenericStateButtonInvoked(state);
			}
		}
	}
}
