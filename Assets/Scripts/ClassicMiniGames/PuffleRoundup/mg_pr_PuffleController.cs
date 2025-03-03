using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

namespace PuffleRoundup
{
	public class mg_pr_PuffleController : MonoBehaviour
	{
		public float m_range;

		public float m_speed;

		public bool m_escaped;

		private bool m_home;

		private Vector3 m_lastPos;

		private Animator m_anim;

		private mg_pr_GameLogic m_gameLogic;

		private bool m_gameStart;

		public mg_PuffleRoundup Minigame;

		private void Awake()
		{
			Minigame = MinigameManager.GetActive<mg_PuffleRoundup>();
			m_anim = GetComponent<Animator>();
		}

		private void Start()
		{
			m_gameLogic = UIManager.Instance.ScreenRootObject.GetComponentInChildren<mg_pr_GameLogic>();
			m_gameStart = true;
		}

		private void Update()
		{
			Vector3 vector = Camera.main.WorldToViewportPoint(base.transform.position);
			if ((double)vector.x < 0.0 || 1.0 < (double)vector.x || (double)vector.y < 0.0 || 1.0 < (double)vector.y)
			{
				Destroy();
				UpdateEscaped(1);
				return;
			}
			if (!MinigameManager.IsPaused)
			{
				AnimationHandler();
			}
			if (m_gameStart)
			{
				m_anim.SetInteger("direction", 11);
				m_gameStart = false;
			}
		}

		private void AnimationHandler()
		{
			Vector3 vector = (base.transform.position - m_lastPos) / Time.deltaTime;
			m_lastPos = base.transform.position;
			if (m_escaped)
			{
				m_anim.SetInteger("direction", 9);
			}
			else if ((double)vector.x < 0.5 && (double)vector.x > -0.5 && (double)vector.y < 0.5 && (double)vector.y > -0.5)
			{
				if (m_home)
				{
					m_anim.SetInteger("direction", 10);
				}
				else
				{
					m_anim.SetInteger("direction", 0);
				}
			}
			else if (vector.x < 1f && vector.x > -1f && vector.y > 0f)
			{
				PlayAudio("move");
				m_anim.SetInteger("direction", 1);
			}
			else if (vector.x > 0f && vector.y > 0f)
			{
				PlayAudio("move");
				m_anim.SetInteger("direction", 2);
			}
			else if (vector.x > 0f && vector.y < 1f && vector.y > -1f)
			{
				PlayAudio("move");
				m_anim.SetInteger("direction", 3);
			}
			else if (vector.y < 0f && vector.x > 0f)
			{
				PlayAudio("move");
				m_anim.SetInteger("direction", 4);
			}
			else if (vector.x < 1f && vector.x > -1f && vector.y < 0f)
			{
				PlayAudio("move");
				m_anim.SetInteger("direction", 5);
			}
			else if (vector.x < 0f && vector.y < 0f)
			{
				PlayAudio("move");
				m_anim.SetInteger("direction", 6);
			}
			else if (vector.x < 0f && vector.y > -1f && vector.y < 1f)
			{
				PlayAudio("move");
				m_anim.SetInteger("direction", 7);
			}
			else if (vector.x < 0f && vector.y > 0f)
			{
				PlayAudio("move");
				m_anim.SetInteger("direction", 8);
			}
		}

		private void OnTriggerStay2D(Collider2D other)
		{
			if (other.gameObject.tag == "PenHitbox")
			{
				m_home = true;
			}
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.tag == "Bounds")
			{
				Destroy();
				UpdateEscaped(1);
			}
			if (other.gameObject.tag == "PenHitbox")
			{
				UpdateCaught(1);
			}
			if (other.gameObject.tag == "TopBounds")
			{
				m_escaped = true;
				UpdateEscaped(1);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.tag == "PenHitbox")
			{
				m_home = false;
				UpdateCaught(-1);
			}
		}

		private void UpdateCaught(int i)
		{
			PlayAudio("success");
			m_gameLogic.caught += i;
		}

		private void UpdateEscaped(int i)
		{
			PlayAudio("fail");
			m_gameLogic.escaped += i;
		}

		public void Destroy()
		{
			Object.Destroy(base.gameObject);
		}

		private void PlayAudio(string sound)
		{
			int num = Random.Range(1, 4);
			if (sound == "move")
			{
				switch (num)
				{
				case 1:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_move1");
					break;
				case 2:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_move2");
					break;
				case 3:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_move3");
					break;
				}
			}
			if (sound == "fail")
			{
				switch (num)
				{
				case 1:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_fail1");
					break;
				case 2:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_fail2");
					break;
				case 3:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_fail3");
					break;
				}
			}
			if (sound == "success")
			{
				switch (num)
				{
				case 1:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_success1");
					break;
				case 2:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_success2");
					break;
				case 3:
					MinigameManager.GetActive().PlaySFX("mg_pr_sfx_success3");
					break;
				}
			}
		}
	}
}
