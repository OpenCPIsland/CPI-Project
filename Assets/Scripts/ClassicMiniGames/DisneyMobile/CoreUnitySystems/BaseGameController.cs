using DisneyMobile.CoreUnitySystems.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	[RequireComponent(typeof(ScriptableObject))]
	public class BaseGameController : MonoBehaviour
	{
		private static BaseGameController m_gameController;

		private EventDispatcher m_eventDispatcher = null;

		private Configurator m_configurator = null;

		private Logger Logger = null;

		public static BaseGameController Instance
		{
			get
			{
				return m_gameController;
			}
		}

		public EventDispatcher EventDispatcher
		{
			get
			{
				if (Instance == null)
				{
					throw new ApplicationException("Accessing eventDispatcher but GameController was not initialized");
				}
				return Instance.m_eventDispatcher;
			}
		}

		public Configurator Configurator
		{
			get
			{
				if (Instance == null)
				{
					throw new ApplicationException("Accessing configurator but GameController was not initialized");
				}
				return Instance.m_configurator;
			}
		}

		public static void DestroyInstance()
		{
			if (m_gameController != null)
			{
				UnityEngine.Object.Destroy(m_gameController.gameObject);
				m_gameController = null;
			}
		}

		protected void Awake()
		{
			if (m_gameController != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(this);
			m_gameController = this;
			InitBootStrappingSystems();
		}

		protected virtual void InitBootStrappingSystems()
		{
			m_eventDispatcher = new EventDispatcher();
			m_configurator = new Configurator();
			m_configurator.Init(false);
			if (m_configurator.IsSystemEnabled(typeof(Logger)))
			{
				IDictionary<string, object> dictionaryForSystem = m_configurator.GetDictionaryForSystem(typeof(Logger));
				Logger = new Logger();
				Logger.Configure(dictionaryForSystem);
			}
			InitAction initAction = new InitActionEnvironmentManager();
			initAction.Configurator = m_configurator;
			initAction.EventDispatcher = m_eventDispatcher;
			CoroutineHelper.RunCoroutineToCompletion(initAction.Perform());
		}

		protected void OnDestroy()
		{
			Cleanup();
		}

		protected void OnApplicationQuit()
		{
			Cleanup();
		}

		protected virtual void Cleanup()
		{
			if (m_eventDispatcher != null)
			{
				m_eventDispatcher.OnApplicationQuit();
			}
		}
	}
}
