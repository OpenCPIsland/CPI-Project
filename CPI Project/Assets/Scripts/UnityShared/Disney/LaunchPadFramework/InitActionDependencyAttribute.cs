using System;

namespace Disney.LaunchPadFramework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class InitActionDependencyAttribute : Attribute
	{
		private Type m_dependentType;

		public Type Type
		{
			get
			{
				return m_dependentType;
			}
			set
			{
				m_dependentType = value;
			}
		}

		public InitActionDependencyAttribute(Type dependencyType)
		{
			m_dependentType = dependencyType;
		}
	}
}
