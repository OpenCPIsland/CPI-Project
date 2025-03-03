using ClubPenguin.Core;
using ClubPenguin.Core.StaticGameData;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ClubPenguin
{
	public class GameData : IGameData
	{
		private bool loadedDefinitions = false;

		private readonly Dictionary<Type, object> dataMap;

		private static MethodInfo _loadAsyncManifestMethod;

		private static MethodInfo _loadImmediateManifestMethod;

		public bool Initialized
		{
			get;
			private set;
		}

		private static MethodInfo loadAsyncManifestMethod
		{
			get
			{
				if (_loadAsyncManifestMethod == null)
				{
					MethodInfo[] methods = typeof(GameData).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
					for (int i = 0; i < methods.Length; i++)
					{
						if (methods[i].Name == "loadAsyncManifest" && methods[i].GetParameters()[0].ParameterType == typeof(ManifestContentKey))
						{
							_loadAsyncManifestMethod = methods[i];
							break;
						}
					}
				}
				return _loadAsyncManifestMethod;
			}
		}

		private static MethodInfo loadImmediateManifestMethod
		{
			get
			{
				if (_loadImmediateManifestMethod == null)
				{
					MethodInfo[] methods = typeof(GameData).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
					for (int i = 0; i < methods.Length; i++)
					{
						if (methods[i].Name == "loadImmediateManifest" && methods[i].GetParameters()[0].ParameterType == typeof(ManifestContentKey))
						{
							_loadImmediateManifestMethod = methods[i];
							break;
						}
					}
				}
				return _loadImmediateManifestMethod;
			}
		}

		public event Action<GameData> EInitialized;

		public GameData()
		{
			dataMap = new Dictionary<Type, object>();
		}

		public IEnumerator LoadDataForDefinitions(Type[] definitionTypesToLoad, System.Action onComplete = null)
		{
			if (!loadedDefinitions)
			{
				ICoroutine coroutine = CoroutineRunner.Start(loadData(definitionTypesToLoad), this, "LoadData");
				while (!coroutine.Completed && !coroutine.Cancelled)
				{
					yield return null;
				}
				loadedDefinitions = true;
				if (onComplete != null)
				{
					onComplete();
				}
			}
			else if (onComplete != null)
			{
				onComplete();
			}
		}

		public void Init(Type[] types)
		{
			CoroutineRunner.Start(loadData(types), this, "LoadData");
		}

		public T Get<T>()
		{
			return (T)Get(typeof(T));
		}

		public object Get(Type type)
		{
			if (!Initialized)
			{
				throw new Exception("Wait until Initialized is true before gettting data.");
			}
			if (!dataMap.ContainsKey(type))
			{
				throw new Exception("Error in get sequence. Definition not loaded before being requested: " + type);
			}
			return dataMap[type];
		}

		public bool IsAvailable<T>()
		{
			return IsAvailable(typeof(T));
		}

		public bool IsAvailable(Type type)
		{
			return dataMap.ContainsKey(type);
		}

		public TDefinition GetDefinitionById<TDefinition, TId>(TypedStaticGameDataKey<TDefinition, TId> staticGameDataKey) where TDefinition : StaticGameDataDefinition
		{
			Dictionary<TId, TDefinition> dictionary = Get<Dictionary<TId, TDefinition>>();
			TDefinition value;
			if (dictionary.TryGetValue(staticGameDataKey.Id, out value))
			{
				return value;
			}
			throw new KeyNotFoundException(string.Format("{0} with Id {1} not found", typeof(TDefinition).Name, staticGameDataKey.Id));
		}

		private IEnumerator loadData(Type[] types)
		{
			List<CoroutineReturn> requests = new List<CoroutineReturn>();
			for (int i = 0; i < types.Length; i++)
			{
				requests.Add(loadAsyncManifest(types[i]));
			}
			while (requests.Any((CoroutineReturn c) => !c.Finished && !c.Cancelled))
			{
				yield return null;
			}
			Initialized = true;
			if (this.EInitialized != null)
			{
				this.EInitialized(this);
			}
		}

		private CoroutineReturn loadAsyncManifest(Type type)
		{
			return (CoroutineReturn)loadAsyncManifestMethod.MakeGenericMethod(type).Invoke(this, new object[1]
			{
				StaticGameDataUtils.GetManifestContentKey(type)
			});
		}

		private CoroutineReturn loadAsyncManifest<T>(ManifestContentKey manifestContentKey) where T : ScriptableObject
		{
			return Content.LoadAsync(onManifestLoaded<T>, manifestContentKey);
		}

		private void loadImmediateManifest(Type type)
		{
			loadImmediateManifestMethod.MakeGenericMethod(type).Invoke(this, new object[1]
			{
				StaticGameDataUtils.GetManifestContentKey(type)
			});
		}

		private void loadImmediateManifest<T>(ManifestContentKey manifestContentKey) where T : ScriptableObject
		{
			onManifestLoaded<T>(manifestContentKey.Key, Content.LoadImmediate(manifestContentKey));
		}

		private void onManifestLoaded<T>(string contentKey, Manifest manifest) where T : ScriptableObject
		{
			Content.TryPinBundle(contentKey);
			if (manifest == null || manifest.Assets == null)
			{
				Log.LogErrorFormatted(this, "Null Manifest {0} for type {1}", contentKey, typeof(T));
				return;
			}
			FieldInfo attributedField = StaticGameDataDefinitionIdAttribute.GetAttributedField(typeof(T));
			if (attributedField != null)
			{
				Type type = typeof(Dictionary<, >).MakeGenericType(attributedField.FieldType, typeof(T));
				IDictionary dictionary = (IDictionary)Activator.CreateInstance(type);
				for (int i = 0; i < manifest.Assets.Length; i++)
				{
					if (!(manifest.Assets[i] != null))
					{
						continue;
					}
					T val = manifest.Assets[i] as T;
					if ((UnityEngine.Object)val != (UnityEngine.Object)null)
					{
						object value = attributedField.GetValue(val);
						if (value == null)
						{
							Log.LogErrorFormatted(manifest.Assets[i], "No {0} value found for asset {1} from manifest {2} at element {3}", attributedField, val, contentKey, i);
						}
						else if (dictionary.Contains(value))
						{
							Log.LogErrorFormatted(manifest.Assets[i], "Duplicate definition ID '{0}' for manifest type '{1}' was found.", value, typeof(T).FullName);
						}
						else
						{
							dictionary.Add(value, val);
						}
					}
					else
					{
						Log.LogErrorFormatted(manifest.Assets[i], "Could not convert this manifest entry to type {0}. Manifest {1} contains errors. Actual type was {2}", typeof(T), contentKey, manifest.Assets[i].GetType());
					}
				}
				dataMap[type] = dictionary;
			}
			else
			{
				T[] value2 = Array.ConvertAll(manifest.Assets, (ScriptableObject asset) => (T)asset);
				dataMap[typeof(T[])] = value2;
			}
		}
	}
}
