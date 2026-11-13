using System;
using System.Collections.Generic;
using UnityEngine;

namespace DisneyMobile.CoreUnitySystems
{
	public abstract class CrossPlatformInitAction : InitAction
	{
		public class CrossPlatformType
		{
			private RuntimePlatform m_platform;

			private string m_tag;

			public RuntimePlatform Platform
			{
				get
				{
					return m_platform;
				}
			}

			public CrossPlatformType()
			{
			}

			public CrossPlatformType(RuntimePlatform p, string tag)
			{
				m_platform = p;
				m_tag = tag;
			}

			public override bool Equals(object obj)
			{
				if (obj == null || GetType() != obj.GetType())
				{
					return false;
				}
				CrossPlatformType crossPlatformType = obj as CrossPlatformType;
				return m_platform == crossPlatformType.m_platform && m_tag == crossPlatformType.m_tag;
			}

			public override int GetHashCode()
			{
				return m_platform.GetHashCode();
			}

			public override string ToString()
			{
				return string.Format("[CrossPlatformType: Platform={0} Tag = {1}]", Platform, m_tag);
			}
		}

		private Dictionary<CrossPlatformType, Type> m_platformTypeMap = new Dictionary<CrossPlatformType, Type>();

		private object m_instance;

		private Type m_defaultType;

		private bool m_createGameObject = false;

		private bool m_ignorePlatformTags = false;

		public object Instance
		{
			get
			{
				return m_instance;
			}
		}

		public static Transform GetRootObject()
		{
			if (BaseGameController.Instance != null)
			{
				return BaseGameController.Instance.transform;
			}
			return null;
		}

		public CrossPlatformInitAction(Type defaultType, bool createGameObject = false, bool ignorePlatformTags = false)
		{
			m_createGameObject = createGameObject;
			m_ignorePlatformTags = ignorePlatformTags;
			SetDefaultType(defaultType);
		}

		public void CreateInstance()
		{
			Type typeForCurrentPlatform = GetTypeForCurrentPlatform();
			if (m_createGameObject)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = typeForCurrentPlatform.ToString();
				string[] array = typeForCurrentPlatform.ToString().Split('.');
				if (array.Length > 0)
				{
					m_instance = gameObject.AddComponent(Type.GetType(array[array.Length - 1]));
				}
				Transform rootObject = GetRootObject();
				if (rootObject != null)
				{
					gameObject.transform.parent = rootObject;
				}
			}
			else
			{
				m_instance = Activator.CreateInstance(typeForCurrentPlatform);
			}
			IConfigurable configurable = m_instance as IConfigurable;
			if (configurable != null)
			{
				IDictionary<string, object> dictionaryForSystem = base.Configurator.GetDictionaryForSystem(GetTypeForCurrentPlatform());
				configurable.Configure(dictionaryForSystem);
			}
		}

		public void AddType(RuntimePlatform platform, Type platformSpecificType)
		{
			AddType(platform, "", platformSpecificType);
		}

		public void AddType(RuntimePlatform platform, string tag, Type platformSpecificType)
		{
			CrossPlatformType crossPlatformType = new CrossPlatformType(platform, tag);
			m_platformTypeMap.Add(crossPlatformType, platformSpecificType);
			Logger.LogInfo(this, "Platform added = " + crossPlatformType.ToString() + " type = " + platformSpecificType.ToString());
		}

		public void SetDefaultType(Type platformSpecificType)
		{
			m_defaultType = platformSpecificType;
		}

		public Type GetTypeForCurrentPlatform()
		{
			string tag = "";
			if (!m_ignorePlatformTags)
			{
				tag = EnvironmentManager.GetPlatformTag();
			}
			CrossPlatformType platform = new CrossPlatformType(Application.platform, tag);
			return GetType(platform);
		}

		public Type GetType(RuntimePlatform platform)
		{
			if (platform != Application.platform)
			{
				Logger.LogFatal(this, "Calling CrossPlatformInitAction::GetType() with platform != Application.platform may return wrong type.");
			}
			else
			{
				Logger.LogWarning(this, "Type CrossPlatformInitAction::GetType(RuntimePlatform platform) is obsolete now, please use GetTypeForCurrentPlatform()");
			}
			string tag = "";
			if (!m_ignorePlatformTags)
			{
				tag = EnvironmentManager.GetPlatformTag();
			}
			CrossPlatformType platform2 = new CrossPlatformType(platform, tag);
			return GetType(platform2);
		}

		public Type GetType(CrossPlatformType platform)
		{
			if (m_platformTypeMap.ContainsKey(platform))
			{
				return m_platformTypeMap[platform];
			}
			CrossPlatformType key = new CrossPlatformType(platform.Platform, "");
			if (m_platformTypeMap.ContainsKey(key))
			{
				return m_platformTypeMap[key];
			}
			return m_defaultType;
		}
	}
}
