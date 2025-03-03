using DisneyMobile.CoreUnitySystems.Utility;
using DisneyMobile.CoreUnitySystems.Utility.Algorithms;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DisneyMobile.CoreUnitySystems
{
	public abstract class InitAction : Action, ITopologicalNode
	{
		private List<Type> m_dependencies = new List<Type>();

		private Configurator m_configurator = null;

		private InitManager m_initManager = null;

		public Configurator Configurator
		{
			get
			{
				return m_configurator;
			}
			set
			{
				m_configurator = value;
			}
		}

		public InitManager InitManager
		{
			get
			{
				return m_initManager;
			}
			set
			{
				m_initManager = value;
			}
		}

		public string TopologicalIdentifier
		{
			get
			{
				return GetType().ToString();
			}
		}

		public List<string> TopologicalDependencies
		{
			get
			{
				return m_dependencies.ConvertAll((Type obj) => obj.ToString());
			}
		}

		public InitAction()
		{
			MemberInfo type = GetType();
			object[] customAttributes = type.GetCustomAttributes(true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				InitActionDependencyAttribute initActionDependencyAttribute = customAttributes[i] as InitActionDependencyAttribute;
				AddDependency(initActionDependencyAttribute.Type);
			}
		}

		public void Initialize(InitManager initManager, EventDispatcher eventDispatcher, Configurator configurator)
		{
			InitManager = initManager;
			base.EventDispatcher = eventDispatcher;
			Configurator = configurator;
		}

		public override bool CanStart()
		{
			return base.State == ActionState.WAITING;
		}

		public void AddDependency(Type actionType)
		{
			if (base.State == ActionState.WAITING)
			{
				m_dependencies.AddIfUnique(actionType);
			}
		}
	}
}
