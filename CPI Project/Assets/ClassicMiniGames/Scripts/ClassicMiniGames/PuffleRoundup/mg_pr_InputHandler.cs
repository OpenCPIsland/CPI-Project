using MinigameFramework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PuffleRoundup
{
	public class mg_pr_InputHandler : MonoBehaviour
	{
		public GameObject snowPuff;

		public Component[] m_puffles;

		public GameObject m_PuffleContainer;

		public mg_PuffleRoundup Minigame;

		private bool m_holdCancelled;

		private void Awake()
		{
			Minigame = MinigameManager.GetActive<mg_PuffleRoundup>();
		}

		private void Start()
		{
			m_PuffleContainer = Minigame.transform.Find("mg_pr_GameContainer/mg_pr_PuffleContainer").gameObject;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			// focus leaving mid hold takes the release with it, and the cursor can still be
			// inside the window where the check below never fires. a constant isFocused test
			// in FixedUpdate would kill input for good if it ever stuck at false
			if (!hasFocus)
			{
				m_holdCancelled = true;
			}
		}

		private void FixedUpdate()
		{
			if (!MinigameManager.IsPaused)
			{
				Vector3 pointerPosition = Vector3.zero;
				bool mouseDown = false;
				bool mouseHeld = false;

				// Use the new Input System for pointer position and button state
				if (Mouse.current != null)
				{
					var mouse = Mouse.current;
					pointerPosition = mouse.position.ReadValue();
					mouseDown = mouse.leftButton.wasPressedThisFrame;
					mouseHeld = mouse.leftButton.isPressed;

					// a button released outside the window never reaches the game, so the hold
					// sticks and the puffles run from the cursor until they cross the boundary
					// and count as escaped. no hold counts while the pointer is outside, and it
					// counts again once the button is seen released inside
					bool pointerInsideWindow = pointerPosition.x >= 0f && pointerPosition.y >= 0f
						&& pointerPosition.x <= Screen.width && pointerPosition.y <= Screen.height;
					if (!pointerInsideWindow)
					{
						m_holdCancelled = true;
					}
					else if (!mouseHeld)
					{
						m_holdCancelled = false;
					}
					if (m_holdCancelled)
					{
						mouseDown = false;
						mouseHeld = false;
					}
				}
#if UNITY_ANDROID || UNITY_IOS
				// Optional: add touch support
				if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
				{
					pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
					mouseDown = Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
					mouseHeld = Touchscreen.current.primaryTouch.press.isPressed;
				}
#endif

				Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
				Vector3 worldPosition = Camera.main.ScreenToWorldPoint(pointerPosition);
				worldPosition.z = 0f;
				RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
				if (mouseDown && hit && hit.transform.gameObject.GetComponent<Collider2D>().name == "mg_pr_PlayArea")
				{
					Object.Instantiate(snowPuff, worldPosition, Quaternion.identity);
				}
				if (mouseHeld)
				{
					PuffleHandler(worldPosition);
				}
			}
		}

		private void PuffleHandler(Vector3 myPosition)
		{
			m_puffles = m_PuffleContainer.GetComponentsInChildren<mg_pr_PuffleController>();
			Component[] puffles = m_puffles;
			for (int i = 0; i < puffles.Length; i++)
			{
				mg_pr_PuffleController mg_pr_PuffleController = (mg_pr_PuffleController)puffles[i];
				float num = Vector3.Distance(myPosition, mg_pr_PuffleController.transform.position);
				float range = mg_pr_PuffleController.gameObject.GetComponent<mg_pr_PuffleController>().m_range;
				bool escaped = mg_pr_PuffleController.gameObject.GetComponent<mg_pr_PuffleController>().m_escaped;
				if (num <= range && !escaped)
				{
					float speed = mg_pr_PuffleController.GetComponent<mg_pr_PuffleController>().m_speed;
					Vector3 b = (-(myPosition - mg_pr_PuffleController.transform.position)).normalized * speed * Time.deltaTime;
					Vector3 v = mg_pr_PuffleController.transform.position + b;
					mg_pr_PuffleController.GetComponent<Rigidbody2D>().MovePosition(v);
				}
			}
		}
	}
}