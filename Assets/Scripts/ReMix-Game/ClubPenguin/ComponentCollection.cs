using System;
using System.Collections.Generic;

namespace ClubPenguin
{
	public abstract class ComponentCollection
	{
		private readonly Dictionary<Type, object> componentMap = new Dictionary<Type, object>();

		public void AddComponent<T>(T component) where T : class
		{
			if (typeof(T).IsInterface)
			{
				throw new Exception("Cannot AddComponent by interfaced type: " + typeof(T).FullName + ". Please use concrete implementation");
			}
			if (typeof(T).IsAbstract)
			{
				throw new Exception("Cannot AddComponent by abstract inplementation: " + typeof(T).FullName + ". Please use concrete implementation");
			}
			if (HasComponent<T>())
			{
				throw new Exception("An instance of this component class has already been set!");
			}
			componentMap.Add(typeof(T), component);
		}

		public T GetComponent<T>(bool strictMode = false) where T : class
		{
			object component = null;
			if (!TryGetComponent<T>(out component, strictMode))
			{
			}
			return (T)component;
		}

		public List<T> GetComponents<T>() where T : class
		{
			List<T> list = new List<T>();
			foreach (KeyValuePair<Type, object> item in componentMap)
			{
				if (item.Value is T)
				{
					list.Add((T)item.Value);
				}
			}
			return list;
		}

		public bool RemoveComponent<T>() where T : class
		{
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsInterface)
			{
				throw new Exception("Cannot RemoveComponent by interfaced type: " + typeof(T).FullName + ". Please use concrete implementation");
			}
			if (typeFromHandle.IsAbstract)
			{
				throw new Exception("Cannot RemoveComponent by abstract inplementation: " + typeof(T).FullName + ". Please use concrete implementation");
			}
			return componentMap.Remove(typeFromHandle);
		}

		public bool HasComponent<T>()
		{
			object component = null;
			return TryGetComponent<T>(out component);
		}

		public List<Type> AddedComponentTypes()
		{
			return new List<Type>(componentMap.Keys);
		}

		public bool TryGetComponent<T>(out object component, bool strictMode = false)
		{
			if (strictMode && (typeof(T).IsInterface || typeof(T).IsAbstract))
			{
				throw new Exception("Cannot GetComponent by interfaced or abstract type: " + typeof(T).FullName + " in strict mode.");
			}
			if (componentMap.TryGetValue(typeof(T), out component))
			{
				return true;
			}
			if (!strictMode)
			{
				foreach (KeyValuePair<Type, object> item in componentMap)
				{
					if (item.Value is T)
					{
						component = item.Value;
						return true;
					}
				}
			}
			return false;
		}

		public void ResetAll()
		{
			if (componentMap != null)
			{
				foreach (KeyValuePair<Type, object> item in componentMap)
				{
					componentMap[item.Key] = null;
				}
				componentMap.Clear();
			}
		}
	}
}
