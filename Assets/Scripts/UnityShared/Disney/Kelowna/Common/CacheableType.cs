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
				if (PlayerPrefs.HasKey(key))
				{
					Type typeFromHandle = typeof(T);
					if (typeFromHandle == typeof(short) || typeFromHandle == typeof(int))
					{
						data = (T)Convert.ChangeType(PlayerPrefs.GetInt(key), typeFromHandle);
					}
					else if (typeFromHandle == typeof(bool))
					{
						data = (T)Convert.ChangeType(PlayerPrefs.GetInt(key), typeFromHandle);
					}
					else if (typeFromHandle == typeof(float))
					{
						data = (T)Convert.ChangeType(PlayerPrefs.GetFloat(key), typeFromHandle);
					}
					else if (typeFromHandle.IsEnum)
					{
						data = (T)Enum.ToObject(typeof(T), PlayerPrefs.GetInt(key));
					}
					else
					{
						data = (T)Convert.ChangeType(PlayerPrefs.GetString(key), typeFromHandle);
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
			if (typeFromHandle == typeof(short) || typeFromHandle == typeof(int) || typeFromHandle.IsEnum)
			{
				PlayerPrefs.SetInt(key, Convert.ToInt32(value));
			}
			else if (typeFromHandle == typeof(bool))
			{
				PlayerPrefs.SetInt(key, Convert.ToInt32(value));
			}
			else if (typeFromHandle == typeof(float))
			{
				PlayerPrefs.SetFloat(key, Convert.ToSingle(value));
			}
			else if (typeFromHandle.IsEnum)
			{
				PlayerPrefs.SetInt(key, (int)(object)value);
			}
			else
			{
				PlayerPrefs.SetString(key, Convert.ToString(value));
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
			PlayerPrefs.DeleteKey(key);
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
