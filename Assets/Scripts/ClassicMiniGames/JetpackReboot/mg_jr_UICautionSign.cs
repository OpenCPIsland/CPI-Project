using CameraExtensionMethods;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_UICautionSign : MonoBehaviour
	{
		public enum SignState
		{
			SHOWING_YELLOW,
			SHOWING_RED,
			INACTIVE,
			MAX
		}

		private const float GAP_TO_SCREEN_EDGE = 0.1f;

		private Animator m_signAnimator;

		private SpriteRenderer SignRenderer
		{
			get;
			set;
		}

		public Vector3 SignSize
		{
			get;
			private set;
		}

		private void Awake()
		{
			m_signAnimator = GetComponent<Animator>();
			Assert.NotNull(m_signAnimator, "Sign animator not found");
			SignRenderer = GetComponentInChildren<SpriteRenderer>();
			Assert.NotNull(SignRenderer, "Sign renderer not found");
			SignSize = SignRenderer.bounds.size;
			ChangeState(SignState.INACTIVE);
		}

		public void SetPosition(float _verticalPosition, bool _isRightSide = true)
		{
			Vector3 zero = Vector3.zero;
			float num = SignSize.y * 0.5f;
			zero.y = _verticalPosition + num;
			if (_isRightSide)
			{
				zero.x = Camera.main.RightEdgeInWorld() - SignSize.x - 0.1f;
				SignRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			else
			{
				zero.x = Camera.main.LeftEdgeInWorld() + SignSize.x + 0.1f;
				SignRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			base.transform.position = zero;
		}

		public void ChangeState(SignState _state)
		{
			if (!(m_signAnimator == null))
			{
				switch (_state)
				{
				case SignState.SHOWING_YELLOW:
					m_signAnimator.SetBool("On", true);
					break;
				case SignState.SHOWING_RED:
					m_signAnimator.SetBool("Red", true);
					break;
				case SignState.INACTIVE:
					m_signAnimator.SetBool("On", false);
					m_signAnimator.SetBool("Red", false);
					break;
				default:
					Assert.IsTrue(false, "Not a valid sign state to switch to");
					break;
				}
			}
		}
	}
}
