using ClubPenguin.ClothingDesigner.ItemCustomizer;
using ClubPenguin.Collectibles;
using ClubPenguin.Locomotion;
using ClubPenguin.UI;
using UnityEngine;

namespace ClubPenguin
{
	public static class SceneRefs
	{
		public static ActionSequencer ActionSequencer
		{
			get;
			private set;
		}

		public static ZiplineConnector ZiplineConnector
		{
			get;
			private set;
		}

		public static CollectibleController CollectibleController
		{
			get;
			private set;
		}

		public static ClothingDesignerCameraController ClothingDesignerCameraController
		{
			get;
			private set;
		}

		public static CelebrationRunner CelebrationRunner
		{
			get;
			private set;
		}

		public static GameObject UiTrayRoot
		{
			get;
			private set;
		}

		public static GameObject UiChatRoot
		{
			get;
			private set;
		}

		public static ZoneLocalPlayerManager ZoneLocalPlayerManager
		{
			get;
			private set;
		}

		public static ZoneRemotePlayerManager ZoneRemotePlayerManager
		{
			get;
			private set;
		}

		public static PopupManager PopupManager
		{
			get;
			private set;
		}

		public static FullScreenPopupManager FullScreenPopupManager
		{
			get;
			set;
		}

		public static void SetActionSequencer(ActionSequencer actionSequencer)
		{
			ActionSequencer = actionSequencer;
		}

		public static void SetPopupManager(PopupManager popupManager)
		{
			PopupManager = popupManager;
		}

		public static void SetZiplineConnector(ZiplineConnector ziplineConnector)
		{
			ZiplineConnector = ziplineConnector;
		}

		public static void SetCollectibleController(CollectibleController collectibleController)
		{
			CollectibleController = collectibleController;
		}

		public static void SetClothingDesignerCameraController(ClothingDesignerCameraController clothingDesignerCameraController)
		{
			ClothingDesignerCameraController = clothingDesignerCameraController;
		}

		public static void SetCelebrationRunner(CelebrationRunner celebrationRunner)
		{
			CelebrationRunner = celebrationRunner;
		}

		public static void SetZoneLocalPlayerManager(ZoneLocalPlayerManager zoneLocalPlayerManager)
		{
			ZoneLocalPlayerManager = zoneLocalPlayerManager;
		}

		public static void SetZoneRemotePlayerManager(ZoneRemotePlayerManager zoneRemotePlayerManager)
		{
			ZoneRemotePlayerManager = zoneRemotePlayerManager;
		}

		public static void SetUiTrayRoot(GameObject uiTrayRoot)
		{
			UiTrayRoot = uiTrayRoot;
		}

		public static void SetUiChatRoot(GameObject uiChatRoot)
		{
			UiChatRoot = uiChatRoot;
		}

		public static void ClearAll()
		{
			ActionSequencer = null;
			PopupManager = null;
			ZiplineConnector = null;
			CollectibleController = null;
			ClothingDesignerCameraController = null;
			CelebrationRunner = null;
			ZoneLocalPlayerManager = null;
			UiTrayRoot = null;
			UiChatRoot = null;
		}
	}
}
