using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using System;
using UnityEngine;

namespace ClubPenguin
{
	[Serializable]
	public class PresenceData : BaseData
	{
		[SerializeField]
		private string world;

		[SerializeField]
		private string room;

		[SerializeField]
		private string contentIdentifier;

		[SerializeField]
		private ZoneId instanceRoom;

		[SerializeField]
		private bool isNotInCurrentRoomsScene;

		[SerializeField]
		private bool isAway;

		[SerializeField]
		private AwayFromKeyboardState afkState = new AwayFromKeyboardState(AwayFromKeyboardStateType.Here, null);

		[SerializeField]
		private TemporaryHeadStatusType temporaryHeadStatusType = TemporaryHeadStatusType.None;

		[SerializeField]
		private bool isDisconnecting;

		public string World
		{
			get
			{
				return world;
			}
			set
			{
				world = value;
				dispatchUpdatedAction();
			}
		}

		public string Room
		{
			get
			{
				return room;
			}
			set
			{
				room = value;
				dispatchUpdatedAction();
			}
		}

		public string ContentIdentifier
		{
			get
			{
				return contentIdentifier;
			}
			set
			{
				contentIdentifier = value;
				dispatchUpdatedAction();
			}
		}

		public bool IsInInstancedRoom
		{
			get
			{
				if (instanceRoom == null)
				{
					return false;
				}
				return !instanceRoom.isEmpty();
			}
		}

		public ZoneId InstanceRoom
		{
			get
			{
				return instanceRoom;
			}
			set
			{
				instanceRoom = value;
				dispatchUpdatedAction();
			}
		}

		public bool IsNotInCurrentRoomsScene
		{
			get
			{
				return isNotInCurrentRoomsScene;
			}
			set
			{
				isNotInCurrentRoomsScene = value;
			}
		}

		public AwayFromKeyboardState AFKState
		{
			get
			{
				return afkState;
			}
			set
			{
				bool flag = value != afkState;
				afkState = value;
				if (flag)
				{
					dispatchUpdatedAction();
				}
			}
		}

		public TemporaryHeadStatusType TemporaryHeadStatusType
		{
			get
			{
				return temporaryHeadStatusType;
			}
			set
			{
				bool flag = value != temporaryHeadStatusType;
				temporaryHeadStatusType = value;
				if (flag && this.TemporaryHeadStatusUpdated != null)
				{
					this.TemporaryHeadStatusUpdated(this);
				}
			}
		}

		public bool IsAway
		{
			get
			{
				return isAway;
			}
			set
			{
				isAway = value;
				dispatchUpdatedAction();
			}
		}

		public bool IsDisconnecting
		{
			get
			{
				return isDisconnecting;
			}
			set
			{
				isDisconnecting = value;
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(PresenceDataMonoBehaviour);
			}
		}

		public event Action<PresenceData> PresenceDataUpdated;

		public event Action<PresenceData> TemporaryHeadStatusUpdated;

		private void dispatchUpdatedAction()
		{
			this.PresenceDataUpdated.InvokeSafe(this);
		}

		protected override void notifyWillBeDestroyed()
		{
			this.PresenceDataUpdated = null;
		}
	}
}
