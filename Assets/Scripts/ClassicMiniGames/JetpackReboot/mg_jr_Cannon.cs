using MinigameFramework;
using NUnit.Framework;
using UnityEngine;

namespace JetpackReboot
{
	public class mg_jr_Cannon : mg_jr_Obstacle
	{
		private const float SHOOT_INTERVAL = 3f;

		private mg_JetpackReboot m_miniGame;

		private Transform m_snowballSpawnSocket;

		private SpriteRenderer m_cannonRenderer;

		private bool m_firing = false;

		private float m_timeSinceLastShot = float.MaxValue;

		private Animator m_animator;

		private float m_startFiringDistance = 0f;

		public mg_jr_Sound ShootSound
		{
			get;
			set;
		}

		public float StartFiringDistance
		{
			get
			{
				return m_startFiringDistance;
			}
			set
			{
				m_startFiringDistance = Mathf.Clamp01(value);
			}
		}

		private void Awake()
		{
			m_miniGame = MinigameManager.GetActive<mg_JetpackReboot>();
			Assert.NotNull(m_miniGame, "mini game not found");
			m_snowballSpawnSocket = base.transform.Find("mg_jr_SnowballSocket");
			Assert.NotNull(m_snowballSpawnSocket, "Snowball socket not found");
			m_cannonRenderer = GetComponentsInChildren<SpriteRenderer>(true)[0];
			Assert.NotNull(m_cannonRenderer, "Cannon renderer not found");
			m_animator = GetComponent<Animator>();
			Assert.NotNull(m_animator, "Cannon Animator not found");
			ShootSound = mg_jr_Sound.FIRE_RED_CANNON;
		}

		private void Update()
		{
			if (m_miniGame.IsPaused)
			{
				return;
			}
			float num = m_miniGame.VisibleWorldBounds.size.x * StartFiringDistance;
			float num2 = m_miniGame.VisibleWorldBounds.max.x - num;
			if (!m_firing && base.transform.position.x + m_cannonRenderer.bounds.size.x < num2 && m_miniGame.VisibleWorldBounds.Intersects(m_cannonRenderer.bounds))
			{
				m_firing = true;
			}
			if (m_firing)
			{
				m_timeSinceLastShot += Time.deltaTime;
				if (m_timeSinceLastShot > 3f && m_miniGame.GameLogic.IsGameInProgress)
				{
					Shoot();
					m_timeSinceLastShot = 0f;
				}
			}
		}

		private void OnDisable()
		{
			m_firing = false;
			m_timeSinceLastShot = float.MaxValue;
		}

		private void Shoot()
		{
			m_animator.SetTrigger("Shoot");
			GameObject pooledResource = m_miniGame.Resources.GetPooledResource(mg_jr_ResourceList.GAME_PREFAB_SNOWBALL);
			pooledResource.transform.position = m_snowballSpawnSocket.transform.position;
			pooledResource.transform.localRotation = m_snowballSpawnSocket.transform.localRotation;
			pooledResource.transform.parent = base.transform.parent;
			MinigameManager.GetActive().PlaySFX(ShootSound.ClipName());
		}
	}
}
