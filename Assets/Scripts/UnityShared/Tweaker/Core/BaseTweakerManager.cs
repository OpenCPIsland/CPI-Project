using System.Collections.Generic;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class BaseTweakerManager<T> where T : ITweakerObject
	{
		private TweakerDictionary<T> objects;

		private IScanner scanner;

		private object lockObj = new object();

		protected TweakerOptions options;

		public BaseTweakerManager(IScanner scanner, TweakerOptions options)
		{
			objects = new TweakerDictionary<T>();
			this.options = options;
			this.scanner = scanner;
			if (this.scanner != null)
			{
				this.scanner.GetResultProvider<T>().ResultProvided += OnObjectFound;
			}
		}

		private void OnObjectFound(object sender, ScanResultArgs<T> e)
		{
			RegisterObject(e.result);
		}

		public void RegisterObject(T t)
		{
			lock (lockObj)
			{
				if (objects.ContainsKey(t.Name))
				{
					throw new NameAlreadyRegisteredException(t.Name);
				}
				if (t.StrongInstance != null)
				{
					T @object = GetObject(new SearchOptions(t.Name.Split('#')[0], null, SearchOptions.ScopeType.All, SearchOptions.BindingType.Instance, t.StrongInstance));
					if (@object != null)
					{
						throw new InstanceAlreadyRegisteredException(@object);
					}
				}
				objects.Add(t.Name, t);
			}
		}

		public void UnregisterObject(T t)
		{
			UnregisterObject(t.Name);
		}

		public void UnregisterObject(string name)
		{
			lock (lockObj)
			{
				if (!objects.ContainsKey(name))
				{
					throw new NotFoundException(name);
				}
				objects.Remove(name);
			}
		}

		public TweakerDictionary<T> GetObjects(SearchOptions options = null)
		{
			PruneDeadInstances();
			TweakerDictionary<T> tweakerDictionary = new TweakerDictionary<T>();
			lock (lockObj)
			{
				foreach (T value in objects.Values)
				{
					if (options == null || options.CheckMatch(value))
					{
						tweakerDictionary.Add(value.Name, value);
					}
				}
			}
			return tweakerDictionary;
		}

		public T GetObject(SearchOptions options)
		{
			lock (lockObj)
			{
				foreach (T value in objects.Values)
				{
					if (options != null && options.CheckMatch(value))
					{
						return ValidateObjectToReturn(value);
					}
				}
				return default(T);
			}
		}

		public T GetObject(string name)
		{
			lock (lockObj)
			{
				T value = default(T);
				objects.TryGetValue(name, out value);
				return ValidateObjectToReturn(value);
			}
		}

		public void PruneDeadInstances()
		{
			lock (lockObj)
			{
				List<string> list = new List<string>();
				foreach (string key in objects.Keys)
				{
					if (!objects[key].IsValid)
					{
						list.Add(key);
					}
				}
				foreach (string item in list)
				{
					objects.Remove(item);
				}
			}
		}

		private T ValidateObjectToReturn(T obj)
		{
			if (obj != null && !obj.IsValid)
			{
				UnregisterObject(obj);
				return default(T);
			}
			return obj;
		}
	}
}
