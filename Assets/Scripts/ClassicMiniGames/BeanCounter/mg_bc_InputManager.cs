using ClubPenguin.Classic.MiniGames;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace BeanCounter
{
	[RequireComponent(typeof(MouseInputObserver))]
	public class mg_bc_InputManager : MonoBehaviour
	{
		private DelegateInputManagerSubscribeEvent m_onDrag;

		private DelegateInputManagerSubscribeEvent m_onTouchUp;

		private DelegateInputManagerSubscribeEvent m_onTouchDown;

		private MouseInputObserver mouseInputObserver;

		public bool IsActive
		{
			get;
			set;
		}

		private void Awake()
		{
			IsActive = true;
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			active.InputManager = this;
			mouseInputObserver = GetComponent<MouseInputObserver>();
		}

		public void Prepare(Camera _camera)
		{
			InputManager.AddCamera(_camera);
			m_onDrag = OnDrag;
			m_onTouchUp = OnTouchUp;
			m_onTouchDown = OnTouchDown;
			mouseInputObserver.MouseMovedEvent += OnMouseMoved;
			mouseInputObserver.PrimaryMouseButtonDownEvent += OnPrimaryMouseDown;
		}

		private void OnPrimaryMouseDown()
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			if (IsActive && active != null && active.GameLogic != null)
			{
				active.GameLogic.OnMouseDown();
			}
		}

		public void TidyUp(Camera _camera)
		{
			InputManager.RemoveCamera(_camera);
			mouseInputObserver.MouseMovedEvent -= OnMouseMoved;
			mouseInputObserver.PrimaryMouseButtonDownEvent -= OnPrimaryMouseDown;
		}

		private void OnDrag(Gesture gesture)
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			if (IsActive && active != null && active.GameLogic != null)
			{
				active.GameLogic.OnTouchDrag(gesture);
			}
		}

		private void OnMouseMoved(Vector3 mousePosition, Vector2 deltaPosition)
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			if (IsActive && active != null && active.GameLogic != null)
			{
				active.GameLogic.OnMouseMove(mousePosition);
			}
		}

		private void OnTouchDown(Gesture gesture)
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			if (IsActive && active != null && active.GameLogic != null)
			{
				active.GameLogic.OnTouchPress(true, gesture);
			}
		}

		private void OnTouchUp(Gesture gesture)
		{
			mg_BeanCounter active = MinigameManager.GetActive<mg_BeanCounter>();
			if (IsActive && active != null && active.GameLogic != null)
			{
				active.GameLogic.OnTouchPress(false, gesture);
			}
		}
	}
}
