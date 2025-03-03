using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_RobotPenguin : mg_jr_Collectable
	{
		public enum RobotPenguinState
		{
			COLLECTABLE,
			FOLLOWING,
			EXPLODING,
			MAX
		}

		private RobotPenguinState m_currentState = RobotPenguinState.MAX;

		private Collider2D m_collider;

		private Animator m_animator;

		private SpriteRenderer m_renderer;

		private mg_jr_Collector m_collector;

		private mg_jr_Blinker m_blinker;

		private mg_jr_Follow m_follower;

		private mg_jr_ObstacleDestroyer m_obstacleDestroyer;

		private bool m_hasBeenVisible = false;

		private bool m_isStarted = false;

		public mg_jr_Collector Collector
		{
			get
			{
				return m_collector;
			}
		}

		public mg_jr_Blinker Blinker
		{
			get
			{
				return m_blinker;
			}
		}

		public mg_jr_ObstacleDestroyer ObstacleDestroyer
		{
			get
			{
				return m_obstacleDestroyer;
			}
		}

		public void Start()
		{
			if (!m_isStarted)
			{
				m_collider = GetComponent<Collider2D>();
				m_collector = GetComponent<mg_jr_Collector>();
				m_follower = GetComponent<mg_jr_Follow>();
				m_animator = GetComponentInChildren<Animator>();
				m_renderer = GetComponentInChildren<SpriteRenderer>();
				m_blinker = GetComponent<mg_jr_Blinker>();
				m_obstacleDestroyer = GetComponent<mg_jr_ObstacleDestroyer>();
				Assert.NotNull(m_collider, "Robotpenguins need a collider");
				Assert.NotNull(m_collector, "Robotpenguins need a collector");
				Assert.NotNull(m_follower, "Robotpenguins need a follower");
				Assert.NotNull(m_animator, "Robotpenguins need an animator");
				Assert.NotNull(m_blinker, "Robotpenguins need an blinker");
				Assert.NotNull(m_obstacleDestroyer, "Robotpenguins need an ObstacleDestroyer");
				SetState(RobotPenguinState.COLLECTABLE);
				m_isStarted = true;
			}
		}

		private void OnDisable()
		{
			if (MinigameManager.GetActive() != null)
			{
				MinigameManager.GetActive().StopSFX(mg_jr_Sound.ROBOT_HELI_LOOP.ClipName());
			}
		}

		private void Update()
		{
			if (MinigameManager.IsPaused || m_currentState != 0)
			{
				return;
			}
			Bounds visibleWorldBounds = MinigameManager.GetActive<mg_JetpackReboot>().VisibleWorldBounds;
			if (!m_hasBeenVisible)
			{
				if (visibleWorldBounds.Contains(base.transform.position))
				{
					m_hasBeenVisible = true;
					MinigameManager.GetActive().PlaySFX(mg_jr_Sound.ROBOT_HELI_LOOP.ClipName());
				}
			}
			else if (m_renderer.bounds.max.x < visibleWorldBounds.min.x)
			{
				MinigameManager.GetActive().StopSFX(mg_jr_Sound.ROBOT_HELI_LOOP.ClipName());
				m_hasBeenVisible = false;
			}
		}

		public override void OnCollection()
		{
			MinigameManager.GetActive().StopSFX(mg_jr_Sound.ROBOT_HELI_LOOP.ClipName());
			MinigameManager.GetActive().PlaySFX(mg_jr_Sound.ROBOT_HELI_PICKUP.ClipName());
		}

		private void SetState(RobotPenguinState _newState)
		{
			switch (_newState)
			{
			case RobotPenguinState.COLLECTABLE:
				base.enabled = true;
				m_collector.enabled = false;
				m_obstacleDestroyer.enabled = false;
				m_collider.isTrigger = true;
				if (GetComponent<Rigidbody2D>() != null)
				{
					Object.Destroy(GetComponent<Rigidbody2D>());
				}
				m_hasBeenVisible = false;
				m_follower.ClearFollowTarget();
				Blinker.StopBlinking();
				m_animator.SetTrigger("GoToInitial");
				break;
			case RobotPenguinState.FOLLOWING:
				base.enabled = false;
				m_collector.enabled = true;
				m_collider.isTrigger = false;
				if (GetComponent<Rigidbody2D>() == null)
				{
					Rigidbody2D rigidbody2D = base.gameObject.AddComponent<Rigidbody2D>();
					rigidbody2D.isKinematic = true;
				}
				m_animator.SetBool("Follower", true);
				break;
			case RobotPenguinState.EXPLODING:
				base.enabled = false;
				Blinker.StopBlinking();
				m_obstacleDestroyer.enabled = false;
				m_collector.enabled = false;
				m_follower.ClearFollowTarget();
				if (GetComponent<Rigidbody2D>() != null)
				{
					Object.Destroy(GetComponent<Rigidbody2D>());
				}
				m_animator.SetTrigger("Kill");
				MinigameManager.GetActive().PlaySFX(mg_jr_Sound.ROBOT_HELI_EXPLODE.ClipName());
				break;
			default:
				Assert.IsTrue(false, "unhandled robot penguin state");
				break;
			}
			m_currentState = _newState;
		}

		public void MakeFollower(GameObject _target, Vector3 _offset, bool _lockPositionWhenClose = false)
		{
			Transform parent = base.transform.parent;
			while (parent != null)
			{
				Assert.IsFalse(parent == _target.transform, "Robot can't be a descendant of its follow target");
				parent = parent.parent;
			}
			m_follower.FollowTarget(_target, _offset, _lockPositionWhenClose);
			SetState(RobotPenguinState.FOLLOWING);
		}

		public void Explode()
		{
			SetState(RobotPenguinState.EXPLODING);
		}

		public void ToggleTurboAnimation(bool _enable)
		{
			Assert.NotNull(m_animator, "Animator is null, did you try and call this method before start?");
			m_animator.SetBool("Turbo", _enable);
		}

		public void Recycle()
		{
			SetState(RobotPenguinState.COLLECTABLE);
			GetComponent<mg_jr_Pooled>().ReturnToPool();
		}
	}
}
