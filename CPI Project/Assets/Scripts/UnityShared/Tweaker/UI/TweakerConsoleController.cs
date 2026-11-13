#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
using Disney.Kelowna.Common;
using System.Collections;
using System.Linq;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tweaker.UI
{
	public class TweakerConsoleController : MonoBehaviour, ITweakerConsoleController
	{
		public InspectorView InspectorViewPrefab;

		public HexGridController GridController;

		private IInspectorController inspector;

		private ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		private bool isLandscape;

		private KeyBindingManager keyBindingManager;

		private bool debugGestureWasActive;

		public Tweaker Tweaker
		{
			get;
			private set;
		}

		public TweakerTree Tree
		{
			get;
			private set;
		}

		public ITweakerSerializer Serializer
		{
			get;
			private set;
		}

		public BaseNode CurrentInspectorNode
		{
			get;
			private set;
		}

		public void OnEnable()
		{
			if (Tree != null)
			{
				Refresh();
			}
		}

		public static bool IsLandscape()
		{
			return Screen.height < Screen.width;
		}

		public static bool IsPortrait()
		{
			return !IsLandscape();
		}

		public void Update()
		{
			if (isLandscape != IsLandscape())
			{
				isLandscape = IsLandscape();
				if (GridController != null)
				{
					GridController.Resize();
				}
				if (inspector != null)
				{
					inspector.Resize();
				}
			}
		}

		public void Init(Tweaker tweaker, ITweakerSerializer serializer)
		{
			logger.Info("Init: " + tweaker);
			Tweaker = tweaker;
			Serializer = serializer;
			Tweaker.Scanner.ScanInstance(GridController);
			GridController.Init();
			Refresh();
			keyBindingManager = base.gameObject.transform.parent.gameObject.AddComponent<KeyBindingManager>();
			keyBindingManager.Init(Tweaker.Invokables.GetInvokables().Values.ToArray());
			CoroutineRunner.StartPersistent(checkShouldActivate(), this, "checkShouldActivate");
		}

		[Invokable("Tweaker.UI.Refresh", Description = "Repopulate the tweaker tree and refresh the hex grid.")]
		public void Refresh()
		{
			Tree = new TweakerTree(Tweaker);
			Tree.BuildTree();
			isLandscape = IsLandscape();
			GridController.Refresh();
		}

		public void ShowInspector(BaseNode nodeToInspect)
		{
			if (inspector != null && inspector.NodeType != nodeToInspect.Type)
			{
				inspector.Destroy();
				inspector = null;
			}
			if (inspector == null)
			{
				CreateInspector(nodeToInspect.Type);
			}
			CurrentInspectorNode = nodeToInspect;
			inspector.InspectNode(nodeToInspect);
		}

		private void CreateInspector(BaseNode.NodeType type)
		{
			InspectorView inspectorView = Object.Instantiate(InspectorViewPrefab);
			inspectorView.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), false);
			inspector = InspectorControllerFactory.MakeController(inspectorView, GridController, type);
			inspector.Closed += InspectorClosed;
		}

		private void InspectorClosed()
		{
			inspector = null;
			CurrentInspectorNode = null;
		}

		public void DestroyObject(GameObject go)
		{
			Object.Destroy(go);
		}

		public void HideConsole()
		{
			base.gameObject.SetActive(false);
		}

		public void ShowConsole()
		{
			base.gameObject.SetActive(true);
		}

		private IEnumerator checkShouldActivate()
		{
			while (true)
			{
				bool debugGestureIsActive = IsDebugGestureActive();
				if (debugGestureIsActive && !debugGestureWasActive)
				{
					ShowConsole();
				}
				debugGestureWasActive = debugGestureIsActive;
				yield return null;
			}
		}

		private bool IsDebugGestureActive()
		{
			bool hasTopLeftTouch = false;
			bool hasBottomRightTouch = false;
			var touchscreen = Touchscreen.current;
			if (touchscreen == null || Screen.width <= 0 || Screen.height <= 0)
			{
				return false;
			}

			for (int i = 0; i < touchscreen.touches.Count; i++)
			{
				var touch = touchscreen.touches[i];
				if (!touch.press.isPressed)
				{
					continue;
				}

				Vector2 position = touch.position.ReadValue();
				float normalizedX = position.x / Screen.width;
				float normalizedY = position.y / Screen.height;
				if (normalizedX < 0.2f && normalizedY > 0.8f)
				{
					hasTopLeftTouch = true;
				}
				else if (normalizedX > 0.8f && normalizedY < 0.2f)
				{
					hasBottomRightTouch = true;
				}
			}

			return hasTopLeftTouch && hasBottomRightTouch;
		}
	}
}
#else
using Disney.Kelowna.Common;
using System.Collections;
using System.Linq;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tweaker.UI
{
	public class TweakerConsoleController : MonoBehaviour, ITweakerConsoleController
	{
		public InspectorView InspectorViewPrefab;

		public HexGridController GridController;

		private IInspectorController inspector;

		private ITweakerLogger logger = LogManager.GetCurrentClassLogger();

		private bool isLandscape;

		private KeyBindingManager keyBindingManager;

		private bool debugGestureWasActive;

		public Tweaker Tweaker
		{
			get;
			private set;
		}

		public TweakerTree Tree
		{
			get;
			private set;
		}

		public ITweakerSerializer Serializer
		{
			get;
			private set;
		}

		public BaseNode CurrentInspectorNode
		{
			get;
			private set;
		}

		public void OnEnable()
		{
			if (Tree != null)
			{
				Refresh();
			}
		}

		public static bool IsLandscape()
		{
			return Screen.height < Screen.width;
		}

		public static bool IsPortrait()
		{
			return !IsLandscape();
		}

		public void Update()
		{
			if (isLandscape != IsLandscape())
			{
				isLandscape = IsLandscape();
				if (GridController != null)
				{
					GridController.Resize();
				}
				if (inspector != null)
				{
					inspector.Resize();
				}
			}
		}

		public void Init(Tweaker tweaker, ITweakerSerializer serializer)
		{
			logger.Info("Init: " + tweaker);
			Tweaker = tweaker;
			Serializer = serializer;
			Tweaker.Scanner.ScanInstance(GridController);
			GridController.Init();
			Refresh();
			keyBindingManager = base.gameObject.transform.parent.gameObject.AddComponent<KeyBindingManager>();
			keyBindingManager.Init(Tweaker.Invokables.GetInvokables().Values.ToArray());
			CoroutineRunner.StartPersistent(checkShouldActivate(), this, "checkShouldActivate");
		}

		[Invokable("Tweaker.UI.Refresh", Description = "Repopulate the tweaker tree and refresh the hex grid.")]
		public void Refresh()
		{
			Tree = new TweakerTree(Tweaker);
			Tree.BuildTree();
			isLandscape = IsLandscape();
			GridController.Refresh();
		}

		public void ShowInspector(BaseNode nodeToInspect)
		{
			if (inspector != null && inspector.NodeType != nodeToInspect.Type)
			{
				inspector.Destroy();
				inspector = null;
			}
			if (inspector == null)
			{
				CreateInspector(nodeToInspect.Type);
			}
			CurrentInspectorNode = nodeToInspect;
			inspector.InspectNode(nodeToInspect);
		}

		private void CreateInspector(BaseNode.NodeType type)
		{
			InspectorView inspectorView = Object.Instantiate(InspectorViewPrefab);
			inspectorView.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), false);
			inspector = InspectorControllerFactory.MakeController(inspectorView, GridController, type);
			inspector.Closed += InspectorClosed;
		}

		private void InspectorClosed()
		{
			inspector = null;
			CurrentInspectorNode = null;
		}

		public void DestroyObject(GameObject go)
		{
			Object.Destroy(go);
		}

		public void HideConsole()
		{
			base.gameObject.SetActive(false);
		}

		public void ShowConsole()
		{
			base.gameObject.SetActive(true);
		}

		private IEnumerator checkShouldActivate()
		{
			while (true)
			{
				if (Keyboard.current != null && Keyboard.current.backquoteKey.wasPressedThisFrame)
				{
					base.gameObject.SetActive(!base.gameObject.activeSelf);
				}

				bool debugGestureIsActive = IsDebugGestureActive();
				if (debugGestureIsActive && !debugGestureWasActive)
				{
					ShowConsole();
				}
				debugGestureWasActive = debugGestureIsActive;
				yield return null;
			}
		}

		private bool IsDebugGestureActive()
		{
			bool hasTopLeftTouch = false;
			bool hasBottomRightTouch = false;
			var touchscreen = Touchscreen.current;
			if (touchscreen == null || Screen.width <= 0 || Screen.height <= 0)
			{
				return false;
			}

			for (int i = 0; i < touchscreen.touches.Count; i++)
			{
				var touch = touchscreen.touches[i];
				if (!touch.press.isPressed)
				{
					continue;
				}

				Vector2 position = touch.position.ReadValue();
				float normalizedX = position.x / Screen.width;
				float normalizedY = position.y / Screen.height;
				if (normalizedX < 0.2f && normalizedY > 0.8f)
				{
					hasTopLeftTouch = true;
				}
				else if (normalizedX > 0.8f && normalizedY < 0.2f)
				{
					hasBottomRightTouch = true;
				}
			}

			return hasTopLeftTouch && hasBottomRightTouch;
		}
	}
}
#endif
