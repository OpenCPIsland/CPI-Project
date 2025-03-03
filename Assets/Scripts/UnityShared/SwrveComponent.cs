using SwrveUnity;
using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SwrveComponent : MonoBehaviour
{
	public SwrveSDK SDK;

	public bool FlushEventsOnApplicationQuit = true;

	protected static SwrveComponent instance;

	public static SwrveComponent Instance
	{
		get
		{
			if (!instance)
			{
				SwrveComponent[] array = UnityEngine.Object.FindObjectsOfType(typeof(SwrveComponent)) as SwrveComponent[];
				if (array != null && array.Length > 0)
				{
					instance = array[0];
				}
				else
				{
					SwrveLog.LogError("There needs to be one active SwrveComponent script on a GameObject in your scene.");
				}
			}
			return instance;
		}
	}

	public SwrveComponent()
	{
		SDK = new SwrveEmpty();
	}

	public void Init(int appId, string apiKey, SwrveConfig config = null)
	{
		if (SDK == null || SDK is SwrveEmpty)
		{
			if (true)
			{
				SDK = new SwrveSDK();
			}
			else
			{
				SDK = new SwrveEmpty();
			}
		}
		if (config == null)
		{
			config = new SwrveConfig();
		}
		SDK.Init(this, appId, apiKey, config);
	}

	public void Start()
	{
		base.useGUILayout = false;
	}

	public void Update()
	{
		if (SDK != null && SDK.Initialised)
		{
			SDK.Update();
		}
	}

	public void OnDestroy()
	{
		if (SDK.Initialised)
		{
			SDK.OnSwrveDestroy();
		}
		StopAllCoroutines();
	}

	public void OnApplicationQuit()
	{
		if (SDK.Initialised && FlushEventsOnApplicationQuit)
		{
			SDK.OnSwrveDestroy();
		}
	}

	public void OnApplicationPause(bool pauseStatus)
	{
		if (SDK != null && SDK.Initialised)
		{
			if (pauseStatus)
			{
				SDK.OnSwrvePause();
			}
			else
			{
				SDK.OnSwrveResume();
			}
		}
	}

	public void SetLocationSegmentVersion(string locationSegmentVersion)
	{
		try
		{
			SDK.SetLocationSegmentVersion(int.Parse(locationSegmentVersion));
		}
		catch (Exception ex)
		{
			SwrveLog.LogError(ex.ToString(), "location");
		}
	}

	public void UserUpdate(string userUpdate)
	{
		try
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(userUpdate);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				dictionary2[enumerator.Current.Key] = string.Format("{0}", enumerator.Current.Value);
			}
			SDK.UserUpdate(dictionary2);
		}
		catch (Exception ex)
		{
			SwrveLog.LogError(ex.ToString(), "userUpdate");
		}
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}
}
