using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.UI
{
	public static class HudEvents
	{
		public struct SetNavigationTarget
		{
			public readonly Transform TargetTransform;

			public readonly Vector3 OnscreenIndicatorOffset;

			public readonly bool ShowOnscreenIndicator;

			public SetNavigationTarget(Transform targetTransform, bool showOnscreenIndicator = true, Vector3 onscreenIndicatorOffset = default(Vector3))
			{
				TargetTransform = targetTransform;
				ShowOnscreenIndicator = showOnscreenIndicator;
				OnscreenIndicatorOffset = onscreenIndicatorOffset;
			}
		}

		public struct SetObjectiveText
		{
			public readonly string ObjectiveText;

			public SetObjectiveText(string objectiveText)
			{
				ObjectiveText = objectiveText;
			}
		}

		public struct SetSubtaskText
		{
			public string ID;

			public string SubtaskText;

			public bool IsComplete;

			public SetSubtaskText(string id, string subtaskText, bool isComplete)
			{
				ID = id;
				SubtaskText = subtaskText;
				IsComplete = isComplete;
			}
		}

		public struct RemoveSubtaskText
		{
			public string ID;

			public RemoveSubtaskText(string id)
			{
				ID = id;
			}
		}

		public struct DestroySubtaskText
		{
			public bool PlayAnimation;

			public DestroySubtaskText(bool playAnimation)
			{
				PlayAnimation = playAnimation;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ResetQuestNotifier
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideQuestNavigation
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowQuestNavigation
		{
		}

		public struct UpdateNavigationDistance
		{
			public readonly int Distance;

			public UpdateNavigationDistance(int distance)
			{
				Distance = distance;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowNavigationDistance
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideNavigationDistance
		{
		}

		public struct SetNavigationStyle
		{
			public readonly bool Use3DNavigation;

			public readonly bool UseZNavigationFor2D;

			public SetNavigationStyle(bool use3DNavigation, bool useZNavigationFor2D = false)
			{
				Use3DNavigation = use3DNavigation;
				UseZNavigationFor2D = useZNavigationFor2D;
			}
		}

		public struct PermanentlySuppressQuestNotifier
		{
			public readonly bool Suppress;

			public readonly bool AutoShow;

			public PermanentlySuppressQuestNotifier(bool suppress, bool autoShow = true)
			{
				Suppress = suppress;
				AutoShow = autoShow;
			}
		}

		public struct SuppressQuestNotifier
		{
			public readonly bool Suppress;

			public readonly bool AutoShow;

			public SuppressQuestNotifier(bool suppress = true, bool autoShow = true)
			{
				Suppress = suppress;
				AutoShow = autoShow;
			}
		}

		public struct ShowHideQuestNotifier
		{
			public readonly bool Show;

			public ShowHideQuestNotifier(bool show)
			{
				Show = show;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct XPAdditionStart
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct XPAdditionStop
		{
		}

		public struct ShowQuestMessage
		{
			public readonly DQuestMessage SpeechData;

			public ShowQuestMessage(DQuestMessage speechData)
			{
				SpeechData = speechData;
			}
		}

		public struct QuestMessageComplete
		{
			public readonly DQuestMessage SpeechData;

			public QuestMessageComplete(DQuestMessage speechData)
			{
				SpeechData = speechData;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HudInitComplete
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CoinAdditionStart
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CoinAdditionStop
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideWaypointArrow
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SuppressCoinDisplay
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct UnsuppressCoinDisplay
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SortingOrderUpdated
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideCellPhoneHud
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowCellPhoneHud
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct HideIglooEditButton
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ShowIglooEditButton
		{
		}

		public struct StartSnowballPowerMeter
		{
			public readonly float MaxChargeTime;

			public StartSnowballPowerMeter(float maxChargeTime)
			{
				MaxChargeTime = maxChargeTime;
			}
		}

		public struct StopSnowballPowerMeter
		{
			public readonly bool WasCancelled;

			public StopSnowballPowerMeter(bool wasCancelled = false)
			{
				WasCancelled = wasCancelled;
			}
		}
	}
}
