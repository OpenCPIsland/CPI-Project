using System;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class CacheableType<T> : ICachableType
	{
		protected T data;

		protected readonly T defaultValue;

		protected bool isPersisted = false;

		protected string key = "";

		public T Value
		{
			get
			{
				return GetValue();
			}
			set
			{
				SetValue(value);
			}
		}

		protected string GetPlatformKey(string keyName)
		{
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
			if (UnityEngine.Application.isEditor)
			{
				return "Editor_" + keyName;
			}
#endif
			return keyName;
		}

		public event Action<T> EChanged;

		public CacheableType(string playerPrefsKey, T defaultValue)
		{
			key = playerPrefsKey;
			this.defaultValue = defaultValue;
		}

		public CacheableType(string playerPrefsKey, T defaultValue, Action<T> changedDelegate)
			: this(playerPrefsKey, defaultValue)
		{
			EChanged += changedDelegate;
		}

		public static implicit operator T(CacheableType<T> input)
		{
			return input.GetValue();
		}

		public static bool operator ==(T lhs, CacheableType<T> rhs)
		{
			return lhs.Equals(rhs.GetValue());
		}

		public static bool operator !=(T lhs, CacheableType<T> rhs)
		{
			return !lhs.Equals(rhs);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			CacheableType<T> cacheableType = obj as CacheableType<T>;
			if ((object)cacheableType == null)
			{
				return false;
			}
			return string.Equals(cacheableType.key, key);
		}

		public bool Equals(T value)
		{
			if (value == null)
			{
				return false;
			}
			return value.Equals(GetValue());
		}

		public override int GetHashCode()
		{
			return (data != null) ? data.GetHashCode() : 0;
		}

		public virtual T GetValue()
		{
			if (!isPersisted)
			{
				string platformKey = GetPlatformKey(key);
				if (PlayerPrefs.HasKey(platformKey))
				{
					Type typeFromHandle = typeof(T);
					if (typeFromHandle == typeof(short) || typeFromHandle == typeof(int))
					{
						data = (T)Convert.ChangeType(PlayerPrefs.GetInt(platformKey), typeFromHandle);
					}
					else if (typeFromHandle == typeof(bool))
					{
						data = (T)Convert.ChangeType(PlayerPrefs.GetInt(platformKey), typeFromHandle);
					}
					else if (typeFromHandle == typeof(float))
					{
						data = (T)Convert.ChangeType(PlayerPrefs.GetFloat(platformKey), typeFromHandle);
					}
					else if (typeFromHandle.IsEnum)
					{
						data = (T)Enum.ToObject(typeof(T), PlayerPrefs.GetInt(platformKey));
					}
					else
					{
						data = (T)Convert.ChangeType(PlayerPrefs.GetString(platformKey), typeFromHandle);
					}
					isPersisted = true;
				}
				else
				{
					SetValue(defaultValue);
				}
			}
			return data;
		}

		public virtual void SetValue(T value)
		{
			T val = data;
			Type typeFromHandle = typeof(T);
			string platformKey = GetPlatformKey(key);
			if (typeFromHandle == typeof(short) || typeFromHandle == typeof(int) || typeFromHandle.IsEnum)
			{
				PlayerPrefs.SetInt(platformKey, Convert.ToInt32(value));
			}
			else if (typeFromHandle == typeof(bool))
			{
				PlayerPrefs.SetInt(platformKey, Convert.ToInt32(value));
			}
			else if (typeFromHandle == typeof(float))
			{
				PlayerPrefs.SetFloat(platformKey, Convert.ToSingle(value));
			}
			else if (typeFromHandle.IsEnum)
			{
				PlayerPrefs.SetInt(platformKey, (int)(object)value);
			}
			else
			{
				PlayerPrefs.SetString(platformKey, Convert.ToString(value));
			}
			data = value;
			isPersisted = true;
			if (this.EChanged != null && !val.Equals(data))
			{
				this.EChanged(data);
			}
		}

		public void Remove()
		{
			PlayerPrefs.DeleteKey(GetPlatformKey(key));
		}

		public void Reset()
		{
			SetValue(defaultValue);
		}

		public override string ToString()
		{
			object obj = GetValue();
			return string.Format("{0}[{1}]", GetType().FullName, obj ?? "null");
		}
	}
}
