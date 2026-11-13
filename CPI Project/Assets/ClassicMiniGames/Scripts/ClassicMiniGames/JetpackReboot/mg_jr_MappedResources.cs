using NUnit.Framework;
using System.Collections.Generic;

namespace JetpackReboot
{
	public abstract class mg_jr_MappedResources<T, U> where T : class
	{
		private Dictionary<U, T> m_resources = new Dictionary<U, T>();

		public abstract void LoadResources();

		protected void AddResourceMapping(U _key, T _resource)
		{
			Assert.IsFalse(m_resources.ContainsKey(_key), "There is already a mapping for that resource key");
			m_resources.Add(_key, _resource);
		}

		public T GetResource(U _resourceKey)
		{
			if (m_resources.ContainsKey(_resourceKey))
			{
				return m_resources[_resourceKey];
			}
			return null;
		}
	}
}
