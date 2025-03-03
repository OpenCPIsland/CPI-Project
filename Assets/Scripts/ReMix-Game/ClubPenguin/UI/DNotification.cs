using Disney.Kelowna.Common;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class DNotification
	{
		public enum NotificationType
		{
			Generic,
			DailyComplete,
			FriendRequest,
			MembershipBanner,
			ActivityTracker
		}

		private static Color DEFAULT_HEADER_TINT = new Color32(0, 95, 202, byte.MaxValue);

		private bool autoClose = true;

		private bool persistBetweenScenes = true;

		private bool adjustRectPositionForNotification = true;

		private Color headerTint = DEFAULT_HEADER_TINT;

		public PrefabContentKey PrefabLocation
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public bool ContainsButtons
		{
			get;
			set;
		}

		public bool AutoClose
		{
			get
			{
				return autoClose;
			}
			set
			{
				autoClose = value;
			}
		}

		public float PopUpDelayTime
		{
			get;
			set;
		}

		public object DataPayload
		{
			get;
			set;
		}

		public bool PersistBetweenScenes
		{
			get
			{
				return persistBetweenScenes;
			}
			set
			{
				persistBetweenScenes = value;
			}
		}

		public NotificationType Type
		{
			get;
			set;
		}

		public bool AdjustRectPositionForNotification
		{
			get
			{
				return adjustRectPositionForNotification;
			}
			set
			{
				adjustRectPositionForNotification = value;
			}
		}

		public Color HeaderTint
		{
			get
			{
				return headerTint;
			}
			set
			{
				headerTint = value;
			}
		}

		public bool WaitForLoading
		{
			get;
			set;
		}

		public bool ShowAfterSceneLoad
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("[DNotification: PrefabLocation={0}, Message={1}, ContainsButtons={2}, AutoClose={3}, PopUpDelayTime={4}, PersistBetweenScenes={5}, Type={6}]", PrefabLocation, Message, ContainsButtons, AutoClose, PopUpDelayTime, PersistBetweenScenes, Type.ToString());
		}
	}
}
