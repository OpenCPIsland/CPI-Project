using ClubPenguin.Core;
using ClubPenguin.DecorationInventory;
using ClubPenguin.ObjectManipulation;
using Disney.Kelowna.Common;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	public static class IglooUIEvents
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LotNextButtonPressed
		{
		}

		public struct IglooEditPressed
		{
			public readonly bool ReloadScene;

			public readonly long LayoutId;

			public IglooEditPressed(bool reloadScene = false, long layoutId = -1L)
			{
				ReloadScene = reloadScene;
				LayoutId = layoutId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LotBackButtonPressed
		{
		}

		public struct CreateIglooButtonPressed
		{
			public readonly SceneLayoutData SceneLayoutData;

			public CreateIglooButtonPressed(SceneLayoutData sceneLayoutData)
			{
				SceneLayoutData = sceneLayoutData;
			}
		}

		public struct SetStateButtonPressed
		{
			public readonly SceneStateData.SceneState SceneState;

			public readonly bool ReloadScene;

			public readonly PrefabContentKey SplashScreen;

			public readonly long LayoutId;

			public SetStateButtonPressed(SceneStateData.SceneState sceneState, bool reloadScene, PrefabContentKey splashScreen, long layoutId)
			{
				SceneState = sceneState;
				ReloadScene = reloadScene;
				SplashScreen = splashScreen;
				LayoutId = layoutId;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct OpenManageIglooPopup
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ManageIglooPopupDisplayed
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct StateTogglingComplete
		{
		}

		public struct CloseManageIglooPopup
		{
			public readonly SceneLayoutData SceneLayoutData;

			public CloseManageIglooPopup(SceneLayoutData sceneLayoutData = null)
			{
				SceneLayoutData = sceneLayoutData;
			}
		}

		public struct SwapScene
		{
			public readonly string SceneName;

			public readonly Dictionary<string, object> SceneArgs;

			public SwapScene(string sceneName, Dictionary<string, object> sceneArgs = null)
			{
				SceneName = sceneName;
				SceneArgs = sceneArgs;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SwapSceneComplete
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct LotScreenReady
		{
		}

		public struct AddNewDecoration
		{
			public readonly DecorationDefinition Definition;

			public readonly Vector2 FinalTouchPoint;

			public AddNewDecoration(DecorationDefinition definition, Vector2 finalTouchPoint)
			{
				Definition = definition;
				FinalTouchPoint = finalTouchPoint;
			}
		}

		public struct AddNewStructure
		{
			public readonly StructureDefinition Definition;

			public readonly Vector2 FinalTouchPoint;

			public AddNewStructure(StructureDefinition definition, Vector2 finalTouchPoint)
			{
				Definition = definition;
				FinalTouchPoint = finalTouchPoint;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct DuplicateSelectedObject
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct ClearCurrentLayout
		{
		}

		public struct ShowNotification
		{
			public readonly PrefabContentKey Prefab;

			public readonly string TranslatedText;

			public readonly float DisplayTime;

			public readonly bool AdjustRectPositionForNotification;

			public readonly bool ShowAfterSceneLoad;

			public ShowNotification(string translatedText, float displayTime, PrefabContentKey prefab, bool adjustRectPositionForNotification, bool showAfterSceneLoad)
			{
				TranslatedText = translatedText;
				DisplayTime = displayTime;
				Prefab = prefab;
				AdjustRectPositionForNotification = adjustRectPositionForNotification;
				ShowAfterSceneLoad = showAfterSceneLoad;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct IglooSceneLoad
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct MaxItemsLimitReached
		{
		}

		public struct ShowSelectedUIWidget
		{
			public readonly ManipulatableObject ManipulatableObject;

			public readonly Bounds BoundsForCameraTarget;

			public readonly float MinCameraDistance;

			public ShowSelectedUIWidget(ManipulatableObject manipulatableObject, Bounds boundsForCameraTarget, float minCameraDistance)
			{
				ManipulatableObject = manipulatableObject;
				BoundsForCameraTarget = boundsForCameraTarget;
				MinCameraDistance = minCameraDistance;
			}
		}
	}
}
