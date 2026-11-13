using System.Collections.Generic;
using UnityEngine;

namespace Disney.MobileNetwork
{
	public class KeyChainManager : MonoBehaviour
	{
		protected Dictionary<string, string> appData = new Dictionary<string, string>();

		public void Awake()
		{
			Service.Set(this);
			Init();
			Object.DontDestroyOnLoad(base.gameObject);
		}

		protected virtual void Init()
		{
		}

		public virtual void PutString(string key, string value)
		{
		}

		public virtual string GetString(string key)
		{
			return "";
		}

		public virtual void RemoveString(string key)
		{
		}
	}
}
